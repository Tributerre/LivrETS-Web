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
            var status = event.target.status;

            switch (status) {
                case 200:
                    console.log("upload completed", event);
                    break;
            }

            $("#image-progress").hide();
        };
        xhr.open("POST", "/Home/AddImage", true);
        xhr.send(formData);
        $("#image-progress").show();
    }, false);
});