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
    $("thead>tr").find("th").addClass("text-center");

    $("#close-error").on("click", function () {
        $("#errors").hide("slow");
    });

    // Select single fair event.
    $("input[type='checkbox'][name='check-select-fair']").on("change", function () {
        updateDeleteSelectedView();
    });

    // Delete fair event.
    $(".btn-delete-fair").on("click", function () {
        var button = $(this);
        var fairId = $(this).attr("data-fair-id");

        button.prop("disabled", true);
        $.ajax({
            method: "DELETE",
            contentType: "application/json",
            url: "",
            dataType: "json",
            data: JSON.stringify({
                
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
    $(".btn-edit-fair").on("click", function () {
        var fairId = $(this).attr("data-fair-id");
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
            url: "",
            dataType: "json",
            data: JSON.stringify({
                
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
