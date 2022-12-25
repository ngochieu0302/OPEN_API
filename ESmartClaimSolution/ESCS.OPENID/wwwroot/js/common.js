function SelectData(idElement, option = {}) {
    this.id = idElement;
    this.option = option;
    this.idSelect2 = null;
    this.setDefaultValue = function (val) {
        var id = this.id;
        $("#" + id).val(val).trigger('change');
    };
    this.onChange = function (callback = undefined) {
        var id = this.id;
        $("#" + id).on("select2:select", function (e) {
            var val = $(this).val();
            if (callback) {
                callback(val);
            }
        });
    };
    this.onInit = function () {
        var id = this.id;
        var option = this.option;
        option.allowClear = true;
        this.idSelect2 = $("#" + id).select2(option);
        $("#" + id).val("").trigger('change');
    };
    this.onInit();
}