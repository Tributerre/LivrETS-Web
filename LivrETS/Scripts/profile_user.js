$(function () {
    $('[data-toggle="popover"]').popover();

    // Select all offer's checkbox for action
    $("input[type='checkbox'][name='check-select-all']").on("change", function () {
        var checked = $(this).is(":checked");

        $("tbody>tr>td")
            .find("input[type='checkbox'][name='check-select-offer']")
            .prop("checked", checked);
    });

    //load table offers
    $.fn.dataTable.ext.errMode = 'throw';
    var table = $('table').DataTable({
        processing: true,
        ajax: {       
            url: "/Account/GetOffersByUser",
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) {
                return val.Offers
            }
        },
        columns: [
            {
                class: "check-row text-center",
                sortable: false,
                data: function (val) {
                    return "<input type='checkbox' name='check-select-offer' data-offer-id='" + val.Id + "' />";
                }
            },
            {
                class: "col-md-5",
                data: function (val) {
                    return "<a href='/Offer/Details/" + val.Id + "'>"+ val.Title +"</a>";
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Price;
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Article.TypeName
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Article.Course.Acronym
                }
            },
            {
                data: function (val) {
                    return new Date(parseInt(val.StartDate.replace('/Date(', ''))).toDateString();
                },
                class: "col-md-1 text-center"
            },
            {
                class: "text-center",
                sortable: false,
                data: function (val) {
                    var sold = "";
                    if (val.Sold == true) {
                        sold = "hide"
                    }
                    return "<a class='btn btn-sm btn-success btn-sale " + sold + "' data-offer-id='" + val.Id + "' " +
                        "data-toggle='modal' data-target='#ModalSaleOffer'" +
                        "data-status='1' id='sale'>vendu</a> " +
                        "<a class='btn btn-sm btn-danger btn-sale hide'  data-offer-id='" + val.Id + "' " +
                        "data-status='0' id='nosale'>non vendu</a>"+
                        "<a href='/Offer/Edit/" + val.Id + "' class='btn btn-sm btn-primary btn-edit-offer' data-offer-id='" + val.Id + "'>" +
                        "<span class='glyphicon glyphicon-edit'></span></a> " +
                        "<a href='#' class='btn btn-sm btn-danger btn-del-offer' data-offer-id='" + val.Id + "'"+
                        "data-toggle='modal' data-target='#ModalDelOffer'>" +
                        "<span class='glyphicon glyphicon-trash'></span></a> "
                        

                }
            }
        ]

    });

    /************************************* sale offer *************************************/
    //sale confirmation
    $('table tbody').on("click", ".btn-sale", function () {
        var $btn = $(this);
        var offerId = $btn.data("offer-id");
        $("#btn-confirm-sale-offer").attr("data-offerid", offerId);
    });
    //event sale offer
    $("#btn-confirm-sale-offer").on("click", function () {
        var $btn = $(this);
        var offerId = $btn.data("offerid");
        var $modal = $('#ModalSaleOffer');

        $txtError = $modal.find(".text-danger");
        $loading = $modal.find(".fa-spinner");
        $loading.removeClass("hide");
        $txtError.text("").addClass("hide");
        $btn.prop("disabled", true);

        $.ajax({
            method: "POST",
            url: "/Offer/ConcludeSell",
            dataType: "json",
            data: {
                offerIds: [offerId]
            },
            success: function (data) { console.log(data)
                if (data.status == 1) {
                    table.ajax.reload();
                    $modal.modal('hide');
                    $loading.addClass("hide");
                    $btn.prop("disabled", false);
                } else { 
                    $txtError.text(data.message).removeClass("hide");
                    $loading.addClass("hide");
                    $btn.prop("disabled", false);
                }
               
            },
            error: function () {
                $txtError.text("Erreur").removeClass("hide");
                $loading.addClass("hide");
                $btn.prop("disabled", false);
            }
        });
    });
    /************************************* end sale offer *************************************/

    /************************************* delete offer *************************************/
    //delete confirmation
    var idTmp = null;
    $('table tbody').on("click", ".btn-del-offer", function () {
        var $btn = $(this);
        idTmp = $btn.data("offer-id");

        $("#btn-confirm-del-offer").attr("data-offerid", offerId);
    });

    // Delete selected event            
    $("#del-all").on("click", function () {
        var $me = $(this);
        var $message = $("#request-message");
        var offerIds = [];
        var elts = $("input[type='checkbox'][name='check-select-offer']:checked");

        $me.prop("disabled", true);
        $message.text("").hide();

        elts.each(function () {
            offerIds.push($(this).data("offer-id"));
        });
        
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Offer/DeleteOffer",
            dataType: "json",
            data: JSON.stringify({
                offerIds: offerIds
            }),
            success: function (data) {
                $me.prop("disabled", false);
                if (data.status == true) {
                    elts.each(function () {
                        $(this).parents("tr").remove();
                        $(this).prop("disabled", false);
                    });
                } else 
                    $message.addClass("text-danger").text("La suppression ne s'est pas faite").show();
            },
            error: function () {
                $message.addClass("text-danger").text("La suppression ne s<est pas faite").show();
                $me.prop("disabled", false);
            }
        });
    });

    // Delete each offer.
    $('#btn-confirm-del-offer').on("click", function () {
        var $me = $(this);
        var offerId = $me.data("offerid");   
        var $modal = $('#ModalDelOffer');
        var $txtError = $modal.find(".text-danger");
        var $loading = $modal.find(".fa-spinner");

        $me.prop("disabled", true);
        $loading.removeClass("hide");
        $txtError.text("").addClass("hide");

        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Offer/DeleteOffer",
            dataType: "json",
            data: JSON.stringify({
                offerIds: idTmp
                //type: true

            }),
            success: function (data) { 
                if (data.status == 1) {
                    table.ajax.reload();
                    $modal.modal('hide');
                    $loading.addClass("hide");
                } else {
                    $txtError.text(data.message).removeClass("hide");
                    $loading.addClass("hide");
                }
                $me.prop("disabled", false);
            },
            error: function (data) {
                $txtError.text(data.message).removeClass("hide");
                $loading.addClass("hide");
                $me.prop("disabled", false);
            }
        });
    });
    /************************************* end delete offer *************************************/

})