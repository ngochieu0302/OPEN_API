function HomePageService() {
    var _service = new Service();
    this.base = new Service();
    //Lấy thông báo người sử dụng
    this.layThongBaoNguoiDung = function (loading = true) {
        var _service = new Service(true);
        return _service.postData("/home/getnotify");
    };
}