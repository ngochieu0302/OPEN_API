var w_height = $(window).height() - 110;
var w_width = $(window).width() - 5;
var w_height_r = $(window).height() - 110;
var w_width_r = $(window).width() - 5;

var grid_height = $(window).height();
var grid_width = $(window).width();

function FRM_W_H_LAYOUTPAGE() {
    if (w_height < 600) {
        w_height = 600;
        w_height_r = 600;
    }
    if (w_width < 900) {
        w_width = 900;
        w_width_r = 900;
    }
}