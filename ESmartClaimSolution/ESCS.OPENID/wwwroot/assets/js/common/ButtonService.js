//create by: thanhnx.vbi
function ButtonService(idButton) {
    this.id = idButton;
    this.show = function () {
        $("#" + this.id).show();
        $("#" + this.id).removeClass("d-none");
    };
    this.hide = function () {
        $("#" + this.id).hide();
        $("#" + this.id).addClass("d-none");
    };
    this.disabled = function (isDisable = true) {
        if (isDisable) {
            $("#" + this.id).addClass("disabled");
        }
        else {
            $("#" + this.id).removeClass("disabled");
        }
    };
    return $("#" + this.id);
}