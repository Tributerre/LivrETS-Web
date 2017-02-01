$(function () {
    $("#sendMail").on("submit", function (e) {
        e.preventDefault();
        var $form = $(this);
        var $loading = $form.find(".fa-spinner");
        var $txtMsg = $(".txtmsg");
        var $btn = $form.find("button")
        $loading.removeClass("hide");
        $txtMsg.addClass("hide");
        $btn.prop("disabled", true);

        $.ajax({
            method: "POST",
            url: $form.attr("action"),
            dataType: "json",
            data: {
                to_name: $form.find("[name='to_name']").val(),
                to_message: $form.find("[name='to_message']").val(),
                to_address: $form.find("[name='to_address']").val(),
                to_offer: $form.find("[name='to_offer']").val()
            },
            success: function (data) {
                $loading.addClass("hide");
                $btn.prop("disabled", false);
                
                if (data.status == 1) {
                    $txtMsg.removeClass("hide").html(data.message);
                } else {
                    $txtMsg.removeClass("hide alert-success").addClass("alert-danger")
                        .html(data.message); 
                }
            },
            error: function () {
                $txtMsg.text("Erreur").removeClass("hide");
                $loading.addClass("hide");
                $btn.prop("disabled", false);
            }
        });
    });
});