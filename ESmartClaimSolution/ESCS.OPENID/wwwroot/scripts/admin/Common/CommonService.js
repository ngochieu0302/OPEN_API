//create by: thanhnx.vbi
function CommonService() {
    this.bindDataFormDetail = function (obj, prefix = "ct", callback = undefined) {
        for (var propertyName in obj) {
            if ($("#" + prefix+"_" + propertyName) !== undefined) {
                $("#" + prefix+"_" + propertyName).html(obj[propertyName]);
            }
        }
        if (callback !== undefined) {
            callback();
        }
    };
    this.bindData = function (obj, callback = undefined) {
        for (var propertyName in obj) {
            if ($("#" + propertyName) !== undefined) {
                $("#" + propertyName).val(obj[propertyName]);
            }
        }
        if (callback !== undefined) {
            callback();
        }
    };
    this.convertDateServerToClient = function (strDate) {
        if (strDate !== null && strDate!=="") {
            var milli = strDate.replace(/\/Date\((-?\d+)\)\//, '$1');
            var d = new Date(parseInt(milli));
            return d.yyyymmdd();
        }
        return "";
    };
    this.readColumnGrid = function (arr) {
        var arr_column = [];
        arr.forEach(function (item) {
            var obj = {};
            obj.headerName = item.headername;
            obj.field = item.field;
            obj.hide = item.hide === 1 ? true : false;
            obj.width = item.width;
            obj.editable = item.editable === 1 ? true : false;
            arr_column.push(obj);
        });
        return arr_column;
    };
    this.setDateValue = function (object,number) {
        var mask = 'XXXX-XX-XX';
        var s = '' + number, value = '';
        for (var im = 0, is = 0; im < mask.length && is < s.length; im++) {
            value += mask.charAt(im) === 'X' ? s.charAt(is++) : mask.charAt(im);
        }
        object.val(value);
    }
    //validate
    this.setError = function (field, message, formName = undefined) {
        if (formName !== undefined) {
            $("form[name='" + formName + "'] p[field='" + field + "']").html(message);
        }
        else {
            $("p[field='" + field + "']").html(message);
        }
    }
    this.isEmpty = function (val) {
        if (val === undefined || val === null || val.trim() === "") {
            return true;
        }
        return false;
    }
    this.isMaxLength = function (val, number) {
        if (val === undefined || val === null || val.trim() === "") {
            return true;
        }
        if (val.length > number) {
            return false;
        }
        return true;
    }
}
Date.prototype.yyyymmdd = function () {
    var mm = this.getMonth() + 1; 
    var dd = this.getDate();
    return [this.getFullYear(),
    (mm > 9 ? '' : '0') + mm,
    (dd > 9 ? '' : '0') + dd
    ].join('-');
};

Date.prototype.toDateInputValue = (function () {
    var local = new Date(this);
    local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
    return local.toJSON().slice(0, 10);
});