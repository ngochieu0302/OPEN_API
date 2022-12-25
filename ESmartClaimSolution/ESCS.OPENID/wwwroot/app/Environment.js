var _service = new Service();
function luuThongTin(obj) {

}
function xoaThongTin(obj) {

}
function timKiemPhanTrang(obj) {
    _service.postData("/Environment/GetPaging", obj).then(function (res) {
        console.log(res);
    });
}
$(document).ready(function () {
    //timKiemPhanTrang({ tk_ma: "dcscds", ten: "csdcsc", ds: [{ ma: "SV001", ten: "Nguyễn Văn A" }, { ma: "SV002", ten: "Nguyễn Văn B" }] });
    //timKiemPhanTrang({});
    _service.postData("/Environment/GetAll", { tk_ma: "csdc"}).then(function (res) {
        console.log("GetAll");
        console.log(res);
    });
    //_service.postData("/Environment/GetPaging", { tk_ma: "csdc"}).then(function (res) {
    //    console.log("GetPaging");
    //    console.log(res);
    //});
});