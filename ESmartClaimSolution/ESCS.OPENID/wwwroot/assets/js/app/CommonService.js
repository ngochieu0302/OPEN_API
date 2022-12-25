//create by: thanhnx.vbi
function CommonService() {
    this.danhMucChung = {
        noi_sua_chua: [
            { ma: "K", ten: "Sửa chữa ngoài" },
            { ma: "C", ten: "Chính hãng" }
        ],
        phuong_an_sua_chua : [
            { ma: "S", ten: "Sửa chữa" },
            { ma: "T", ten: "Thay thế" }
        ],
        thu_hoi_vat_tu: [
            { ma: "C", ten: "Có" },
            { ma: "K", ten: "Không" }
        ],
        nguon_tb: [
            { ma: "CTCT", ten: "Tổng đài" },
            { ma: "MOBILE", ten: "App mobile" },
            { ma: "TTGD", ten: "Trực tiếp" }
        ],
        nhom_tai_lieu: [
            { ma: "TT", ten: "Tổn thất" },
            { ma: "TL", ten: "Giấy tờ, tài liệu" }
        ],
        khau_tru: [
            { ma: "C", ten: "Có" },
            { ma: "K", ten: "Không" }
        ],
        tl_thue: [
            { ma: "10", ten: "10" },
            { ma: "0", ten: "0" }
        ]
    };
    // Lấy danh sách đơn vị hành chính
    this.layTatCaDonViHanhChinh = function (obj = {}) {
        var _service = new Service();
        return _service.postData("/common/getadministrativeunits", obj);
    };
    // Lấy control ẩn hiển của từng màn hình
    this.layControl = function (obj = {}) {
        var _service = new Service();
        return _service.postData("/common/getcontrol", obj);
    };
}
