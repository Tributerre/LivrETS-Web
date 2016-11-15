$(function () {
    var sortOrder = getParamURL("sortOrder");

    if(sortOrder == 'L'){
        $("#sort-book").addClass("active-sort");
    }else if(sortOrder == 'N'){
        $("#sort-nc").addClass("active-sort");
    }else if(sortOrder == 'C'){
        $("#sort-calc").addClass("active-sort");
    }

    $("#sort-by").on("change", function () {
        document.location.href = "/home/index?sortOrder=" + $(this).val();
    });

    
});

function getParamURL(param) {
    var vars = {};
    window.location.href.replace(location.hash, '').replace(
        /[?&]+([^=&]+)=?([^&]*)?/gi, // regexp
        function (m, key, value) { // callback
            vars[key] = value !== undefined ? value : '';
        }
    );

    if (param) {
        return vars[param] ? vars[param] : null;
    }
    return vars;
}