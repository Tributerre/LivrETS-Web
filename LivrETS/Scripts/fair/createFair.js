$(function () {
    $(".date").datetimepicker({
        locale: "fr-ca",
        format: "DD-MM-YYYY",
        minDate: new Date()
    });
    $(".date").each(function () {
        $(this).data("DateTimePicker").date(null);
    });

    $("#btn-add-activity").on("click", function () {
        var $elt = $("#add-activity").clone()
            .removeAttr("id")
            .addClass("add-activity")
            .removeClass("hide")
            .appendTo("#grp-activity");

        $("#grp-activity").find(".date").datetimepicker({
            locale: "fr-ca"
        });
        $("#grp-activity").find(".date").each(function () {
            $(this).data("DateTimePicker").date(null);
        });
    });

    $("#grp-activity").on("click", ".btn-del-activity", function () {
        var $me = $(this);
        $me.parents(".add-activity").remove();
    });
});