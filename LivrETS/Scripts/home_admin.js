$(function () {
    $("#btn-change-fairs").on("click", function () {
        var $link = $(this);
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: "/Fair/CheckStatusFairs",
            dataType: "json",
            success: function () {
                alert("bon")
            },
            error: function () {
                $("#error-message").text("Une erreur est survenue lors de la suppression de la foire.");
                $("#errors").show("slow");
            }
        });
    });
});