"use strict";
Object.defineProperty(WebSocket, 'OPEN', { value: 1, }); 

var connection = new signalR.HubConnectionBuilder().withUrl("/notify").build();
connection.on("ReceiveNotify", function (partner, titile, message) {
    var _notify = new NotifyService();
    _notify.error("Thông báo lỗi: " + titile);
    var dt = new Date();
    var time = dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();

    var month = dt.getMonth() + 1;
    var day = dt.getDate();
    var output = (day < 10 ? '0' : '') + day + '/' +
        (month < 10 ? '0' : '') + month + '/' +
        dt.getFullYear();
    var datetime = time + " - " + output;
    var id = dt.getFullYear() + (month < 10 ? '0' : '') + month + (day < 10 ? '0' : '') + day + dt.getHours()  + dt.getMinutes() + dt.getSeconds();
    var str = '<div class="alert alert-danger" style="margin-bottom:5px;"><strong> ' + titile + ': </strong> <i style="font-size:10px;">' + datetime + ' - chưa đọc</i><p style="font-size:10px">' + partner + '</p><a href="#" onclick="showDetailError(\''+id+'\')" style="color:#fff;font-size:10px;"><i class="fa fa-angle-double-right" aria-hidden="true"></i> xem chi tiết</a></div>';
    var p = "<p style='display:none' id='error_" + id + "'>" + message + "</p>";
    str = str + p;
    $("#modal-body-content").prepend(str);
});
connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
