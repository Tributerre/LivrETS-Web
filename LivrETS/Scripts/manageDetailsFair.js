/*
LivrETS - Centralized system that manages selling of pre-owned ETS goods.
Copyright (C) 2016  TribuTerre

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
$(document).ready(function () {

    var table = $('#tab-offers').DataTable({
        processing: true,
        ajax: {
            url: "/Admin/ListOffersFair",
            data: { id: $("span.fairId").text()},
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
                    return "<a href='/Offer/Details/" + val.Id + "'>" + val.Title + "</a>";
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
                    if (val.Sold) {
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
                data: function (val) {
                    return new Date(parseInt(val.StartDate.replace('/Date(', ''))).toDateString();
                },
                class: "col-md-2 text-center"
            },
            {
                class: "text-center",
                sortable: false,
                data: function (val) {
                    var sold = "";
                    var nosold = "hide";
                    if (val.Sold == true) { 
                        nosold = "";
                        sold = "hide"
                    }

                    return "<a class='btn btn-sm btn-success btn-sale " + sold + "' data-offer-id='" + val.Id + "' " +
                        "data-toggle='modal' data-target='#ModalSaleOffer'"+
                        "data-status='1' id='sale'>vendu</a>" +
                        "<a class='btn btn-sm btn-danger btn-sale " + nosold + " hide' data-offer-id='" + val.Id + "' " +
                        "data-status='0' id='nosale'>non vendu</a>";
                }
            }
        ]

    });

    //table 1
    $('#sold-by-type-article').DataTable({
        processing: true,
        ajax: {
            url: "/Fair/GetTotalSalesByCourse",
            data: { fairId: $("span.fairId").text() },
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) { 
                return val
            }
        },
        columns: [
            {
                data: function (val) {
                    return val.Acronym;
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Ventes;
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Total

                }
            }
        ]

    });

    //table 2
    $('#sold-by-user').DataTable({
        processing: true,
        ajax: {
            url: "/Fair/GetTotalSalesBySeller",
            data: { fairId: $("span.fairId").text() },
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) {
                return val
            }
        },
        columns: [
            {
                data: function (val) {
                    return val.FirstName+ " "+ val.LastName;
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Ventes;
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Total

                }
            }
        ]

    });

    /************************************* sale offer *************************************/
    //sale confirmation
    $('table tbody').on("click", ".btn-sale", function () {
        var $btn = $(this);
        var offerId = $btn.data("offer-id");
        $("#btn-confirm-sale-offer").attr("data-offer-id", offerId);
    });
    // sale fair event.
    $("#btn-confirm-sale-offer").on("click", function () {
        var $btn = $(this);
        var offerId = $btn.data("offer-id");
        var fairId = $(".fairId").text();
        var $modal = $('#ModalSaleOffer');

        $txtError = $modal.find(".text-danger");
        $loading = $modal.find(".fa-spinner");
        $loading.removeClass("hide");
        $txtError.text("").addClass("hide");
        $btn.prop("disabled", true);

        $.ajax({
            method: "POST",
            url: "/Fair/ConcludeSell",
            dataType: "json",
            data: {
                fairId: fairId,
                offerIds: [offerId]
            },
            success: function (data) { 
                if (data.status == 1){
                    $("#sale").removeClass("hide")
                    $("#nosale").addClass("hide")
                    $btn.prop("disabled", false);
                    $modal.modal('hide');
                    $loading.addClass("hide");
                    table.ajax.reload();
                    
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

    $("#close-error").on("click", function () {
        $("#errors").hide("slow");
    });

    // Select single fair event.
    $('table tbody').on("click", ".btn-edit-fair", function () {
        updateDeleteSelectedView();
    });

    // Delete fair event.
    $('table tbody').on("click", ".btn-delete-fair", function () {
        var button = $(this);
        var fairId = $(this).attr("data-fair-id");

        button.prop("disabled", true);
        $.ajax({
            method: "DELETE",
            contentType: "application/json",
            url: "/Admin/DeleteFair",
            dataType: "json",
            data: JSON.stringify({
                Id: fairId
            }),
            success: function () {
                button.parents("tr").remove();
                updateDeleteSelectedView();
            },
            error: function () {
                button.prop("disabled", false);
                $("#error-message").text("Une erreur est survenue lors de la suppression de la foire.");
                $("#errors").show("slow");
            }
        });
    });

    // Edit fair event
    $('table tbody').on("click", ".btn-edit-fair", function () {
        var fairId = $(this).attr("data-fair-id");

        $.ajax({
            method: "POST",
            contentType: "application/json",
            dataType: "json",
            url: "/Admin/GetFairData",
            data: JSON.stringify({
                Id: fairId
            }),
            success: function (fair) {
                var dateFormat = "DD-MM-YYYY";

                $("#NewFairModal").modal("show");

                startDate = moment(fair.StartDate);
                endDate = moment(fair.EndDate);
                pickingStartDate = moment(fair.PickingStartDate);
                pickingEndDate = moment(fair.PickingEndDate);
                saleStartDate = moment(fair.SaleStartDate);
                saleEndDate = moment(fair.SaleEndDate);
                retrievingStartDate = moment(fair.RetrievalStartDate);
                retrievingEndDate = moment(fair.RetrievalEndDate);

                $("#start-date").val(startDate.format(dateFormat));
                $("#end-date").val(endDate.format(dateFormat));
                $("#picking-start-date").val(pickingStartDate.format(dateFormat));
                $("#picking-end-date").val(pickingEndDate.format(dateFormat));
                $("#sale-start-date").val(saleStartDate.format(dateFormat));
                $("#sale-end-date").val(saleEndDate.format(dateFormat));
                $("#retrieval-start-date").val(retrievingStartDate.format(dateFormat));
                $("#retrieval-end-date").val(retrievingEndDate.format(dateFormat));
                window.fairId = fair.Id;

                switch (fair.Trimester) {
                    case "A":
                        $("#select-trimester").val("A");
                        break;

                    case "H":
                        $("#select-trimester").val("H");
                        break;

                    case "É":
                        $("#select-trimester").val("E");
                        break;
                }

                setTimeout(function () { // Wait until the calendar is shown
                    $("#dp-start-date").trigger("dp.change", [startDate]);
                }, 600);
            }
        });
    });

    // Select all fair's checkbox for action
    $("input[type='checkbox'][name='check-select-all']").on("change", function () {
        var checked = $(this).is(":checked");

        $("tbody>tr>td")
            .find("input[type='checkbox'][name='check-select-fair']")
            .prop("checked", checked);
        updateDeleteSelectedView();
    });

    // Delete selected event            
    $("#div-actions").on("click", "#div-delete-selected>#btn-delete-selected", function () {
        $(this).prop("disabled", true);
        var fairIds = [];

        $("input[type='checkbox'][name='check-select-fair']:checked").each(function () {
            fairIds.push($(this).attr("data-fair-id"));
        });

        $.ajax({
            method: "DELETE",
            contentType: "application/json",
            url: "/Admin/DeleteFair",
            dataType: "json",
            data: JSON.stringify({
                Ids: fairIds.join()
            }),
            success: function () {
                $("input[type='checkbox'][name='check-select-fair']:checked").each(function () {
                    $(this).parents("tr").remove();
                    $(this).prop("disabled", false);
                    updateDeleteSelectedView();
                });
            },
            error: function () {
                $("#error-message").text("Une erreur est survenue lors de la suppression des foires. Svp, rechargez la page.");
                $("#errors").show("slow");
            }
        });
    });

    //flot element 
    $(".chart-one").load(
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Fair/GetTotalSalesAmountByArticleType",
            data:JSON.stringify({fairId:$(".fairId").text()}),
            dataType: "json",
            success: function (data) {
                var plotObj = $.plot($("#total_sale_article_type"), data, {
                    series: {
                        pie: {
                            show: true
                        }
                    },
                    grid: {
                        hoverable: true
                    },
                    tooltip: true,
                    tooltipOpts: {
                        content: "%p.0%, %s", // show percentages, rounding to 2 decimal places
                        shifts: {
                            x: 20,
                            y: 0
                        },
                        defaultTheme: false
                    }
                });
            }
        })
    );

    //flot element 
    $(".chart-one").load(
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Fair/GetTotalSalesByArticleType",
            data: JSON.stringify({ fairId: $(".fairId").text() }),
            dataType: "json",
            success: function (data) {
                var plotObj = $.plot($("#nb_sale_article_type"), data, {
                    series: {
                        pie: {
                            show: true
                        }
                    },
                    grid: {
                        hoverable: true
                    },
                    tooltip: true,
                    tooltipOpts: {
                        content: "%p.0%, %s", // show percentages, rounding to 2 decimal places
                        shifts: {
                            x: 20,
                            y: 0
                        },
                        defaultTheme: false
                    }
                });
            }
        })
    );

        

});

/**
 * Updates the selected number in the actions panel.
 */
function updateDeleteSelectedView() {
    var checkedCount = $("tbody>tr>td").find("input[type='checkbox'][name='check-select-fair']:checked").length;

    if ($("#div-delete-selected").is(":visible")) {
        if (checkedCount === 0) {
            $("#div-delete-selected").remove();
        } else {
            $("#nb-delete-selected").text("" + checkedCount);
        }
    } else {
        if (checkedCount > 0) {

            var text =
            $("<p>").append(  // p
                $("<u>")  // u
                    .text(" foire(s) sélectionnée(s)")
                    .prepend($("<span>")  // span
                        .attr("id", "nb-delete-selected")
                        .text("" + checkedCount)
                    )
            );
            var deleteButton = $("<button>")
                .attr("id", "btn-delete-selected")
                .attr("class", "btn btn-danger")
                .text("Supprimer");
            var deleteDiv = $("<div>")
                .attr("class", "row")
                .attr("id", "div-delete-selected")
                .append(text)
                .append(deleteButton);

            $("#div-actions").append(deleteDiv);
        }
    }
}
