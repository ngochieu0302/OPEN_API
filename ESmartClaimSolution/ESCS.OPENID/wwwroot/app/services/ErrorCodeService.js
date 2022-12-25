function ErrorCodeService() {
    var _service = new Service();
    this.pageLoad = function () {
        return _service.postData("/errorcode/pageload");
    }
    this.getPaging = function (obj) {
        return _service.postData("/errorcode/getpaging", obj);
    };
    this.save = function (obj) {
        return _service.postData("/errorcode/save", obj);
    };
}