// Morris.js Charts sample data for SB Admin template

$(function () {

    $("#morris-area-chart").load(
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Fair/GetStatsFairs",
            dataType: "json",
            success: function (data) {
                console.log(data)
                Morris.Bar({
                    element: 'morris-area-chart',
                    data: data,
                    xkey: 'year',
                    xLabelFormat: function (x) {
                        return x.src.trimester+x.src.year;
                    },
                    ykeys: ['articles', 'articles_sold'],
                    labels: ['articles', 'articles vendus'],
                    hideHover: 'auto',
                    resize: true
                });
            }
        })
    );

});
