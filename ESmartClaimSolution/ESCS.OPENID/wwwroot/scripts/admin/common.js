$(document).ready(function () {
    $(".code").keypress(function (e) {
        if (e.which < 48 || (e.which > 57 && e.which < 97) || e.which > 122 || e.shiftKey/* z */) {
            e.preventDefault();
        }
    });
    $('.number').keypress(function (event) {
        var keycode = event.which;
        if (!(event.shiftKey == false && (keycode == 46 || keycode == 8 || keycode == 37 || keycode == 39 || (keycode >= 48 && keycode <= 57)))) {
            event.preventDefault();
        }
    });
});
function dangPhatTrien(e) {
    e.preventDefault();
    var notify = new NotifyService();
    notify.error("Tính năng đang phát triển");
};