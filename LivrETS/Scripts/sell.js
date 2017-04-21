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

    $('[data-toggle="popover"]').popover();
    changeType($("input[name='Type']:checked").val());

    document.getElementById("imageupload").addEventListener("change", function () {
        var $error = $(".error-img");
        $error.hide();
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
            xhr.open("POST", "/Offer/AddImage", true);
            xhr.send(formData);
            $("#image-progress").show();
        } else {
            //$.notifyWarning("5 images maximum par annonce.");
            $error.text("5 images maximum par annonce.").show();
        }
    }, false);

    /**
    * @param val id of course 
    * update the value of input hidden
    */
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
        changeType($(this).val())
    });

    $("#btn-newCourse").on("click", function () {
        var courseTextInput = $("#newCourse");
        var courseTxt = courseTextInput.val();
        var $error = $(".error-acronym");
        var spinner = $("<div>");
        var btnCourseText = $(this).html();
        var restoreButton = function () {
            $("#btn-newCourse").html(btnCourseText);
        };
        $error.hide();

        var pattern = /^[A-Z]{3}[0-9]{3}$/;
        var resultReg = pattern.test(courseTxt);

        if (courseTxt === "") {
            $error.text("Champ obligatoire").show();
            //courseTextInput.parent().addClass("has-error");
        } else if (!resultReg) {
            //$.notifyError("erreur format sigle du cours. Ex: MAT145");
            $error.text("erreur format sigle du cours. Ex: MAT145").show();
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
                url: "/Offer/AddNewCourse",
                data: { acronym: courseTxt },
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
                    $('#btn-choice-class').text(data["acronym"])

                    $("#courses-list").append(newCourseElement);
                    courseTextInput.val("");
                    restoreButton();
                    $.notifySuccess("Le cours a été ajouté à la liste avec succès!");
                },
                statusCode: {
                    500: function () {
                        restoreButton();
                        //$.notifyError("Une erreur est survenue lors du traitement de votre demande. Svp, réessayez.")
                        $error.text("Une erreur est survenue lors du traitement de votre demande. Svp, réessayez.")
                                .show();
                    },
                    409: function () {
                        restoreButton();
                        //$.notifyError("Le cours exsite déjà dans la liste.");
                        $error.text("Le cours exsite déjà dans la liste.").show();
                    }
                }
            });
        }
    });
    
    var $parent = null;
    var imgId = null;
    //delete image 
    $(".btn-del-img").on("click", function () { 
        var $btn = $(this);
        imgId = $btn.data("imgid");
        $parent = $btn.parent(".col-md-2");
        $("#btn-confirm-del-img").attr("data-imgid", imgId);     
    });

    //confirm delete image 
    $("#btn-confirm-del-img").on("click", function () {
        var $btn = $(this);
        //var imgId = $btn.data("imgid");
        var $modal = $('#ModalConfirmDelImg');  
        var $txtError = $modal.find(".text-danger");
        var $loading = $modal.find(".fa-spinner");

        $loading.removeClass("hide");
        $txtError.text("").addClass("hide");
        $btn.prop("disabled", true);

        $.ajax({
            method: "POST",
            url: "/Offer/DeleteImg",
            dataType: "json",
            data: {
                imgIds: [imgId]
            },
            success: function (data) {
                if (data.status == 1) {
                    $btn.prop("disabled", false);
                    $modal.modal('hide');
                    $loading.addClass("hide");
                    $parent.hide();
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

    //update form article
    function changeType(value) {
        
        if (value === 'C') {
            $("#calculator-models-dropdown").show('fast');
            $("#isbn-text-input").hide('fast');
            $("#bloc-course").hide('fast');
        /*}else if(value === 'L'){
            $("#warning-isbn").removeClass("hide");
            $("#calculator-models-dropdown").hide('fast');
            $("#isbn-text-input").show('fast');
            $("#bloc-course").show('fast');*/
        } else {
            $("#calculator-models-dropdown").hide('fast');
            $("#isbn-text-input").show('fast');
            $("#bloc-course").show('fast');
            //$("#warning-isbn").addClass("hide")
        } 
    }
});