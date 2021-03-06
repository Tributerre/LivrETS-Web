﻿/*
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
    //init managefairs table width datables plugins

    $('table').DataTable({
        processing: true,
        ajax: {
            url: "/Admin/ListFairs",
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) {
                return val.listFairs
            }
        },
        columns: [
            {
                class: "check-row text-center",
                sortable: false,
                data: function (val) {
                    return "<input type='checkbox' name='check-select-fair' data-fair-id='" + val.Id + "' />";
                }
            },
            {
                data: "LivrETSID",
                visible: false
            },
            {
                data: function (val) {
                    return new Date(parseInt(val.StartDate.replace('/Date(', ''))).toDateString();
                }
            },
            {
                data: function (val) {
                    return new Date(parseInt(val.EndDate.replace('/Date(', ''))).toDateString();
                }
            },
            {
                data: function (val) {
                    var status = ["PREFAIR", "PICKING", "SALE", "RETRIEVAL", "POSTFAIR"];

                    return status[val.Phase];
                }
            },
            {
                class: "text-center",
                sortable: false,
                data: function (val) {
                    return "<a href='#' class='btn btn-sm btn-primary btn-edit-fair' data-fair-id='"+ val.Id +"' ><span class='glyphicon glyphicon-edit'></span></a> "+
                    "<a href='#' class='btn btn-sm btn-danger btn-delete-fair' data-fair-id='" + val.Id + "'><span class='glyphicon glyphicon-trash'></span></a>"
                    
                }
            }
        ]

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
