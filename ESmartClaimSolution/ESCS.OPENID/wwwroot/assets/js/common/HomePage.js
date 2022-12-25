function HomePage() {
    this.service = new HomePageService();
    this.notifyService = new NotifyService();

    this.buttons = {
        btnShowNotify: new ButtonService("btnShowNotify")
    }
    this.InitPage = function () {
        var _instance = this;
        _instance.getNotify();
    }
    this.getNotify = function (page = 1, callback = undefined) {
        var _instance = this;
        _instance.service.layThongBaoNguoiDung(false).then(res => {
            console.log(res);
            $("head title").html("ESCS - ESmart Claim Solution");
            $("#app-notify-not-read").removeClass("point");
            if (res.state_info.status !== "OK") {
                _instance.notifyService.error(res.state_info.message_body);
                return;
            }
            if (res.data_info.data !== undefined && res.data_info.data !== null && res.data_info.data.length > 0) {
                var tn_chua_doc = res.data_info.data.where(n => n.so_tn_chua_doc > 0).length;
                if (tn_chua_doc>0) {
                    $("#app-notify-not-read").addClass("point");
                    $("head title").html("(" + tn_chua_doc+") ESCS - ESmart Claim Solution");
                }
                ESUtil.genHTML("app_notify_template", "app_notify", res.data_info);
            }
           
        });
    }
}
var _homePage = new HomePage();
$(document).ready(function () {
    _homePage.InitPage();
});