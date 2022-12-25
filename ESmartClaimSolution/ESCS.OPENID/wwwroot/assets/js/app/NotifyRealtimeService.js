"use strict";
Object.defineProperty(WebSocket, 'OPEN', { value: 1, }); 

var connection = new signalR.HubConnectionBuilder().withUrl("/notify-service").build();
connection.on("sendToUser", function (thong_bao) {
    var _notifyService = new NotifyService();
    if (thong_bao !== undefined && thong_bao !== null) {
        var homePage = new HomePage();
        _notifyService.error("Bạn có 1 thông báo mới");
        homePage.getNotify(1);
    }
});
connection.start().then(function () {
    connection.invoke("GetConnectionId").then(function (connectionId) {
        console.log(connectionId);
    });
}).catch(function (err) {
    return console.error(err.toString());
});
