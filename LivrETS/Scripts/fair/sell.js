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
    $.sameWidthForElements.apply(this, [
        $("#subtotal-addon"),
        $("#commission-addon"),
        $("#total-addon")
    ])

    $("#article-livretsid").on("keyup", function (event) {
        if (event.keyCode == 13) {  // Enter
            var livretsId = $(this).val()

            if (livretsId == null || livretsId === "")
                return

            $.ajax({
                method: "POST",
                url: "/Fair/OfferInfo",
                dataType: "json",
                data: {
                    LivrETSID: livretsId
                },
                success: function (data) {
                },
                statusCode: {
                    400: function (event, message) {
                        $.notifyError("Une erreur est survenue. Svp réessayez.")
                    },
                    500: function (event, message) {
                        $.notifyError("Une erreur est survenue. Svp réessayez.")
                    }
                }
            })
        }
    })
})