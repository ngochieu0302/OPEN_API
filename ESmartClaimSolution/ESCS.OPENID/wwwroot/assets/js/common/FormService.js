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
function changeValueItem(arr, name, count) {
    for (var i in arr) {
        if (arr[i].name === name) {
            arr[i].count = count;
            break; 
        }
    }
}
function convertDateServerToClient(strDate) {
    if (strDate !== null && strDate !== "") {
        var milli = strDate.replace(/\/Date\((-?\d+)\)\//, '$1');
        var d = new Date(parseInt(milli));
        return d.yyyymmdd();
    }
    return "";
}
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
    //Lấy dữ liệu từ form ra dạng json
    this.getJsonData = function () {
        if (this.frm !== undefined) { 
            var obj = this.frm.serializeObject();
            this.frm.find("[value-format='number']").each(function () {
                var name = $(this).attr("name");
                var val = $(this).val();
                if (val !== undefined && val !== null && val!=="") {
                    obj[name] = val.dateToNumber();
                }
            });
            return obj;
        }
        return {};
    };
    //Lấy dữ liệu từ form dạng FormData (form chứa file)
    this.getFormFileData = function () {
        if (this.frm !== undefined) {
            return new FormData(this.frm[0]);
        }
        return new FormData();
    };
    //Gán giá trị cho các control trong form
    this.setData = function (data) {
        var frm = this.frm;
        var common = new CommonService();
        $.each(data, function (key, value) {
            if (!Array.isArray(value)) {
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
                                    if (value.toString() === val.toString())
                                        $(this).prop('checked', true);
                                    else
                                        $(this).prop('checked', false);
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
                        case "select-multiple":
                            if (value !== null) {
                                ctrl.val(value).trigger('change');
                            }
                            break;
                        case "date":
                            if (value !== null) {
                                ctrl.val(convertDateServerToClient(value.toString()));
                            }
                            break;
                        default:
                            var displayForMat = ctrl.attr("display-format");
                            if (displayForMat !== undefined && displayForMat === "date") {
                                if (!isNaN(value)) {
                                    value = parseInt(value).numberToDate();
                                }
                            }
                            ctrl.val(value);
                    }
                }
            }
            else {
                var ctrlArr = $("[name='" + key + "[]']", frm);
                if (ctrlArr.prop("type") ==="select-multiple") {
                    if (value !== null) {
                        ctrlArr.val(value).trigger('change');
                    }
                }
            }
        });
    };
    //Chọn control muốn thao tác đến nó
    this.getControl = function (name) {
        return this.frm.find("[name='" + name + "']");
    };
    this.getControlById = function (id) {
        return this.frm.find("[id='" + id + "']");
    };
    this.getMultipleControl = function (controls) {
        var arr = controls.split(",");
        var filer = "";
        for (var i = 0; i < arr.length; i++) {
            if (i === 0) {
                filer += "form[name='" + this.formName + "'] [name='" + arr[i].trim() + "']";
            }
            else {
                filer += ",[name='" + arr[i].trim() + "']";
            }
        }
        return $(filer);
    };
    //Kiểm tra dữ liệu form
    this.isValid = function () {
        var meta = this.metadata;
        var frmService = this;
        var check = true;
        var formName =  this.formName;
        $("form[name='" + formName + "'] .invalid").remove();
        $("form[name='" + formName + "']").find("input, select, textarea").each(function (i, el) {
            var attrName = $(el).attr('name');
            $(el).removeClass("is-invalid");
            if (typeof attrName !== typeof undefined && attrName !== false) {
                var label = $(el).prev().text();
                var attrRequired = $(el).attr('required');
                var attrMaxLength = $(el).attr('maxlength');
                var attrMinLength = $(el).attr('minlength');
                var attrMinValue = $(el).attr('min');
                var attrMaxValue = $(el).attr('max');
                var fnValidate = $(el).attr('fn-validate');

                var val = $(el).val();
                var type = $(el).prop("type");
                var parent = $(el).parent();
                var isInputGroup = $(el).parent().hasClass("input-group");
                var message = "";
                if (typeof attrRequired !== typeof undefined && attrRequired !== false) {
                    if (val === "" || val === null || (Array.isArray(val) && val.length <= 0)) {
                        $(el).addClass("is-invalid");
                        //message = "Bạn chưa chọn ";
                        //if (type === "text") {
                        //    message = "Bạn chưa nhập "; 
                        //}
                        //if (isInputGroup) {
                        //    label = $(el).parent().prev().text();
                        //    parent.parent().append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + label + '</small>');
                        //}
                        //else {
                        //    parent.append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + label + '</small>');
                        //}
                        check = false;
                    }
                }
                if (val !== "" && typeof attrMaxLength !== typeof undefined && attrMaxLength !== false && val.length > parseInt(attrMaxLength)) {
                    $(el).addClass("is-invalid");
                    //message = label + "không vượt quá " + attrMaxLength+" ký tự";
                    //if (isInputGroup) {
                    //    label = $(el).parent().prev().text();
                    //    parent.parent().append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message  + '</small>');
                    //}
                    //else {
                    //    parent.append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message  + '</small>');
                    //}
                    check = false;
                }
                if (val !== "" && typeof attrMinLength !== typeof undefined && attrMinLength !== false && val.length < parseInt(attrMinLength)) {
                    $(el).addClass("is-invalid");
                    //message = label + " phải lớn hơn hoặc bằng " + attrMinLength + " ký tự";
                    //if (isInputGroup) {
                    //    label = $(el).parent().prev().text();
                    //    parent.parent().append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                    //}
                    //else {
                    //    parent.append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                    //}
                    check = false;
                }
                if (val !== "" && typeof attrMinValue !== typeof undefined && attrMinValue !== false && parseFloat(val) < parseFloat(attrMinValue)) {
                    $(el).addClass("is-invalid");
                    //message = "Giá trị " + label + " phải lớn hơn  hoặc bằng " + attrMinValue;
                    //if (isInputGroup) {
                    //    label = $(el).parent().prev().text();
                    //    parent.parent().append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                    //}
                    //else {
                    //    parent.append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                    //}
                    check = false;
                }
                if (val !== "" && typeof attrMaxValue !== typeof undefined && attrMaxValue !== false && parseFloat(val) > parseFloat(attrMaxValue)) {
                    $(el).addClass("is-invalid");
                    //message = "Giá trị " + label + " phải nhỏ hơn  hoặc bằng " + attrMaxValue;
                    //if (isInputGroup) {
                    //    label = $(el).parent().prev().text();
                    //    parent.parent().append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                    //}
                    //else {
                    //    parent.append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                    //}
                    check = false;
                }
                if (typeof fnValidate !== typeof undefined && fnValidate !== false) {
                    var result = eval(fnValidate + "(this)");
                    if (result !== "") {
                        $(el).addClass("is-invalid");
                        //message = result;
                        //if (isInputGroup) {
                        //    label = $(el).parent().prev().text();
                        //    parent.parent().append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                        //}
                        //else {
                        //    parent.append('<small id="error_' + formName + '_' + attrName + '" class="form-text text-danger invalid">' + message + '</small>');
                        //}
                        check = false;
                    }
                }
            }
        });
        return check;
    };
    //Xóa message thông báo lỗi của form
    this.clearErrorMessage = function () {
        if (this.frm !== undefined) {
            this.frm.find('.invalid').html("");
            this.frm.find('.is-invalid').removeClass("is-invalid");
        }
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
    this.getValue = function (name) {
        return this.frm.find("[name='" + name + "']").val();
    };
    this.Image = function (id) {
        return this.frm.find("img[id='" + id + "']");
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
    this.OnInit();
}