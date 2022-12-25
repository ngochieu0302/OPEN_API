//create by: thanhnx.vbi
function ButtonService(idButton) {
    this.id = idButton;
    this.show = function () {
        $("#" + this.id).show();
    };
    this.hide = function () {
        $("#" + this.id).hide();
    };
    return $("#" + this.id);
}