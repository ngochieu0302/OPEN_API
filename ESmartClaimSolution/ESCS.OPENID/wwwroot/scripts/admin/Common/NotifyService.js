//create by: thanhnx.vbi
function NotifyService() {
    this.success = function (msg) {
        $.smallBox({
            title: msg,
            content: "<i class='fa fa-clock-o'></i> <i>Thông báo sẽ đóng sau 4 giây...</i>",
            color: "#71a06a",
            iconSmall: "fa fa-times fa-2x fadeInRight animated",
            timeout: 4000
        });
    };
    this.error = function (msg) {
        $.smallBox({
            title: msg,
            content: "<i class='fa fa-clock-o'></i> <i>Thông báo sẽ đóng sau 4 giây...</i>",
            color: "#C46A69",
            iconSmall: "fa fa-times fa-2x fadeInRight animated",
            timeout: 4000
        });
    };
    this.warning = function (msg) {
        $.smallBox({
            title: msg,
            content: "<i class='fa fa-clock-o'></i> <i>Thông báo sẽ đóng sau 4 giây...</i>",
            color: "#C46A69",
            iconSmall: "fa fa-times fa-2x fadeInRight animated",
            timeout: 4000
        });
    };
}