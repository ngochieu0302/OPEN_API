//create by: thanhnx.vbi
function ConfirmService(message, acceptCallback, cancelCallBack = undefined) {
    $.SmartMessageBox({
        title: "Thông báo!",
        content: message,
        buttons: '[Hủy][Chấp nhận]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Chấp nhận") {
            acceptCallback();
        }
        if (ButtonPressed === "Hủy") {
            if (cancelCallBack !== undefined) {
                cancelCallBack();
            }
            return 0;
        }

    });
}