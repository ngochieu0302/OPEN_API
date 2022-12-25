using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class ThongTinEmail<T>
    {
        public string key { get; set; }
        public NguoiNhan nguoi_nhan { get; set; }
        public T mail { get; set; }
        public List<File> file { get; set; }
        public ServerMail server_mail { get; set; }
        public string template { get; set; }
        public List<BenhVien> data1 { get; set; }
        public List<SuKienBaoHiem> data2 { get; set; }
    }
    public class NguoiNhan
    {
        public string email { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string tieu_de { get; set; }
    }
    public class File
    {
        public string ma_doi_tac { get; set; }
        public decimal? so_id { get; set; }
        public decimal? bt { get; set; }
        public string ma_file { get; set; }
        public string url_file { get; set; }
        public string ten_file { get; set; }
        public string file_base_64 { get; set; }
    }
    public class ServerMail
    {
        public string ma_doi_tac { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string smtp_server { get; set; }
        public string smtp_tai_khoan { get; set; }
        public string smtp_mat_khau { get; set; }
        public string smtp_port { get; set; }
        public string dung_proxy { get; set; }
        public string proxy_server { get; set; }
        public string proxy_tai_khoan { get; set; }
        public string proxy_mat_khau { get; set; }
        public string ap_dung { get; set; }
        public string ten_hthi { get; set; }
    }
    public class ThongBaoGiamDinh
    {
        public string ma_doi_tac { get; set; }
        public string ma_chi_nhanh { get; set; }
        public string ngay_ht { get; set; }
        public string ngay_thanh_toan { get; set; }
        public string so_id { get; set; }
        public string nv { get; set; }
        public string so_hs { get; set; }
        public string so_id_hd { get; set; }
        public string so_id_dt { get; set; }
        public string so_hd { get; set; }
        public string doi_tuong { get; set; }
        public string gcn { get; set; }
        public string nguon_tb { get; set; }
        public string nguon_tb_ten { get; set; }
        public string ngay_tb { get; set; }
        public string ngay_tn_hs_moi { get; set; }
        public string nguoi_tb { get; set; }
        public string moi_qh_tb { get; set; }
        public string moi_qh_tb_ten { get; set; }
        public string dthoai_tb { get; set; }
        public string email_tb { get; set; }
        public string nguoi_lhe { get; set; }
        public string moi_qh_lhe { get; set; }
        public string moi_qh_lhe_ten { get; set; }
        public string dthoai_lhe { get; set; }
        public string email_lhe { get; set; }
        public string email_chu_xe { get; set; }
        public string so_tien { get; set; }
        public string tien_de_xuat_duyet_gia { get; set; }
        public string gara { get; set; }
        public string kien_nghi { get; set; }
        public string y_kien { get; set; }
        public string nsd { get; set; }
        public string ten_gdvtt { get; set; }
        public string chu_xe { get; set; }
        public string so_khung { get; set; }
        public string so_may { get; set; }
        public string hang_xe { get; set; }
        public string hieu_xe { get; set; }
        public string dien_thoai { get; set; }
        public string dthoai_chu_xe { get; set; }
        public string dthoai_kh { get; set; }
        public string email { get; set; }
        public string chu_xe_dchi { get; set; }
        public string ten_dvi_cap { get; set; }
        public string trong_tai { get; set; }
        public string so_cho { get; set; }
        public string nam_sx { get; set; }
        public string dchi { get; set; }
        public string hieu_luc { get; set; }
        public string trang_thai { get; set; }
        public string ma_trang_thai { get; set; }
        public string ngay_gd { get; set; }
        public string dia_diem { get; set; }
        public string dia_diem_vu_tt { get; set; }
        public string thong_tin_gdv { get; set; }
        public string ten_gdv { get; set; }
        public string dthoai_gdv { get; set; }
        public string thong_tin_kh { get; set; }
        public string thong_tin_lhe { get; set; }
        public string url_xac_nhan { get; set; }
        public string ten_nguoi_duyet_chinh_pasc { get; set; }
        public string ten_nguoi_duyet_chinh_bt { get; set; }
        public string dien_thoai_nguoi_trinh { get; set; }
        public string ten_nguoi_trinh_pasc { get; set; }
        //Thông tin công ty
        public string yeu_cau_bs { get; set; }
        public string ten_cb { get; set; }
        public string dthoai_cb { get; set; }
        public string email_cb { get; set; }
        public string ten_cty { get; set; }

        public string ten_cty_tat { get; set; }
        public string sdt_cty { get; set; }
        public string dia_chi_cty { get; set; }
        public string website_cty { get; set; }
        public string dthoai_cty { get; set; }
        public string logo_email { get; set; }
        public string fax { get; set; }
        public string sdt_tong_dai { get; set; }
        public string slogan { get; set; }
        public string email_cty { get; set; }
        //Kết thúc
        public string ngay_xr { get; set; }
        public string nguyen_nhan { get; set; }
        public string tien_giam_dinh { get; set; }
        public string tien_duyet_gia { get; set; }
        public string tien_bao_lanh { get; set; }
        public string tien_duyet { get; set; }
        public string so_tien_bt { get; set; }
        public string tien_gara_bao_gia { get; set; }
        public string tien_kh_chua_vat { get; set; }
        public string so_ct { get; set; }
        public string thue_duyet { get; set; }
        public string thue_de_xuat_duyet { get; set; }
        public string tong_tien { get; set; }
        public string tong_tien_de_xuat { get; set; }
        public string tien_dx_da_vat { get; set; }
        public string thong_tin_btv { get; set; }
        public string ten_btv { get; set; }
        public string email_btv { get; set; }
        public string dien_thoai_btv { get; set; }

        public string trang_thai_hs { get; set; }
        public string ma_trang_thai_hs { get; set; }
        public string dia_diem_gd { get; set; }
        public string dia_diem_xr { get; set; }
        public string loai_trinh_y_kien { get; set; }
        public string domain { get; set; }
        public string ngay_hoan_thanh { get; set; }

        // Health care
        public string ma_cv { get; set; }
        public string ten_lhnv { get; set; }
        public string ten_ndbh { get; set; }
        public string ten_cong_ty { get; set; }
        public string bsct { get; set; }
        public string giay_to { get; set; }
        public string hsbg { get; set; }
        public string ten_cong_ty_tat { get; set; }
        public string ten_cnhanh_cap { get; set; }
        public string d_chi_chi_nhanh { get; set; }
        public string d_thoai_chi_nhanh { get; set; }
        public string email_chi_nhanh { get; set; }
        public string dthoai_nsd { get; set; }
        public string nguoi_lh { get; set; }
        public string ten_bv { get; set; }
        public string ly_do { get; set; }
        public string ten_khach { get; set; }
        public string ten_nguoi_tb { get; set; }
        public string ten_kh { get; set; }
        public string ten_ben_mua_bh { get; set; }
        public string ten_nguoi_duoc_bh { get; set; }
        public string ten_nguoi_yc_tra_tien_bh { get; set; }
        public string tong_so_tien_yc_bh { get; set; }
        public string so_tien_yc { get; set; }
        public string so_tien_duyet { get; set; }
        public string so_tk_bv { get; set; }
        public string ngan_hang { get; set; }
        public string ttin_hs { get; set; }
        public string otp { get; set; }
        public string short_link { get; set; }
        public string short_link_otp { get; set; }
        public string ngay_vv { get; set; }
        public string ngay_rv { get; set; }
        public string gio_vv { get; set; }
        public string gio_rv { get; set; }
        public string tien_giam { get; set; }
        public string ghi_chu { get; set; }
        public string tien_yc { get; set; }
        public string tien_duyet_yc { get; set; }
        public string nguoi_huong_thu { get; set; }
        public string stk_ngan_hang { get; set; }
        public string ngan_hang_nth { get; set; }
        public string tai_lieu_bo_sung { get; set; }
        public string dchi_dt { get; set; }
        public string d_thoai_dt { get; set; }
        public string email_dt { get; set; }
        public string chan_doan { get; set; }
        public string cmt { get; set; }
        public string ma_doi_tac_ql { get; set; }
        public string ten_goi_bh { get; set; }
        public string ten_nguyen_nhan { get; set; }
        public string ten_hinh_thuc { get; set; }
        public string ten_doi_tac_ql { get; set; }
        public string ten_ql { get; set; }
        public string hinh_thuc_tt { get; set; }
    }
    public class BenhVien
    {
        public string ten_bv { get; set; }
        public string ngay_vv { get; set; }
        public string ngay_rv { get; set; }
        public string tien_yc { get; set; }
        public decimal? tien_yc_so { get; set; }
    }
    public class SuKienBaoHiem
    {
        public string ten_nguyen_nhan { get; set; }
        public string ten_bv { get; set; }
        public string ngay_vv { get; set; }
        public string ngay_rv { get; set; }
        public string chan_doan { get; set; }
        public string tien_yc { get; set; }
        public string ten_ql { get; set; }
        public string tien_nam_ql { get; set; }
        public string tien_ngay_ql { get; set; }
        public string tien_duyet { get; set; }
        public string ngay_lan { get; set; }
        public string tien_giam { get; set; }
        public string tong_tien_giam { get; set; }
        public string nguyen_nhan_giam { get; set; }
        public string ghi_chu_qloi { get; set; }
        public string ghi_chu { get; set; }
    }
}
