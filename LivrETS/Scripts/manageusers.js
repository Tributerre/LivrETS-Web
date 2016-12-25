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


    //Id for current user
    var currentId;
    var listRoles;
    var selected = [];

    //init manage users table width datables plugins
    
    var $table = $('table').DataTable({
        processing: true,
        ajax: {
            url: "/Admin/ListUsers",
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) {
                currentId = val.current_id;
                listRoles = val.listRoles;
                return val.listUser
            }
        },
        rowCallback: function( row, data ) {
            if ( $.inArray(data.DT_RowId, selected) !== -1 ) {
                $(row).addClass('selected');
            }
        },
        columns: [
            {
                class: "check-row text-center",
                sortable: false,
                visible: false,
                data: function (val) {
                    if (currentId != val.Id) {
                        return "<input type='checkbox' name='check-select-user-for-action' data-user-id='" + val.Id + "'>";
                    } else {
                        return "<input type='checkbox' style='visibility: hidden;'>";
                    }
                }
            },
            {
                visible: false,
                data: function(val){
                    return val.Id;
                }
            },
            {
                data: "FirstName"
            },
            {
                data: "LastName"
            },
            {
                data: "BarCode"
            },
            {
                data: function (val) {
                    return new Date(parseInt(val.SubscribedAt.replace('/Date(', ''))).toDateString();
                }
            },
            {
                data: function (val) {
                    var html = "<div class='dropdown'>"
                    html += "<button data-toggle='dropdown' id='" + val.Id + "' "+
                        "class='btn btn-default btn-sm dropdown-toggle' aria-haspopup='true' " +
                            "aria-expanded='true'>" + val.Role + " <span class='caret'></span></button>";
                    html += "<ul class='dropdown-menu' aria-labelledby='" + val.Id + "'>";

                    for (var i = 0; i < listRoles.length; i++) {
                        if (val.Role != listRoles[i].Name) {
                            html += "<li><a href='#' data-rolename='" + listRoles[i].Name + "' "+
                                "data-userid='" + val.Id + "' class='elt-role'>" +
                                listRoles[i].Name + "</a></li>";
                        }
                        
                    }

                    html += "<li><input type='radio' id='User' name='UserRole' value='User'>"+
                        "<label for='User'>User</label></li>";
                    html += "</ul>";
                    html += "</div>";
                    return html;
                }
            },
            {
                sortable: false,
                data: function (val) {
                    if (currentId != val.Id) {
                        return "<button type='button' class='btn-delete-user btn btn-danger btn-xs'"+
                                "data-user-id='" + val.Id + "'>" +
                            "<i class='glyphicon glyphicon-trash'></i>" +
                            "</button>"
                    } else {
                        return "<button type='button' style='visibility: hidden;'></button>";
                    }
                }
            }
        ]
        
    });

    $('#myModal').on("hide.bs.modal", function () {
        $('table').find(".selected").removeClass('selected');
    });

    $('table tbody').on('click', 'tr td:not(tr td:last-child)', function () {
        var $modal = $('#myModal');
        var data = $table.row(this).data();

        //change background row
        var id = this.id;
        var index = $.inArray(id, selected);

        if (index === -1) {
            selected.push(id);
        } else {
            selected.splice(index, 1);
        }
        $(this).toggleClass('selected');

        $modal.modal('show');

        $modal.on('shown.bs.modal', function () {
            $.ajax({
                method: "PUT",
                contentType: "application/json",
                url: "/Account/GetUserBy",
                dataType: "json",
                data: JSON.stringify({
                    id: data.Id 
                }),
                success: function (val) {
                    if (val.status) {
                        $("#data-user").html(
                            "<li><b>Nom :</b> "+ val.firstname +"</li>"+
                            "<li><b>Prénpm :</b> "+ val.lastname +"</li>"+
                            "<li><b>Email :</b> "+ val.email +"</li>"+
                            "<li><b>Bar code :</b> "+ val.barcode +"</li>"+
                            "<li><b>Article posté :</b> "+ val.articles +"</li>"+
                            "<li><b>Numéro de téléphone :</b> "+ val.phone +"</li>"
                            );
                    } else {
                        $('table').prepend('<div>Vous avez rater</div>');
                    }
                },
                error: function (err) {
                    console.log(err);
                    $("#error-message").text("Une erreur est survenue lors du changement de privilège.");
                    $("#errors").show("slow");
                }
            });
        });
    });

    

    $("#close-error").on("click", function () {
        $("#errors").hide("slow");
    });

    $('table tbody').on('click', '.elt-role', function (e) {
        e.preventDefault();

        $me = $(this);
        $.ajax({
            method: "PUT",
            contentType: "application/json",
            url: "/Admin/ChangeUserRole",
            dataType: "json",
            data: JSON.stringify({
                UserId: $me.data("userid"),
                NewRole: $me.data('rolename')
            }),
            success: function (val) {
                $('table').append('<div>Vous avez rater</div>');
                if (val.status) {
                    $table.ajax.reload();
                } else {
                    $('table').prepend('<div>Vous avez rater</div>');
                }
            },
            error: function (err) {
                console.log(err);
                $("#error-message").text("Une erreur est survenue lors du changement de privilège.");
                $("#errors").show("slow");
            }
        });
    });

    // Role change event.
    /*$("input[type='radio'][name='UserRole']").on("change", function () {
        console.log("good");
        $.ajax({
            method: "PUT",
            contentType: "application/json",
            url: "/Admin/ChangeUserRole",
            dataType: "json",
            data: JSON.stringify({
                UserId: $(this).attr("data-user-id"),
                NewRole: $(this).val()
            }),
            success: function () { },
            error: function (err) {
                console.log(err);
                $("#error-message").text("Une erreur est survenue lors du changement de privilège.");
                $("#errors").show("slow");
            }
        });
    });*/

    // Select single user event.
    $("input[type='checkbox'][name='check-select-user-for-action']").on("change", function () {
        updateDeleteSelectedView();
    });

    // Delete user event.
    $('table tbody').on("click", ".btn-delete-user", function () {
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
                $table.ajax.reload();
                updateDeleteSelectedView();
            },
            error: function () {
                button.prop("disabled", false);
                $("#error-message").text("Une erreur est survenue lors de la suppression de l'utilisateur.");
                $("#errors").show("slow");
            }
        });
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
            url: "/Admin/DeleteUser",
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