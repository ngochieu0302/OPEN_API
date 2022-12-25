//create by: thanhnx.vbi
function Service() {
    this.isArrayData = false;
    this.request = {
        cache: false,
        datatype: 'json',
        headers: {
           
        },
        beforeSend: function () {
            showLoading(true);
        },
        complete: function () {
            showLoading(false);
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
                    $.smallBox({
                        title: err.responseJSON.message,
                        content: "<i class='fa fa-clock-o'></i> <i>Thông báo sẽ đóng sau 4 giây...</i>",
                        color: "#C46A69",
                        iconSmall: "fa fa-times fa-2x fadeInRight animated",
                        timeout: 4000
                    });
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
                    $.smallBox({
                        title: err.responseJSON.message,
                        content: "<i class='fa fa-clock-o'></i> <i>Thông báo sẽ đóng sau 4 giây...</i>",
                        color: "#C46A69",
                        iconSmall: "fa fa-times fa-2x fadeInRight animated",
                        timeout: 4000
                    });
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
                    $.smallBox({
                        title: err.responseJSON.message,
                        content: "<i class='fa fa-clock-o'></i> <i>Thông báo sẽ đóng sau 4 giây...</i>",
                        color: "#C46A69",
                        iconSmall: "fa fa-times fa-2x fadeInRight animated",
                        timeout: 4000
                    });
                }
                reject(err);
            };
            $.ajax(rq);
        });
    };
}