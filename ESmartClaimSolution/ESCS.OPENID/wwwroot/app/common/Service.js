let URL_LOGIN = "/Admin/Authenticate/Login";
function Service() {
    this.isArrayData = false;
    this.request = {
        cache: false,
        datatype: 'json',
        headers: {},
        beforeSend: function () {
            //showLoading(true);
        },
        complete: function () {
            //console.log("hoàn thành");
            //showLoading(false);
        }
    };
    this.addHeader = function (key, value) {
        this.request.headers[key] = value;
    };
    this.getData = function (url) {
        var rq = this.request;

        return new Promise((resolve, reject) => {
            rq.type = 'get';
            rq.url = url;
            rq.data = {};
            rq.success = function (response) {
                resolve(response);
            };
            rq.error = function (err) {
                if (err.status === 427 || err.status === 401) {
                    toastr.error(err.responseJSON.message);
                }
                if (err.status === 402) {
                    if (URL_LOGIN !== undefined) {
                        window.location.href = URL_LOGIN;
                    }
                }
                reject(err);
            };
            $.ajax(rq);
        });
    };
    this.postData = function (url, data) {
        var rq = this.request;
        var isArr = this.isArrayData;
        return new Promise((resolve, reject) => {
            rq.type = 'post';
            rq.url = url;
            if (isArr) {
                rq.contentType = 'application/json; charset=utf-8';
                rq.data = JSON.stringify(data);
            }
            else {
                rq.data = data;
            }
            rq.success = function (response) {
                resolve(response);
            };
            rq.error = function (err) {
                if (err.status === 427 || err.status === 401) {
                    toastr.error(err.responseJSON.message);
                }
                if (err.status === 402) {
                    if (URL_LOGIN !== undefined) {
                        window.location.href = URL_LOGIN;
                    }
                }
                reject(err);
            };
            $.ajax(rq);
        });
    };
    this.postFormData = function (url, data) {
        var rq = this.request;
        return new Promise((resolve, reject) => {
            rq.type = 'post';
            rq.url = url;
            rq.processData = false;
            rq.data = data;
            rq.contentType = false,
                rq.success = function (response) {
                    resolve(response);
                };
            rq.error = function (err) {
                if (err.status === 427 || err.status === 401) {
                    toastr.error(err.responseJSON.message);
                }
                if (err.status === 402) {
                    if (URL_LOGIN !== undefined) {
                        window.location.href = URL_LOGIN;
                    }
                }
                reject(err);
            };
            $.ajax(rq);
        });
    };
    this.postSigleFile = function (url, idImage) {
        var data = new FormData();
        var files = $('#' + idImage).get(0).files;
        data.append('file', files[0]);
        var rq = this.request;
        return new Promise((resolve, reject) => {
            rq.type = 'post';
            rq.url = url;
            rq.processData = false;
            rq.data = data;
            rq.contentType = false, rq.success = function (response) {
                resolve(response);
            };
            rq.error = function (err) {
                if (err.status === 427 || err.status === 401) {
                    toastr.error(err.responseJSON.message);
                }
                if (err.status == 402) {
                    if (URL_LOGIN !== undefined) {
                        window.location.href = URL_LOGIN;
                    }
                }
                reject(err);
            }
            $.ajax(rq);
        });
    };
}