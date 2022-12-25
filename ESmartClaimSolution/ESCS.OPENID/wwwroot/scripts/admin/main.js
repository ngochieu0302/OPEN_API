$(function () {
    $('.dtp').datetimepicker({
        format: "DD/MM/YYYY"
    });
});
$(document).ready(function () {
    $(".select2").select2({ width: '100%' });
});
function showLoading(isShow) {
    if (isShow) {
        $("#loading-wrapper").show();
        return;
    }
    $("#loading-wrapper").hide();
}