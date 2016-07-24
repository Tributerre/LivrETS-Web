/**
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
along with this program.  If not, see <http://www.gnu.org/licenses/>
**/

$(document).ready(function () {
    'use strict';

    document.getElementById("imageupload").addEventListener("change", function () {
        if ($("#images-panel>div[class='panel-body']").find("img").length < 5) {
            var image = this.files[0];
            var xhr = new XMLHttpRequest();
            var formData = new FormData();
            var progressHandler = function (event) {
                var done = event.position || event.loaded,
                    total = event.totalSize || event.total;
                var percent = Math.floor(done / total * 1000) / 10;

                $("#image-progress>div").css("width", percent);
            };

            formData.append("image", image);
            xhr.addEventListener("progress", progressHandler, false);

            if (xhr.upload) {
                xhr.upload.onprogress = progressHandler;
            }

            xhr.onreadystatechange = function (event) {
                if (xhr.readyState == 4) {
                    if (xhr.status == 200) {
                        var json = JSON.parse(xhr.responseText);
                        var image = $("<div>")
                            .attr("class", "col-md-2")
                            .append($("<img>")
                                .attr("src", json["thumbPath"])
                                .attr("class", "img-responsive img-rounded")
                                .attr("alt", "Preview image")
                            );

                        $("#images-panel>div[class='panel-body']").append(image);
                    }
                }

                $("#image-progress").hide();
                $("#image-progress>div").css("width", 0);
            };
            xhr.open("POST", "/Home/AddImage", true);
            xhr.send(formData);
            $("#image-progress").show();
        } else {
            $.notify({
                message: "5 images maximum par offre."
            }, {
                type: 'warning'
            });
        }
    }, false);

    function setHiddenAcronym(val) {
        $("#hidden-acronym").val(val);
    }

    $("#courses-list").on("change", "li>input[name='Course']", function () {
        setHiddenAcronym($("input[name='Course']:checked").val())
    });

    $("input[name='SellingStrategy']").on("change", function () {
        if ($(this).val() === 'FAIR') {
            $("#info-sellingframe-fair").show('fast');
        } else {
            $("#info-sellingframe-fair").hide('fast');
        }
    });

    $("input[name='Type']").on('change', function () {
        var value = $(this).val();

        if (value === 'C') {
            $("#calculator-models-dropdown").show('fast');
            $("#isbn-text-input").hide('fast');
        } else {
            $("#calculator-models-dropdown").hide('fast');
            $("#isbn-text-input").show('fast');
        }
    });

    $("#btn-newCourse").on("click", function () {
        var courseTextInput = $("#newCourse");
        var spinner = $("<div>");
        var btnCourseText = $(this).html();
        var restoreButton = function () {
            $("#btn-newCourse").html(btnCourseText);
        };
        var notifyError = function (message) {
            $.notify({
                icon: "glyphicon glyphicon-remove",
                message: message
            }, {
                type: "danger"
            });
        };

        if (courseTextInput.val() === "") {
            courseTextInput.parent().addClass("has-error");
        } else {
            $(this).html(spinner);
            spinner.spinner({
                radius: 7,
                strokeWidth: 3
            });

            if (courseTextInput.hasClass("has-error")) {
                courseTextInput.removeClass("has-error");
            }

            $.ajax({
                method: "POST",
                dataType: "json",
                url: "/Home/AddNewCourse",
                data: { acronym: courseTextInput.val() },
                success: function (data) {
                    var newCourseElement = $("<li>")
                        .append($("<input>")
                            .attr("id", data['courseId'])
                            .attr("type", "radio")
                            .attr("name", "Course")
                            .attr("value", data["acronym"]))
                        .append($("<label>")
                            .attr("for", data["courseId"])
                            .text(data["acronym"]));

                    setHiddenAcronym(data["acronym"])

                    $("#courses-list").append(newCourseElement);
                    courseTextInput.val("");
                    restoreButton();
                    $.notify({
                        icon: "glyphicon glyphicon-ok",
                        message: "Le cours a été ajouté à la liste avec succès!"
                    }, {
                        type: 'success'
                    });
                },
                statusCode: {
                    500: function () {
                        restoreButton();
                        notifyError("Une erreur est survenue lors du traitement de votre demande. Svp, réessayez.")
                    },
                    409: function () {
                        restoreButton();
                        notifyError("Le cours exsite déjà dans la liste.");
                    }
                }
            });
        }
    });
});