const pageLogin = "/home/login";
function showLoading(isShow, aceptShow) {
    if (isShow && aceptShow) {
        ESLoading.show('body');
    }
    else {
        ESLoading.hide('body');
    }
}
function Service(isLoading = true) {
    this.isLoading = isLoading;
    this.isArrayData = false;
    this.request = {
        cache: false,
        datatype: 'json',
        headers: {

        },
        beforeSend: function () {
            showLoading(true, isLoading);
        },
        complete: function () {
            showLoading(false, isLoading);
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
                var _notifyService = new NotifyService();
                if (err.status === 427) {
                    _notifyService.error(err.responseJSON.state_info.message_body);
                }
                else if (err.status === 428) {
                    window.location.href = pageLogin;
                }
                else {
                    _notifyService.error(err.responseJSON.state_info.message_body);
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
                console.log(err);
                var _notifyService = new NotifyService();
                if (err.status === 427) {
                    _notifyService.error(err.responseJSON.state_info.message_body);
                }
                else if (err.status === 428) {
                    window.location.href = pageLogin;
                }
                else {
                    _notifyService.error("Đã có lỗi xảy ra");
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
                if (err.status === 427) {
                    alert(err.responseJSON.message);
                }
                else if (err.status === 428) {
                    window.location.href = pageLogin;
                }
                else {
                    alert(err.responseJSON.message);
                }
                reject(err);
            };
            $.ajax(rq);
        });
    };
    this.all = function (arrPromise) {
        return Promise.all(arrPromise);
    }
}