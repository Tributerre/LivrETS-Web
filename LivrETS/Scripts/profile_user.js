$(function () {
    $('[data-toggle="popover"]').popover()

    //load table offers
    $('#section-activities table').DataTable({
        processing: true,
        ajax: {       
            url: "/Account/GetOffersByUser",
            type: "POST",
            dataType: "JSON",
            dataSrc: function (val) {
                return val.Offers
            }
        },
        columns: [
            {
                class: "check-row text-center",
                sortable: false,
                data: function (val) {
                    return "<input type='checkbox' name='check-select-fair' data-fair-id='" + val.Id + "' />";
                }
            },
            {
                data: "Id",
                class: "Offer-id",
                visible: false
            },
            {
                data: function (val) {
                    return new Date(parseInt(val.StartDate.replace('/Date(', ''))).toDateString();
                },
                class: "col-md-2 text-center"
            },
            {
                data: "Title",
                class: "col-md-5"
            },
            {
                class: "text-center",
                data: function (val) {
                    return val.Price;
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    if (val.Sold){
                        return "Vendu";
                    } else {
                        return "Non vendu";
                    }
                        
                }
            },
            {
                class: "text-center",
                data: function (val) {
                    if (val.ManagedByFair) {
                        return "Non";
                    } else {
                        return "Oui";
                    }
                }
            },
            {
                class: "text-center",
                sortable: false,
                data: function (val) {
                    return "<a href='#' class='btn btn-sm btn-primary btn-edit-fair' data-fair-id='" + val.Id + "' ><span class='glyphicon glyphicon-edit'></span></a> " +
                    "<a href='#' class='btn btn-sm btn-danger btn-delete-fair' data-fair-id='" + val.Id + "'><span class='glyphicon glyphicon-trash'></span></a>"

                }
            }
        ]

    });
})