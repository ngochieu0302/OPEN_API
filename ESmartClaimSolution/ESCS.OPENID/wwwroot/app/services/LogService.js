function LogService() {
    var _service = new Service();
    this.getPaging = function (obj) {
        return _service.postData("/log/requestdata", obj);
    };
    this.getResponseById = function (obj) {
        return _service.postData("/log/responsedatabyid", obj);
    };
}