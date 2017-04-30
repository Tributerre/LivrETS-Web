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
            url: "/Admin/ListCourses",
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) {
                return val.courses
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
                class: "check-row text-center",
                data: function (val) {
                    return val.Acronym;
                }
            },
            {
                class: "text-center",
                sortable: false,
                data: function (val) {
                    var btn1 = "<a class='btn btn-sm btn-danger btn-del-course' data-course-acronym='" + val.Acronym + "' " +
                        "data-status='1' ><span class='fa fa-trash'></span></a>";
                    return btn1;
                }
            }
        ]

    });

    $("#close-error").on("click", function () {
        $("#errors").hide("slow");
    });

    // Select all
    $("input[type='checkbox'][name='check-select-all']").on("change", function () {
        var checked = $(this).is(":checked");

        $("tbody>tr>td")
            .find("input[type='checkbox'][name='check-select-offer']")
            .prop("checked", checked);
    });

    // Delete course
    $('table tbody').on("click", ".btn-del-course", function () {
        var $btn = $(this);
        var acronym = $btn.attr("data-course-acronym");
        var $errorMsg = $("#error-message");
        var $errorBloc = $("#errors");

        $btn.prop("disabled", true);
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Course/DeleteCourse",
            dataType: "json",
            data: JSON.stringify({
                acronym: [acronym]
            }),
            success: function (data) {
                console.log(data)
                if (data.status == 1) 
                    $btn.parents("tr").remove();
                 else {
                    $errorMsg.text(data.message);
                    $errorBloc.show("slow");
                }

                $btn.prop("disabled", false);
                
            },
            error: function () {
                $btn.prop("disabled", false);
                $errorMsg.text("Une erreur est survenue lors de la suppression.");
                $errorBloc.show("slow");
            }
        });
    });

});
