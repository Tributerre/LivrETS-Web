$(function () {
    $('[data-toggle="popover"]').popover();

    //load table offers
    $('table').DataTable({
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
                data: function (val) {
                    return new Date(parseInt(val.StartDate.replace('/Date(', ''))).toDateString();
                },
                class: "col-md-2 text-center"
            },
            {
                data: "Title",
                class: "col-md-5"
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
                    if (val.Sold){
                        return "Vendu";
                    } else {
                        return "Non vendu";
                    }
                        
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    if (val.ManagedByFair) {
                        return "Oui";
                    } else {
                        return "Non";
                    }
                }
            },
            {
                class: "text-center",
                sortable: false,
                data: function (val) {
                    return "<a href='#' class='btn btn-sm btn-primary btn-edit-offer' data-offer-id='" + val.Id + "'>"+
                            "<span class='glyphicon glyphicon-edit'></span></a> " +
                            "<a href='#' class='btn btn-sm btn-danger btn-delete-offer' data-offer-id='" + val.Id + "'>"+
                            "<span class='glyphicon glyphicon-trash'></span></a>"

                }
            }
        ]

    });

    // Select single fair event.
    $('table tbody').on("click", ".btn-edit-offer", function () {
        
    });

    // Select all offer's checkbox for action
    $("input[type='checkbox'][name='check-select-all']").on("change", function () { 
        var checked = $(this).is(":checked");

        $("tbody>tr>td")
            .find("input[type='checkbox'][name='check-select-offer']")
            .prop("checked", checked);
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
            method: "DELETE",
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
    $('table tbody').on("click", ".btn-delete-offer", function () {
        var $me = $(this);
        var offerId = $me.data("offer-id");
        var $message = $("#request-message");

        $me.prop("disabled", true);
        $message.text("").hide();

        $.ajax({
            method: "DELETE",
            contentType: "application/json",
            url: "/Offer/DeleteOffer",
            dataType: "json",
            data: JSON.stringify({
                offerIds: offerId
            }),
            success: function (data) {
                if (data.status == true) 
                    $me.parents("tr").remove(); 
                else
                    $message.addClass("text-danger").text("La suppression ne s<est pas faite").show();
                $me.prop("disabled", false);
            },
            error: function () {
                $message.addClass("text-danger").text("La suppression ne s<est pas faite").show();
                $me.prop("disabled", false);
            }
        });
    });

})