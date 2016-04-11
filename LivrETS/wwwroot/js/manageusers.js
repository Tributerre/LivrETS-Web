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
$(function () {
    $("thead>tr").find("th").addClass("text-center");
    
    $("#close-error").on("click", function () {
        $("#errors").hide("slow");
    });
    
    // Role change event.
    $("input[type='radio'][name='UserRole']").on("change", function () {
        $.ajax({
            method: "PUT",
            contentType: "application/json",
            url: "/Admin/ChangeUserRole",
            dataType: "json",
            data: JSON.stringify({
                UserId: $(this).attr("data-user-id"),
                NewRole: $(this).val()
            }),
            error: function () {
                $("#error-message").text("Une erreur est survenue lors du changement de privilège.");
                $("#errors").show("slow");
            }
        });
    });
    
    // Select single user event.
    $("input[type='checkbox'][name='check-select-user-for-action']").on("change", function () {
        updateDeleteSelectedView();
    });
    
    // Delete user event.
    $(".btn-delete.user").on("click", function () {
        var button = $(this);
        var userId = $(this).attr("data-user-id");

        button.prop("disabled", true);
        $.ajax({
            method: "DELETE",
            contentType: "application/json",
            url: "/Admin/DeleteUser",
            dataType: "json",
            data: JSON.stringify({
                UserId: userId
            }),
            success: function () {
                button.parents("tr").remove();
            },
            error: function () {
                button.prop("disabled", false);
                $("#error-message").text("Une erreur est survenue lors de la suppression de l'utilisateur.");
                $("#errors").show("slow");
            }
        });
        
        updateDeleteSelectedView();
    });
    
    // Select all users's checkbox for action
    $("input[type='checkbox'][name='check-select-all']").on("change", function () {
        var checked = $(this).is(":checked");
        
        $("tbody>tr>td")
            .find("input[type='checkbox'][name='check-select-user-for-action']")
            .prop("checked", checked);
        updateDeleteSelectedView();
    });

    // Delete selected event            
    $("#div-actions").on("click", "#div-delete-selected>#btn-delete-selected", function () {
        $(this).prop("disabled", true);
        var userIds = [];
        
        $("input[type='checkbox'][name='check-select-user-for-action']:checked").each(function () {
            userIds.push($(this).attr("data-user-id"));
        });
        
        $.ajax({
            method: "DELETE",
            contentType: "application/json",
            url: "/Admin/DeleteUsers",
            dataType: "json",
            data: JSON.stringify({
                UserIds: userIds.join()
            }),
            success: function () {
                $("input[type='checkbox'][name='check-select-user-for-action']:checked").each(function () {
                    $(this).parents("tr").remove();
                    $(this).prop("disabled", false);
                    updateDeleteSelectedView();
                });
            },
            error: function () {
                $("#error-message").text("Une erreur est survenue lors de la suppression des l'utilisateurs. Svp, rechargez la page.");
                $("#errors").show("slow");
            }
        });
    });
});

/**
 * Updates the selected number in the actions panel.
 */
function updateDeleteSelectedView() {
    var checkedCount = $("tbody>tr>td").find("input[type='checkbox'][name='check-select-user-for-action']:checked").length;
    
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
                    .text(" utilisateur(s) sélectionné(s)")
                    .append($("<span>")  // span
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