using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class KQThongTinGCN
    {
        public List<HopDong> hd { get; set; }
        public List<GiayChungNhan> gcn { get; set; }
        public List<DieuKhoan> dk { get; set; }
        public List<DongBH> dong_bh { get; set; }
        public List<TaiBH> tai_bh { get; set; }
        public List<DKBSNghiepVu> dkbs { get; set; }
    }
    public class HopDong
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public string ngay_ps { get; set; }
        public string ma_nhan_vien { get; set; }
        public string phong_kd { get; set; }
        public string so_hdong { get; set; }
        public string loai_hd { get; set; }
        public string so_hdong_goc { get; set; }
        public string ma_kh { get; set; }
        public string ten_kh { get; set; }
        public string dchi_kh { get; set; }
        public string gio_hl_hd { get; set; }
        public string ngay_hl_hd { get; set; }
        public string gio_kt_hd { get; set; }
        public string ngay_kt_hd { get; set; }
        public string ngay_cap_hd { get; set; }
        public decimal? so_id_hdong_goc { get; set; }
        public decimal? so_id_hdong_dau { get; set; }
        public string dien_thoai_kh { get; set; }
        public string email_kh { get; set; }
        public string so_cmt_kh { get; set; }
        public string mst_kh { get; set; }
        public string loai_kh { get; set; }
        public string ma_gt { get; set; }
        public string ten_gt { get; set; }
    }
    public class GiayChungNhan
    {
        public string ma_chi_nhanh { get; set; }
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string so_gcn { get; set; }
        public string ten_chu_xe { get; set; }
        public string dchi_chu_xe { get; set; }
        public string md_su_dung { get; set; }
        public string loai_xe { get; set; }
        public string ten_loai_xe { get; set; }
        public string bien_so_xe { get; set; }
        public string so_khung { get; set; }
        public string so_may { get; set; }
        public string hang_xe { get; set; }
        public string hieu_xe { get; set; }
        public string trong_tai { get; set; }
        public string so_cho { get; set; }
        public string so_nguoi_tgbh { get; set; }
        public string so_lphu_xe { get; set; }
        public string nam_sx { get; set; }
        public string gia_tri_xe { get; set; }
        public string stbh_tnds_tn { get; set; }
        public string loai_bh { get; set; }
        public string gio_hl_gcn { get; set; }
        public string ngay_hl_gcn { get; set; }
        public string gio_kt_gcn { get; set; }
        public string ngay_kt_gcn { get; set; }
        public string ngay_cap_gcn { get; set; }
        public string nhom_xe { get; set; }
    }
    public class DieuKhoan
    {
        public string ma_chi_nhanh { get; set; }
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string nghiep_vu { get; set; }
        public string so_tien_bh { get; set; }
        public string so_tien_bh_toi_da { get; set; }
        public string tl_phi { get; set; }
        public string phi { get; set; }
        public string thue { get; set; }
        public string tien_mt { get; set; }
        public string loai_mt { get; set; }
        public string loai_hh { get; set; }
        public string ma_dkbs { get; set; }
    }
    public class DongBH
    {
        public string ma_chi_nhanh { get; set; }
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string kieu_dong_bh { get; set; }
        public string loai_dong_bh { get; set; }
        public string nghiep_vu { get; set; }
        public string tl_dong_bh { get; set; }
    }
    public class TaiBH
    {
        public string ma_chi_nhanh { get; set; }
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string kieu_tai_bh { get; set; }
        public string nghiep_vu { get; set; }
        public string tl_tai_bh { get; set; }
    }
    public class DKBSNghiepVu
    {
        public string ma_chi_nhanh { get; set; }
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string nghiep_vu { get; set; }
        public string ma_dkbs { get; set; }
        public string ten_dkbs { get; set; }
        public string muc_ktru { get; set; }
        public decimal? so_tien_ktru { get; set; }
    }
}
