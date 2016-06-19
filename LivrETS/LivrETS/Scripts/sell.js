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
        if ($("#images-panel>div[class='panel-body']>img").length < 5) {
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
            };
            xhr.open("POST", "/Home/AddImage", true);
            xhr.send(formData);
            $("#image-progress").show();
        } else {
            // alert the user of the limit.
        }
    }, false);
});