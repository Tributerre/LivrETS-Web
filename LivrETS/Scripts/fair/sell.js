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
    $("#btn-cash-ok").prop("disabled", true);

    $.sameWidthForElements.apply(this, [
        $("#subtotal-addon"),
        $("#commission-addon"),
        $("#total-addon")
    ])

    $("#btn-reinitialize").on("click", function () {
        window.location.reload()
    })

    $("#btn-conclude").on("click", function () {
        $("#modal-cash").modal("show")
    })

    var total = 0;
    var cashReceived = 0;

    $("#btn-cash-ok").on("click", function () {
        if ($("#modal-cash").is(":visible")) {
            $("#modal-cash").modal("hide")
        }

        var ids = $.map($("#articles-table>tbody").find("td.livretsid"), function (element) {
            return element.innerText
        })
        
        if ((cashReceived - total) >= 0) {
            $.ajax({
                method: "POST",
                url: "/Fair/ConcludeSell",
                dataType: "json",
                data: {
                    ids: ids
                },
                success: function () {
                    $.notifySuccess("Vente réussi")
                    setTimeout(function () {
                        window.location.reload(true)
                    }, 1000)
                },
                statusCode: {
                    400: function (event, message) {
                        $.notifyError("Un des identificateurs n'est pas valide.")
                    },
                    500: function (event, message) {
                        $.notifyError("Une erreur est survenue. Svp réessayez.")
                    }
                }
            });
        } else {
            $.notifyError("Le montant entré n'est pas suffisant")
        }
        
    })

    $("#in-cash-received").on("keyup", function () {
        var currentText = $(this).val()
        var inCashReturned = $("#in-cash-returned")
        $("#btn-cash-ok").prop("disabled", true);

        if (isNaN(currentText)) {
            inCashReturned.val("ERREUR")
        } else {
            if (currentText === '') {
                inCashReturned.val('')
            } else {
                cashReceived = parseFloat(currentText)
                total = parseFloat($("#total").val())
                if ((cashReceived - total) < 0) 
                    $("#btn-cash-ok").prop("disabled", true);
                else 
                    $("#btn-cash-ok").prop("disabled", false);

                inCashReturned.val(cashReceived - total)
            }
        }
    })

    $("#article-livretsid").on("keyup", function (event) {
        if (event.keyCode == 13) {  // Enter
            var livretsId = $(this).val().toUpperCase().trim()

            var ids = $.map($("#articles-table>tbody").find("td.livretsid"), function (element) {
                return element.innerText
            })

            if (livretsId == null || livretsId === "" || ids.indexOf(livretsId) !== -1)
                return

            $.ajax({
                method: "POST",
                url: "/Fair/OfferInfo",
                dataType: "json",
                data: {
                    LivrETSID: livretsId
                },
                success: function (data) {
                    var element = $("<tr>")
                        .append($("<td>").text(data.id).addClass("livretsid"))
                        .append($("<td>").text(data.articleTitle))
                        .append($("<td>").text(data.sellerFullName))
                        .append($("<td>").text(data.offerPrice))

                    $("#articles-table>tbody").append(element)

                    var ids = $.map($("#articles-table>tbody").find("td.livretsid"), function (element) {
                        return element.innerText
                    })
                    $.ajax({
                        method: "POST",
                        url: "/Fair/CalculatePrices",
                        dataType: "json",
                        data: {
                            LivrETSIDs: ids
                        },
                        success: function (data) {
                            $("#subtotal").val(data.subtotal)
                            $("#commission").val(data.commission)
                            $("#total").val(data.total)
                        },
                        statusCode: {
                            400: function (event, message) {
                                $.notifyError("Un des identificateurs n'est pas valide.")
                            },
                            500: function (event, message) {
                                $.notifyError("Une erreur est survenue. Svp réessayez.")
                            }
                        }
                    })
                },
                statusCode: {
                    204: function () {
                        $.notifyWarning("Cet article est déjà vendu.")
                    },
                    400: function (event, message) {
                        $.notifyError("L'identificateur n'est pas valide.")
                    },
                    500: function (event, message) {
                        $.notifyError("Une erreur est survenue. Svp réessayez.")
                    }
                }
            })
        }
    })
})