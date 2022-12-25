function ActionService() {
    var _service = new Service();
    this.pageLoad = function () {
        return _service.postData("/action/pageload");
    }
    this.getPaging = function (obj) {
        return _service.postData("/action/getpaging", obj);
    };
    this.save = function (obj) {
        return _service.postData("/action/save", obj);
    };
    this.saveNew = function (obj) {
        return _service.postData("/action/savenew", obj);
    };
    this.genCode = function () {
        return _service.postData("/action/gencode", {});
    };
    this.clearCache = function (obj) {
        return _service.postData("/action/clearcache", obj);
    };
    this.getParamStored = function (db, schema, storedname, package = null) {
        return _service.postData("/action/getparamstored", {
            db: db,
            schema: schema,
            storedname: storedname,
            package: package
        });
    };
    this.getDetail = function (ac_actionid) {
        return _service.postData("/action/getdetail", { actionid: ac_actionid});
    };
}