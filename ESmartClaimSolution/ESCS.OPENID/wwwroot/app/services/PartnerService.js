function PartnerService() {
    var _service = new Service();
    this.getPaging = function (obj) {
        return _service.postData("/partner/getpaging", obj);
    };
    this.save = function (obj) {
        return _service.postData("/partner/save", obj);
    };
    this.getDetail = function (partner_code) {
        return _service.postData("/partner/getdetail", { code: partner_code});
    };
}