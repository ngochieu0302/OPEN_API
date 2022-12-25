//create by: thanhnx.vbi
var validateMessage = {
    required: function (value, name, attrValue, type = "input") {
        if (!attrValue) {
            return "";
        }
        if (value !== undefined && value !== null && value !== "") {
            return "";
        }
        switch (type) {
            case "select-one":
                return "Bạn chưa chọn " + name + ".";
            case "radio":
                return "Bạn chưa chọn " + name + ".";
            default:
                return "Bạn chưa nhập " + name + ".";
        }
        
    },
    maxValue: function (value, name, attrValue, type = "input") {
        if (value === undefined || value === null || value === "") {
            return "";
        }
        if (isNaN(value)) {
            return name + " không đúng định dạng kiểu số.";
        }
        if (Number(value) > attrValue) {
            return name + " không vượt quá giá trị " + attrValue + ".";
        }
        return "";
    },
    minValue: function (value, name, attrValue, type = "input") {
        if (value !== undefined && value !== null && value !== "") {
            return name + " phải lớn hơn giá trị " + attrValue + ".";
        }
        if (isNaN(value)) {
            return name + " không đúng định dạng kiểu số.";
        }
        if (value < attrValue) {
            return name + " phải lớn hơn giá trị " + attrValue+".";
        }
        return "";
    },
    maxLength: function (value, name, attrValue, type = "input") {
        if (value.length > attrValue) {
            return name + " không vượt quá " + attrValue+" ký tự.";
        }
        return "";
    },
    minLength: function (value, name, attrValue, type = "input") {
        if (value.length < attrValue) {
            return name + " phải lơn hơn " + attrValue + " ký tự.";
        }
        return "";
    },
    pattern: function (value, name, attrValue, type = "input") {
        if (value !== undefined && value !== null && value !== "") {
            return "";
        }
        var patt = new RegExp(attrValue);
        if (!patt.test(value)) {
            return name + " không đúng định dạng.";
        }
        return "";
    }
};
(function ($) {
    $.fn.setDataSource = function (data, displayMember, valueMember, textOption, defaultValue) {
        var select = $(this);
        select.html("");
        if (textOption !== undefined) {
            var option = "<option value=''>" + textOption+"</option>";
            select.append(option);
        }
        $.each(data, function (key, value) {
            var option = "<option value='" + value[valueMember] + "'>" + value[displayMember] + "</option>";
            select.append(option);
        });
        if (defaultValue !== undefined) {
            select.val(defaultValue);
        }
        return this;
    };
    $.fn.isSelectHour = function (textOption = undefined, defaultValue = undefined) {
        var select = $(this);
        select.html("");
        if (textOption !== undefined) {
            var option = "<option value=''>" + textOption + "</option>";
            select.append(option);
        }
        for (var i = 1; i < 24; i++) {
            var optionValue = "<option value='" + i + "'>" + i + " giờ</option>";
            select.append(optionValue);
        }
        if (defaultValue !== undefined) {
            select.val(defaultValue);
        }
        return this;
    };
    $.fn.isSelectMinutes = function (textOption = undefined, defaultValue = undefined) {
        var select = $(this);
        select.html("");
        if (textOption !== undefined) {
            var option = "<option value=''>" + textOption + "</option>";
            select.append(option);
        }
        for (var i = 0; i < 60; i++) {
            var val = "";
            if (i < 10) {
                val = "0" + i;
            }
            else {
                val = i.toString();
            }
            var optionValue = "<option value='" + i + "'>" + val + " phút</option>";
            select.append(optionValue);
        }
        if (defaultValue !== undefined) {
            select.val(defaultValue);
        }
        return this;
    };
    $.fn.setDataSourceDonVi = function (data, displayMember, valueMember, textOption, defaultValue) {
        var select = $(this);
        select.html("");
        if (textOption !== undefined) {
            var option = "<option value=''>" + textOption + "</option>";
            select.append(option);
        }
        $.each(data, function (key, value) {
            var option = "";
            if (value["cap_do"]===1) {
                option = "<option value='" + value[valueMember] + "' class='text-bold'>" + value[displayMember] + "</option>";
            } if (value["cap_do"] === 2) {
                option = "<option value='" + value[valueMember] + "'>+ " + value[displayMember] + "</option>";
            }
            select.append(option);
        });
        if (defaultValue !== undefined) {
            select.val(defaultValue);
        }
        return this;
    };
    $.fn.addEventLoadChildren = function (data, name, parent, displayMember, valueMember, textOption, defaultValue, callback = undefined) {
        var select = $(this);
        var formName = '';
        try {
            formName = "form[name='" + select[0].form.name+"'] ";
        }
        catch
        {
            formName = '';
        }
        select.off("select2:select");
        select.on("select2:select", function (e) {
            var value = $(this).val();
            var selectChildren = $(formName+"select[name='" + name + "']");
            selectChildren.html("");
            if (value === null) {
                return;
            }
            if (textOption !== undefined) {
                var option = "<option value=''>" + textOption + "</option>";
                selectChildren.append(option);
            }
            var arr = $.grep(data, function (n, i) {
                return n[parent].toString() === value.toString();
            });
            $.each(arr, function (key, value) {
                var option = "<option value='" + value[valueMember] + "'>" + value[displayMember] + "</option>";
                selectChildren.append(option);
            });
            if (defaultValue !== undefined) {
                selectChildren.val(defaultValue);
            }
            if (callback !== undefined) {
                callback(value);
            }
        });
        
        return this;
    };
    $.fn.addEventChange = function (callback) {
        var select = $(this);
        var formName = '';
        try {
            formName = "form[name='" + select[0].form.name + "'] ";
        }
        catch
        {
            formName = '';
        }
        select.off("select2:select");
        select.on("select2:select", function (e) {
            var value = $(this).val();
            callback(value);
        });
        return this;
    };
    $.fn.addEventUnSelected = function (callback) {
        var select = $(this);
        var formName = '';
        try {
            formName = "form[name='" + select[0].form.name + "'] ";
        }
        catch
        {
            formName = '';
        }
        select.off("select2:unselect");
        select.on("select2:unselect", function (e) {
            var value = $(this).val();
            callback(value);
        });
        return this;
    };
    $.fn.selectedValue = function (value) {
        var select = $(this);
        if (value !== undefined && value !== null) {
            select.val(value.toString()).trigger("select2:select");
        }
        else {
            select.val("").trigger("select2:select");
        }
        return this;
    };
    $.fn.getValue = function () {
        var element = $(this);
        switch (element.prop("type")) {
            case "radio": 
                var val_radio = null;
                element.each(function () {
                    if ($(this).is(':checked')) {
                        val_radio = $(this).val();
                    }
                });
                return val_radio;
            case "checkbox":
                var val_checkbox = null;
                if (element !== undefined && element.length === 1) {
                    element.each(function () {
                        if ($(this).is(':checked')) {
                            val_checkbox = $(this).val();
                        }
                    });
                }
                else if (element !== undefined && element.length > 1) {
                    val_checkbox = [];
                    element.each(function () {
                        if ($(this).is(':checked')) {
                            val_checkbox.push($(this).val());
                        }
                    });
                }
                return val_checkbox;
            default:
                return element.val();
        }
    };
    $.fn.setValue = function (value) {
        var ctrl = $(this);
        var common = new CommonService();
        switch (ctrl.prop("type")) {
            case "radio":
                ctrl.each(function () {
                    if ($(this).val() === value.toString()) {
                        $(this).attr('checked', true);
                        //$(this).prop('checked', true);
                    }
                });
                break;
            case "file":
                break;
            case "checkbox":
                if (!Array.isArray(value)) {
                    ctrl.each(function () {
                        if ($(this).val() === value.toString())
                            $(this).prop('checked', true);
                    });
                }
                else {
                    ctrl.each(function () {
                        var el = $(this);
                        value.find(function (val) {
                            if (el.val() === val.toString())
                                el.prop('checked', true);
                        });
                    });
                }
                break;
            case "textarea":
                ctrl.html(value);
                break;
            case "select-one":
                if (value !== null) {
                    ctrl.val(value.toString()).trigger('change');
                }
                break;
            case "date":
                if (value !== null) {
                    ctrl.val(common.convertDateServerToClient(value.toString()));
                }
                break;
            default:
                ctrl.val(value);
        }

    };
    $.fn.showImage = function (url) {
        if (url !== undefined && url !== null) {
            $(this).attr("src", url);
        }
    };
    $.fn.setImageDefault = function (type) {
        switch (type) {
            case "NGUOI_DUNG":
                $(this).attr("src", "/Content/asset/img/avatars/anh_dai_dien_default.jpg");
                break;
            default:
        }
    };
    $.fn.readOnly = function (action = true) {
        var ctrl = $(this);
        if (action) {
            ctrl.attr("readonly", "readonly");
        }
        else {
            ctrl.removeAttr("readonly");
        }
    };
    $.fn.addEventChangeImage = function () {
        var ctrl = $(this);
        var name = ctrl[0].name;
        ctrl.change(function () {
            if (ctrl[0].files && ctrl[0].files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#show_' + name).attr('src', e.target.result);
                };
                reader.readAsDataURL(ctrl[0].files[0]);
            }
        });
    };
    $.fn.inputMoney = function () {
        var ctrl = $(this);
        ctrl.autoNumeric('init', { aPad: false, vMin: '0', mDec: '0' });
        return ctrl;
    };
    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
}(jQuery));
function FormService(formName = undefined) {
    this.formName = formName;
    this.method = undefined;
    this.action = undefined;
    this.enctype = undefined;
    this.frm = undefined;
    this.metadata = {};
    this.OnInit = function () {
        if (formName !== undefined && formName !== '') {
            this.frm = $("form[name='" + this.formName + "']");
            if (this.frm !== undefined) {
                this.frm.attr('novalidate', 'novalidate');
                this.method = this.frm.attr('method');
                this.action = this.frm.attr('action');
                this.enctype = this.frm.attr('enctype');
            }
        }

    };
    this.getStringData = function () {
        if (this.frm !== undefined) {
            if (typeof CKEDITOR !== 'undefined') {
                for (var i in CKEDITOR.instances) {
                    CKEDITOR.instances[i].updateElement();
                }
            }
            return this.frm.serialize();
        }
        return "";
    };
    this.getJsonData = function () {
        if (this.frm !== undefined) { 
            return this.frm.serializeObject();
        }
        return {};
    };
    this.getFormFileData = function () {
        if (this.frm !== undefined) {
            return new FormData(this.frm[0]);
        }
        return new FormData();
    };
    this.resetForm = function () {
        if (this.frm !== undefined) {
            this.frm[0].reset();
            this.frm.find("input[type='hidden']").each(function () {
                $(this).val('');
            });
            this.frm.find('select').each(function () {
                $(this).val('').trigger('change');
            });
            this.frm.find('textarea').each(function () {
                $(this).val('');
            });
        }
    };
    this.clearMessage = function () {
        if (this.frm !== undefined) {
            this.frm.find('.invalid').html("");
        }
    };
    this.setData = function (data) {
        var frm = this.frm;
        var common = new CommonService();
        $.each(data, function (key, value) {
            var ctrl = $("[name='" + key + "']", frm);
            var a = ctrl.prop("type");
            if (ctrl.data("type") === "ckeditor" && CKEDITOR !== undefined) {
                var editor = CKEDITOR.instances[key];
                if (editor) { editor.destroy(true); }
                CKEDITOR.replace(key).setData(value);
            }
            else {
                switch (ctrl.prop("type")) {
                    case "radio":
                        ctrl.each(function () {
                            if ($(this).val() === value.toString())
                                $(this).prop('checked', true);
                        });
                        break;
                    case "file":
                        break;
                    case "checkbox":
                        if (!Array.isArray(value)) {
                            ctrl.each(function () {
                                if ($(this).val() === value.toString())
                                    $(this).prop('checked', true);
                            });
                        }
                        else {
                            ctrl.each(function () {
                                var el = $(this);
                                value.find(function (val) {
                                    if (el.val() === val.toString())
                                        el.prop('checked', true);
                                });
                            });
                        }
                        break;
                    case "textarea":
                        ctrl.val(value);
                        break;
                    case "select-one":
                        if (value !== null) {
                            ctrl.val(value.toString()).trigger('change');
                        }
                        break;
                    case "date":
                        if (value !== null) {
                            ctrl.val(common.convertDateServerToClient(value.toString()));
                        }
                        break;
                    default:
                        ctrl.val(value);
                }
            }
            
        });
    };
    this.getControl = function (name) {
        return this.frm.find("[name='" + name + "']");
    };
    this.getValue = function (name) {
        return this.frm.find("[name='" + name + "']").val();
    };
    this.Image = function (id) {
        return this.frm.find("img[id='" + id + "']");
    };
    this.isValid = function () {
        var meta = this.metadata;
        var frmService = this;
        var check = true;
        $(".invalid").remove();
        $.each(meta, function (field, obj) {
            $.each(obj.validate, function (key, value) {
                var val = frmService.getControl(field).getValue();
                var message = "";
                if (validateMessage[key] !== undefined) {
                    message = validateMessage[key](val, obj.name, value, obj.type);
                }
                if (message !== "") {
                    var selector = "form[name='" + frmService.formName + "'] [name='" + field + "']";
                    if (frmService.getControl(field)[0].type === "select-one") {
                        $(selector + " + .select2").after("<div class='invalid' style='color:red'>" + message + "</div>");
                    }
                    else if (frmService.getControl(field)[0].type ==="radio") {
                        selector = "form[name='" + frmService.formName + "'] [name='" + field + "']:first";
                        $(selector).parent().after("<div class='invalid' style='color:red'>" + message + "</div>");
                    }
                    else {
                        $(selector).after("<div class='invalid' style='color:red'>" + message + "</div>");
                    }
                    if (check) {
                        check = false;
                    }
                    return false;
                }
                return true;
            });
        });
        return check;
    };
    this.submit = function (callback) {
        var method = this.method;
        var action = this.action;
        var enctype = this.enctype;
        var form = this;
        this.frm.unbind("submit").submit(function (e) {
            e.preventDefault();
            if (form.isValid()) {
                var service = new Service();
                if (method === undefined || method.trim() === '' ||
                    action === undefined || action.trim() === '' ||
                    (method.toLowerCase() !== "get" && method.toLowerCase() !== "post")) {
                    toastr.error("Form không đủ thông tin để thực hiện gửi dữ liệu.");
                }
                if (method.toLowerCase() === "get") {
                    service.getData(action + "?" + form.getStringData())
                        .then(function (data) { callback(data); })
                        .catch(function (err) {});
                }
                if (method.toLowerCase() === "post") {
                    if (enctype === undefined) {
                        service.postData(action, form.getJsonData())
                            .then(function (data) { callback(data); })
                            .catch(function (err) {});
                    }
                    else if (enctype === "multipart/form-data") {
                        service.postFormData(action, form.getFormFileData())
                            .then(function (data) { callback(data); })
                            .catch(function (err) {});
                    }
                }
            }
        });
        
    };
    this.OnInit();
}