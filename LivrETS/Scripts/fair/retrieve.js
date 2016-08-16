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
    $("#btn-reinitialize").on("click", function () {
        window.location.reload(true)
    })

    $("#btn-retrieve").on("click", function () {
        var ids = $.map($("#articles-table").find(".article-livretsid"), function (element) {
            return element.innerText.toUpperCase().trim()
        })

        $.ajax({
            method: "POST",
            url: "/Fair/RetrieveArticles",
            dataType: "json",
            data: {
                ids: ids
            },
            success: function () {
                setTimeout(function () {
                    window.location.reload(true)
                }, 0001)
            },
            statusCode: {
                500: function () {
                    $.notifyError("Une erreur est survenue. Svp réessayer.")
                }
            }
        })
    })

    $("#in-barcode").on("keyup", function (event) {
        if (event.keyCode == 13) {  // Enter
            var barCode = $(this).val().toUpperCase().trim()

            if (barCode === "")
                return

            $.ajax({
                method: "POST",
                url: "/Fair/OffersNotSold",
                dataType: "json",
                data: {
                    UserBarCode: barCode
                },
                success: function (data) {
                    $.each(data, function (index, value) {
                        var tr = $("<tr>")
                            .append(
                                $("<td>").text(value["id"]).addClass("article-livretsid")
                            ).append(
                                $("<td>").text(value["title"])
                            ).append(
                                $("<td>").text(value["userFullName"])
                            ).append(
                                $("<td>").text(value["price"])
                            )

                        $("#articles-table>tbody").append(tr)
                    })
                },
                statusCode: {
                    404: function () {
                        $.notifyError("L'utilisateur ayant ce code à barres n'a pas été trouvé.")
                    },
                    500: function () {
                        $.notifyError("Une erreur est survenue. Svp réessayer.")
                    }
                }
            })
        }
    })
})