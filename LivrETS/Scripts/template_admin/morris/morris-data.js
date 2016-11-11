// Morris.js Charts sample data for SB Admin template

$(function () {

    $("#morris-area-chart").load(
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Admin/GetStatsFairs",
            dataType: "json",
            success: function (data) {
                Morris.Bar({
                    element: 'morris-area-chart',
                    data: data,
                    xkey: 'year',
                    ykeys: ['articles', 'articles_sold'],
                    labels: ['articles', 'articles vendus'],
                    hideHover: 'auto',
                    resize: true
                });
            }
        })
    );

});
