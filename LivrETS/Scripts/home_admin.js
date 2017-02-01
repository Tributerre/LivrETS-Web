$(function () {
    $("#btn-change-fairs").on("click", function () {
        var $link = $(this);
        var $loading = $(".loading");

        $loading.show();

        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Fair/CheckStatusFair",
            dataType: "json",
            success: function (data) {
                $loading.hide();
            },
            error: function () {
                $loading.hide();
            }
        });
    });

    $('[data-toggle="popover"]').popover();
});