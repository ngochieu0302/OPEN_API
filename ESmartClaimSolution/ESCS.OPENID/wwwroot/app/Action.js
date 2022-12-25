const GRID_HO_SO_SO_DONG = 12;
const DEFAULT_DB = "202008011500005";
const DEFAULT_SCHEMA = "202008011600002";
const DEFAULT_LOAI_FILE = "TAI_LIEU";
const DEFAULT_LOAI_MAY_CHU = "1";
const DEFAULT_CACHE = {
    DEFAULT_DB_CACHE: "0",
    CONNECTION_NAME: "ESCS",
    MAT_KHAU: "Thang@Binh",
    PORT: "6379",
    TIME_LIVE: "30",
    SERVER_CACHE: "202008011500003",
};
const DEFAULT_LOAI_MAY_CHU_FILE = {
    DUONG_DAN_GOC: "D:\\wwwroot",
    THU_MUC_GOC: "FILE_CAM_XOA",
    IP_MAY_CHU: "27.71.231.47",
    TAI_KHOAN: "administrator",
    MAT_KHAU: "Thang@Binh"
};

var objDanhMuc = {};
var _commonService = new CommonService();
var _notifyService = new NotifyService();
var _frmTKiem = new FormService("frmTKiem");
var _frmAction = new FormService("frmAction");
var _actionService = new ActionService();
var configColumn = [
    { field: "r__", title: "STT", width: "4%", hozAlign: "center", headerSort: false },
    { field: "exc_actioncode", title: "Mã action api", width: "10%", headerSort: false, formatter: "html" },
    { field: "ac_action_name", title: "Tên action api", width: "25%", headerSort: false },
    { field: "ac_action_type", title: "Loại action", width: "8%", hozAlign: "center", headerSort: false },
    { field: "exc_storedprocedure", title: "Tên thủ tục", width: "18%", headerSort: false },
    { field: "exc_package_name", title: "Package", width: "8%", hozAlign: "center", headerSort: false },
    { field: "ac_is_internal", title: "Public", width: "6%", hozAlign: "center", headerSort: false, formatter: formatterTypeInternal },
    { field: "ac_type_cache", title: "Cache", width: "4%", hozAlign: "center", headerSort: false, formatter: formatterTypeCache },
    { field: "ac_send_notify", title: "Notify", width: "5%", hozAlign: "center", headerSort: false, formatter: formatterNotify },
    { field: "clear_cache_name", title: "Action api xóa cache", width: "15%", headerSort: false, formatter: "html" },
    { field: "sc_schema", title: "Schema", width: "5%", hozAlign: "center", headerSort: false },
    { field: "db_dbname", title: "Database", width: "8%", hozAlign: "center", headerSort: false },
    { field: "exc_isactive_text", title: "Trạng thái", width: "9%", hozAlign: "center", headerSort: false }
];
var _gridAction = new GridViewService("gridAction", configColumn, getPaging, rowClick);
function formatterTypeInternal(cell, formatterParams, onRendered) {
    var val = cell.getValue();
    if (val === "PUBLIC") {
        return '<i class="fa fa-check" style="color:#28a745" aria-hidden="true"></i>';
    }
    if (val === "PRIVATE") {
        return '<i class="fa fa-times" style="color:#dc3545" aria-hidden="true"></i>';
    }
    return '';
}
function formatterTypeCache(cell, formatterParams, onRendered) {
    var val = cell.getValue();
    if (val === "ALLOW_CACHE") {
        return '<i class="fa fa-check" style="color:#28a745" aria-hidden="true"></i>';
    }
    if (val === "NONE") {
        return '<i class="fa fa-times" style="color:#dc3545" aria-hidden="true"></i>';
    }
    return '';
}
function formatterNotify(cell, formatterParams, onRendered) {
    var val = cell.getValue();
    if (val === "NOTIFY") {
        return '<i class="fas fa-comment-dots" style="color:#28a745"></i>';
    }
    return '';
}
function getPaging(trang) {
    var objTimKiem = _frmTKiem.getJsonData();
    objTimKiem.search = objTimKiem.search.trim();
    objTimKiem.trang = trang;
    objTimKiem.so_dong = GRID_HO_SO_SO_DONG;
    _actionService.getPaging(objTimKiem).then(res => {
        _gridAction.setDataSource(res, trang);
        if (res.data !== null && res.data.length < GRID_HO_SO_SO_DONG) {
            _gridAction.addRowEmpty(GRID_HO_SO_SO_DONG - res.data.length);
        }
    });
}
function rowClick(data, row) {
    _actionService.getDetail(data.ac_actionid).then(res => {
        $("#btnXoaCache").removeClass("d-none");
        var obj = chuanHoaJson(res);
        hienThiChiTiet(obj);
        for (var i = 0; i < row.getTable().getRows().length; i++) {
            row.getTable().getRows()[i].deselect();
        }
        row.select();
        showModalBoiThuong();
    });
}
function chuanHoaJson(res) {
    var obj = {};
    for (var property in res.action) {
        obj["ac_" + property] = res.action[property];
    }
    if (res.action_config !== null) {
        for (var property in res.action_config) {
            obj["acf_" + property] = res.action_config[property];
        }
    }
    if (res.action_exc_db !== null) {
        for (var property in res.action_exc_db) {
            obj["exc_" + property] = res.action_exc_db[property];
        }
    }
    if (res.action_file !== null) {
        for (var property in res.action_file) {
            obj["file_" + property] = res.action_file[property];
        }
    }
    if (res.action_mail !== null) {
        for (var property in res.action_mail) {
            obj["mail_" + property] = res.action_mail[property];
        }
    }
    obj.is_ac_send_notify = (obj.ac_send_notify != undefined && obj.ac_send_notify != null && obj.ac_send_notify != "") ? "1" : "0";
    return obj;
}
function pageLoad(callback = undefined) {
    _actionService.pageLoad().then(res => {
        objDanhMuc = res;
        _frmTKiem.getControl("db_id").setDataSource(res.database, "db_dbname", "db_id", "Chọn database");
        _frmTKiem.getControl("db_id").addEventChange(val => {
            var arr = $.grep(objDanhMuc.schema, (el, index) => { return el.sc_database_id == val; });
            _frmTKiem.getControl("sc_id").setDataSource(arr, "sc_schema", "sc_id", "Chọn schema");
            _frmTKiem.getControl("sc_id").setValue("");
        });

        _frmAction.getControl("db_id").setDataSource(res.database, "db_dbname", "db_id", "Chọn database", DEFAULT_DB);
        _frmAction.getControl("db_id").addEventChange(val => {
            var arr = $.grep(objDanhMuc.schema, (el, index) => { return el.sc_database_id == val; });
            _frmAction.getControl("exc_schema_id").setDataSource(arr, "sc_schema", "sc_id", "Chọn schema");
            _frmAction.getControl("exc_schema_id").setValue("");
        });
        _frmAction.getControl("file_db_id").setDataSource(res.database, "db_dbname", "db_id", "Chọn database", DEFAULT_DB);
        _frmAction.getControl("file_db_id").addEventChange(val => {
            var arr = $.grep(objDanhMuc.schema, (el, index) => { return el.sc_database_id == val; });
            _frmAction.getControl("file_schema_id").setDataSource(arr, "sc_schema", "sc_id", "Chọn schema");
            _frmAction.getControl("file_schema_id").setValue("");
        });
        _frmAction.getControl("mail_db_id").setDataSource(res.database, "db_dbname", "db_id", "Chọn database", DEFAULT_DB);
        _frmAction.getControl("mail_db_id").addEventChange(val => {
            var arr = $.grep(objDanhMuc.schema, (el, index) => { return el.sc_database_id == val; });
            _frmAction.getControl("mail_schema_id").setDataSource(arr, "sc_schema", "sc_id", "Chọn schema");
            _frmAction.getControl("mail_schema_id").setValue("");
        });

        _frmAction.getControl("db_id").trigger("select2:select");
        _frmAction.getControl("file_db_id").trigger("select2:select");
        _frmAction.getControl("mail_db_id").trigger("select2:select");
        _frmAction.getControl("exc_schema_id").setValue(DEFAULT_SCHEMA);
        _frmAction.getControl("file_schema_id").setValue(DEFAULT_SCHEMA);
        _frmAction.getControl("mail_schema_id").setValue(DEFAULT_SCHEMA);

        var arrServerCache = $.grep(objDanhMuc.server, (el, index) => { return el.sv_cat_server.includes("CACHE"); });
        _frmAction.getControl("acf_id_server_cache").setDataSource(arrServerCache, "cf_server_ip", "sv_id", "Chọn server cache");
        _frmAction.getControl("acf_id_server_cache").setValue(arrServerCache[0].sv_id);

    });
    getPaging(1);
}
function showModalBoiThuong(ishow = true) {
    $('#inside-modal .nav-tabs.profile-tab').tabdrop();
    $("#inside-modal").modal("show");
    $(".page-wrapper").addClass("position-relative");
    $("#inside-modal").addClass("position-absolute");
    $("#inside-modal").css("padding-right", "");
    $("#inside-modal").css("padding-left", "");
    $("#inside-modal").css("padding-top", "50px");
    $("#inside-modal").css("z-index", "9");
    $('.modal-backdrop').hide();
    $('body').removeClass("modal-open");
    $('body').css("padding-right", "0px");
    if (!ishow) {
        $("#inside-modal").modal("hide");
    }
}
function anHienDiv(div, hthi = true) {
    if (hthi) {
        $("#" + div).removeClass("d-none");
    }
    else {
        $("#" + div).addClass("d-none");
    }
}
function hienThiChiTiet(obj) {
    console.log(obj);
    _frmAction.getControl("ac_action_type").setValue(obj.ac_action_type);
    _frmAction.getControl("ac_action_type").trigger("select2:select");
    _frmAction.getControl("ac_action_type").readOnly();
    _frmAction.getControl("ac_type_cache").setValue(obj.ac_type_cache);
    _frmAction.getControl("ac_type_cache").trigger("select2:select");
    _frmAction.getControl("ac_type_ddos").setValue(obj.ac_type_ddos);
    _frmAction.getControl("ac_type_ddos").trigger("select2:select");
    _frmAction.getControl("is_ac_send_notify").setValue(obj.is_ac_send_notify);
    _frmAction.getControl("is_ac_send_notify").trigger("select2:select");
    _frmAction.getControl("ac_is_file").setValue((obj.ac_is_file == null || obj.ac_is_file == "") ? "K" : obj.ac_is_file);
    _frmAction.getControl("ac_is_file").trigger("select2:select");

    if (obj.ac_send_notify != undefined && obj.ac_send_notify != null && obj.ac_send_notify != "") {
        var arr = obj.ac_send_notify.split(";");
        $("#notify_NOTIFY").prop("checked", false);
        $("#notify_SMS").prop("checked", false);
        $("#notify_EMAIL").prop("checked", false);
        if (arr.includes("NOTIFY")) {
            $("#notify_NOTIFY").prop("checked", true);
        }
        if (arr.includes("EMAIL")) {
            $("#notify_EMAIL").prop("checked", true);
        }
        if (arr.includes("SMS")) {
            $("#notify_SMS").prop("checked", true);
        }
    }
    _frmAction.getControl("db_id").setValue(DEFAULT_DB);
    _frmAction.getControl("db_id").trigger("select2:select");
    _frmAction.getControl("exc_schema_id").setValue(DEFAULT_SCHEMA);
    _frmAction.getControl("file_db_id").setValue(DEFAULT_DB);
    _frmAction.getControl("file_db_id").trigger("select2:select");
    _frmAction.getControl("file_schema_id").setValue(DEFAULT_SCHEMA);
    _frmAction.getControl("mail_db_id").setValue(DEFAULT_DB);
    _frmAction.getControl("mail_db_id").trigger("select2:select");
    _frmAction.getControl("mail_schema_id").setValue(DEFAULT_SCHEMA);

    _frmAction.getControl("file_is_local").setValue(obj.file_is_local);
    _frmAction.getControl("file_is_local").trigger("select2:select");
    _frmAction.getControl("mail_is_attach_file").setValue(obj.mail_is_attach_file);
    _frmAction.getControl("mail_is_attach_file").trigger("select2:select");
    _frmAction.getControl("mail_is_local").setValue(obj.mail_is_local);
    _frmAction.getControl("mail_is_local").trigger("select2:select");

    _frmAction.setData(obj);
}
function isRequired(val, error_message) {
    if (val === undefined || val === null || val.trim() === "") {
        _notifyService.error(error_message);
        return false;
    }
    return true;
}
function validFormAction(obj) {
    if (!isRequired(obj.ac_action_name, "Bạn chưa nhập tên action api"))
        return false;
    if (!isRequired(obj.ac_action_type, "Bạn chưa chọn loại action"))
        return false;
    if (!isRequired(obj.ac_type_cache, "Bạn chưa chọn sử dụng cache"))
        return false;
    if (!isRequired(obj.ac_type_ddos, "Bạn chưa chọn áp dụng ddos"))
        return false;
    if (obj.ac_action_type === "EXCUTE_DB") {
        if (!isRequired(obj.exc_storedprocedure, "Bạn chưa nhập tên thủ tục"))
            return false;
        if (!isRequired(obj.exc_type_exec, "Bạn chưa chọn loại kết quả trả về"))
            return false;
        if (!isRequired(obj.db_id, "Bạn chưa chọn database"))
            return false;
        if (!isRequired(obj.exc_schema_id, "Bạn chưa chọn schema"))
            return false;
    }
    if (obj.ac_action_type === "FILE") {
        if (!isRequired(obj.file_storedprocedure, "Bạn chưa nhập tên thủ tục"))
            return false;
        if (!isRequired(obj.file_type_exec, "Bạn chưa chọn loại kết quả trả về"))
            return false;
        if (!isRequired(obj.file_db_id, "Bạn chưa chọn database"))
            return false;
        if (!isRequired(obj.file_schema_id, "Bạn chưa chọn schema"))
            return false;
        if (!isRequired(obj.file_type_file, "Bạn chưa chọn loại file lưu trữ"))
            return false;
        if (!isRequired(obj.file_is_local, "Bạn chưa chọn loại máy chủ file"))
            return false;
        if (!isRequired(obj.file_extensions_file, "Bạn chưa nhập extension file"))
            return false;
        if (!isRequired(obj.file_ip_remote, "Bạn chưa nhập IP máy chủ file/đường dẫn gốc"))
            return false;
        if (!isRequired(obj.file_base_folder, "Bạn chưa nhập thư mục gốc"))
            return false;
    }
    if (obj.ac_action_type === "MAIL") {
        if (!isRequired(obj.mail_storedprocedure, "Bạn chưa nhập tên thủ tục"))
            return false;
        if (!isRequired(obj.mail_type_exec, "Bạn chưa chọn loại kết quả trả về"))
            return false;
        if (!isRequired(obj.mail_db_id, "Bạn chưa chọn database"))
            return false;
        if (!isRequired(obj.mail_schema_id, "Bạn chưa chọn schema"))
            return false;
        if (!isRequired(obj.mail_is_attach_file, "Bạn chưa chọn đính kèm file"))
            return false;
    }
    if (obj.ac_type_cache === "ALLOW_CACHE") {
        if (!isRequired(obj.acf_prefix_key_cache, "Bạn chưa nhập tiền tố key cache"))
            return false;
        if (!isRequired(obj.acf_cache_connection_name, "Bạn chưa nhập tên connection cache"))
            return false;
        if (!isRequired(obj.acf_cache_password, "Bạn chưa nhập mật khẩu DB cache"))
            return false;
        if (!isRequired(obj.acf_db_cache, "Bạn chưa chọn database cache"))
            return false;
        if (!isRequired(obj.acf_id_server_cache, "Bạn chưa chọn server cache"))
            return false;
        if (!isRequired(obj.acf_cache_port, "Bạn chưa nhập port server cache"))
            return false;
        if (!isRequired(obj.ac_time_live_cache, "Bạn chưa nhập thời gian sống của cache"))
            return false;
    }
    if (obj.ac_type_ddos === "APPLY") {
        if (!isRequired(obj.ac_max_rq_ddos, "Bạn chưa nhập Số lượng request tối đa"))
            return false;
        if (!isRequired(obj.ac_max_time_ddos, "Bạn chưa nhập Thời gian cho phép"))
            return false;
        if (!isRequired(obj.ac_time_lock, "Bạn chưa nhập Thời gian khóa hành động"))
            return false;
    }
    return true;
}
function onThayDoiThongBao(el) {
    var notify = "";
    $(".ac_notify:checked").each(function () {
        if (notify == "") {
            notify += $(this).val();
        }
        else {
            notify += ";" + $(this).val();
        }
    });
    _frmAction.getControl("ac_send_notify").setValue(notify);
}
$(document).ready(function () {
    pageLoad();
    anHienDiv("divExcuteDB", true);
    anHienDiv("divFile", false);
    anHienDiv("divMail", false);
    anHienDiv("divCauHinhAction", false);
    anHienDiv("divDDOS", false);
    anHienDiv("divCauHinhThongTinMayChuFile", false);
    anHienDiv("divCauHinhThongTinAnh", false);
    anHienDiv("divCauHinhThongTinMayChuFileMail", false);
    anHienDiv("divCauHinhApiGateWay", false);
    $("#btnFormTimKiem").click(function () {
        getPaging(1);
    });
    $("#btnXoaCache").click(function () {
        _actionService.clearCache(_frmAction.getJsonData()).then(res => {
            _notifyService.success("Xóa cache thành công.")
        });
    });
    $("#btnThemMoi").click(function () {
        _frmAction.getControl("ac_action_type").readOnly(false);
        _frmAction.resetForm();
        var objAction = _frmAction.getJsonData();
        objAction.ac_order_exc = 1;
        objAction.ac_is_async = "SYNC";
        objAction.ac_isactive = 1;
        objAction.ac_action_type = "EXCUTE_DB";
        objAction.ac_type_cache = "NONE";
        objAction.ac_is_internal = "PRIVATE";
        objAction.ac_send_notify = "";
        objAction.is_ac_send_notify = "0";
        objAction.ac_is_file = "K";
        objAction.ac_type_ddos = "NONE";
        objAction.ac_check_authen = "C";
        objAction.exc_type_exec = "RETURN_NONE";
        objAction.ac_api_gateway = "K";
        objAction.db_id = DEFAULT_DB;
        objAction.exc_schema_id = DEFAULT_SCHEMA;
        _frmAction.setData(objAction);
        _frmAction.getControl("ac_is_file").trigger("select2:select");
        _frmAction.getControl("ac_action_type").trigger("select2:select");
        _frmAction.getControl("ac_type_cache").trigger("select2:select");
        _frmAction.getControl("ac_type_ddos").trigger("select2:select");
        _frmAction.getControl("mail_is_attach_file").trigger("select2:select");
        _frmAction.getControl("is_ac_send_notify").trigger("select2:select");
        $("#btnXoaCache").addClass("d-none");
        showModalBoiThuong();
    });
    $("#btnLuuAction").click(function () {
        var obj = _frmAction.getJsonData();
        if (!validFormAction(obj)) {
            return;
        }
        _actionService.saveNew(obj).then(res => {
            getPaging(1);
            showModalBoiThuong(false);
            _notifyService.success("Lưu thông tin thành công");
        });
    });
    $("#btnClone").click(function () {
        _frmAction.getControl("ac_actionid").val("");
        _frmAction.getControl("exc_actioncode").val("");
        _frmAction.getControl("file_actioncode").val("");
        _frmAction.getControl("mail_actioncode").val("");

        _frmAction.getControl("ac_action_name").val("");
        _frmAction.getControl("exc_storedprocedure").val("");
        _frmAction.getControl("exc_package_name").val("");

        _frmAction.getControl("file_storedprocedure").val("");
        _frmAction.getControl("file_package_name").val("");

        _frmAction.getControl("mail_storedprocedure").val("");
        _frmAction.getControl("mail_package_name").val("");

        _frmAction.getControl("acf_prefix_key_cache").val("");
        _frmAction.getControl("acf_prefix_key_cache").val("");
        _notifyService.success("Đã thực hiện copy");
    });
    _frmAction.getControl("ac_action_type").addEventChange(val => {
        anHienDiv("divExcuteDB", false);
        anHienDiv("divFile", false);
        anHienDiv("divCauHinhThongTinMayChuFile", false);
        anHienDiv("divCauHinhThongTinAnh", false);

        anHienDiv("divMail", false);

        anHienDiv("divCauHinhThongTinMayChuFileMail", false);
        anHienDiv("divCauHinhThongTinTaiKhoanMayChuFileMail", false);

        switch (val) {
            case "EXCUTE_DB":
                anHienDiv("divExcuteDB", true);
                break
            case "FILE":
                anHienDiv("divFile", true);
                _frmAction.getControl("file_db_id").setValue(DEFAULT_DB);
                _frmAction.getControl("file_db_id").trigger("select2:select");
                _frmAction.getControl("file_schema_id").setValue(DEFAULT_SCHEMA);
                _frmAction.getControl("file_type_exec").setValue("RETURN_NONE");

                _frmAction.getControl("file_type_file").setValue(DEFAULT_LOAI_FILE);
                _frmAction.getControl("file_is_local").setValue(DEFAULT_LOAI_MAY_CHU);
                _frmAction.getControl("file_is_local").trigger("select2:select");
                break
            case "SENDMAIL":
                anHienDiv("divMail", true);

                _frmAction.getControl("mail_db_id").setValue(DEFAULT_DB);
                _frmAction.getControl("mail_db_id").trigger("select2:select");
                _frmAction.getControl("mail_schema_id").setValue(DEFAULT_SCHEMA);
                _frmAction.getControl("mail_type_exec").setValue("RETURN_NONE");
                _frmAction.getControl("mail_is_attach_file").setValue("0");

                break
            default:
                break;
        }
    });
    _frmAction.getControl("ac_type_cache").addEventChange(val => {
        anHienDiv("divCauHinhAction", false);
        _frmAction.getControl("acf_cache_connection_name").setValue("");
        _frmAction.getControl("acf_cache_password").setValue("");
        _frmAction.getControl("acf_cache_port").setValue("");
        _frmAction.getControl("ac_time_live_cache").setValue("");
        _frmAction.getControl("acf_param_cache").setValue("");
        if (val === "ALLOW_CACHE") {
            anHienDiv("divCauHinhAction", true);
            _frmAction.getControl("acf_cache_connection_name").setValue(DEFAULT_CACHE.CONNECTION_NAME);
            _frmAction.getControl("acf_cache_password").setValue(DEFAULT_CACHE.MAT_KHAU);
            _frmAction.getControl("acf_cache_port").setValue(DEFAULT_CACHE.PORT);
            _frmAction.getControl("ac_time_live_cache").setValue(DEFAULT_CACHE.TIME_LIVE);
            _frmAction.getControl("acf_db_cache").setValue(DEFAULT_CACHE.DEFAULT_DB_CACHE);
            _frmAction.getControl("acf_id_server_cache").setValue(DEFAULT_CACHE.SERVER_CACHE);
            _frmAction.getControl("acf_param_cache").setValue("b_ma_doi_tac_nsd");
        }
    });
    _frmAction.getControl("ac_type_ddos").addEventChange(val => {
        anHienDiv("divDDOS", false);
        if (val === "APPLY") {
            anHienDiv("divDDOS", true);
        }
    });
    _frmAction.getControl("file_is_local").addEventChange(val => {
        anHienDiv("divCauHinhThongTinMayChuFile", false);
        _frmAction.getControl("file_base_folder").setValue(DEFAULT_LOAI_MAY_CHU_FILE.THU_MUC_GOC);
        _frmAction.getControl("file_ip_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.DUONG_DAN_GOC);
        _frmAction.getControl("file_user_remote").setValue("");
        _frmAction.getControl("file_pas_remote").setValue("");
        if (val === "0") {
            anHienDiv("divCauHinhThongTinMayChuFile", true);
            _frmAction.getControl("file_ip_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.IP_MAY_CHU);
            _frmAction.getControl("file_user_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.TAI_KHOAN);
            _frmAction.getControl("file_pas_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.MAT_KHAU);
        }
    });
    _frmAction.getControl("file_type_file").addEventChange(val => {
        anHienDiv("divCauHinhThongTinAnh", false);
        if (val === "ANH") {
            anHienDiv("divCauHinhThongTinAnh", true);
        }
    });
    _frmAction.getControl("mail_is_attach_file").addEventChange(val => {
        anHienDiv("divCauHinhThongTinMayChuFileMail", false);
        if (val === "1") {
            anHienDiv("divCauHinhThongTinMayChuFileMail", true);
            _frmAction.getControl("mail_type_file").setValue(DEFAULT_LOAI_FILE);
            _frmAction.getControl("mail_base_folder").setValue(DEFAULT_LOAI_MAY_CHU_FILE.THU_MUC_GOC);
            _frmAction.getControl("mail_ip_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.DUONG_DAN_GOC);
            _frmAction.getControl("mail_user_remote").setValue("");
            _frmAction.getControl("mail_pas_remote").setValue("");

            _frmAction.getControl("mail_user_remote").readOnly();
            _frmAction.getControl("mail_pas_remote").readOnly();
        }
    });
    _frmAction.getControl("mail_is_local").addEventChange(val => {
        _frmAction.getControl("mail_base_folder").setValue(DEFAULT_LOAI_MAY_CHU_FILE.THU_MUC_GOC);
        _frmAction.getControl("mail_ip_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.DUONG_DAN_GOC);
        _frmAction.getControl("mail_user_remote").setValue("");
        _frmAction.getControl("mail_pas_remote").setValue("");
        _frmAction.getControl("mail_user_remote").readOnly();
        _frmAction.getControl("mail_pas_remote").readOnly();
        if (val === "0") {
            _frmAction.getControl("mail_ip_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.IP_MAY_CHU);
            _frmAction.getControl("mail_user_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.TAI_KHOAN);
            _frmAction.getControl("mail_pas_remote").setValue(DEFAULT_LOAI_MAY_CHU_FILE.MAT_KHAU);
            _frmAction.getControl("mail_user_remote").readOnly(false);
            _frmAction.getControl("mail_pas_remote").readOnly(false);
        }
    });
    _frmAction.getControl("is_ac_send_notify").addEventChange(val => {
        anHienDiv("divCauHinhThongBao", false);
        _frmAction.getControl("ac_send_notify").setValue("");
        $("#notify_NOTIFY").prop("checked", false);
        $("#notify_SMS").prop("checked", false);
        $("#notify_EMAIL").prop("checked", false);
        if (val === "1") {
            anHienDiv("divCauHinhThongBao", true);
        }
    });
    _frmAction.getControl("ac_api_gateway").addEventChange(val => {
        anHienDiv("divCauHinhApiGateWay", false);
        _frmAction.getControl("ac_api_gateway_url").setValue("");
        _frmAction.getControl("ac_api_gateway_callback").setValue("");
        _frmAction.getControl("ac_api_gateway_sync").setValue("DBO");
        if (val == "C") {
            anHienDiv("divCauHinhApiGateWay", true);
        }
    });
});