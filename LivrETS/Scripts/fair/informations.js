$(document).ready(function () {

    var table = $('#table-2').DataTable({
        processing: true,
        ajax: {
            url: "/Admin/ListOffersFair",
            data: { id: $("#table-2").data("id") },
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) {
                return val.Offers
            }
        },
        columns: [
           {
               sortable: false,
               data: function (val) {

                   return val.Title
               }
           },
            {
                data: function (val) {
                    return parseFloat(val.Price).toFixed(2) + " $";;
                }
            }
        ]

    });

});