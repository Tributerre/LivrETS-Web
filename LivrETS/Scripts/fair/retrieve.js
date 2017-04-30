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
    });
    $("#in-barcode").focus();

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
    });

    $("#in-barcode").on("keyup", function (event) {
        if (event.keyCode == 13) {  // Enter
            var barCode = $(this).val().toUpperCase().trim()

            var totalPrice = 0;
            var $tbody = $("#articles-table>tbody");

            if (barCode === "")
                return

            //load table offers
            $.fn.dataTable.ext.errMode = 'throw';
            var table = $('#articles-table').DataTable({
                createdRow: function (row, data, index) {
                    if (data.sold) {
                        var pricedisplay = parseFloat($("#retreiveprice").text());
                        var price = parseFloat($('td', row).eq(3).text());

                        var totalPrice = pricedisplay + price;
                        $("#retreiveprice").text(totalPrice.toFixed(2));
                    }
                },
                fnInitComplete: function (oSettings, json) {
                    var btnClear = $('<button class="btn btn-sm btn-success btnClearDataTableFilter">Reset</button>');
                    btnClear.appendTo($('#' + oSettings.sTableId).parents('.dataTables_wrapper').find('.dataTables_filter'));
                    $('#' + oSettings.sTableId + '_wrapper .btnClearDataTableFilter').click(function () {
                        $('#' + oSettings.sTableId).dataTable().fnFilter('');
                    });
                },
                scrollY: "500px",
                scrollCollapse: true,
                paging: false,
                processing: true,
                ajax: {
                    url: "/Fair/OffersNotSold",
                    type: "POST",
                    dataType: "JSON",
                    data: { UserBarCode: barCode },
                    dataSrc: function (val) {
                        return val
                    }
                },
                columns: [
                    {
                        data: function (val) {
                            return val.id;
                        }
                    },
                    {
                        searchable: false,
                        data: function (val) {
                            return val.title;
                        }
                    },
                    {
                        searchable: false,
                        data: function (val) {
                            return val.userFullName;
                        }
                    },
                    {
                        searchable: false,
                        data: function (val) {
                            return val.price;
                        }
                    },
                    {
                        searchable: false,
                        data: function (val, row) {
                            
                            if (val.sold) {
                                return "vendu";
                            }

                            return "non vendu";
                        }
                    }
                ]


            });

            var input_search = $("input[type='search']");
            input_search.focus();
            $("#btn-reset-search").on("click", function (e) {
                /*input_search.val("");
                table.ajax.refresh();*/
                //oTable_events.fnFilter($("input[type='search']").val())
            });

        }
    });

    
    
});
