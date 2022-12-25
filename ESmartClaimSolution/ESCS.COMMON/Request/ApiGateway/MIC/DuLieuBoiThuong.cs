using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class DuLieuBoiThuong
    {
        public string ma_dvi { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }
        public HoSo hs { get; set; }
        public List<NghiepVu> nv { get; set; }
        

    }
    public class DuLieuBoiThuongOpes
    {
        public List<HoSoOPES> hs { get; set; }
        public List<VuTT> vu_tt { get; set; }
        public List<NghiepVu> nv { get; set; }
        public List<ThuHuong> thu_huong { get; set; }
        public List<ChiPhikhac> chi_phi_khac { get; set; }
        public List<TamUng> tam_ung { get; set; }
        public List<ThanhToan> thanh_toan { get; set; }

    }
    public class VuTT 
    {
        public string ngay_ton_that { get; set; }
        public string dia_diem_xr { get; set; }
        public string nguyen_nhan { get; set; }
        public string hau_qua { get; set; }
    }
    public class HoSo
    {
        public string ma_chi_nhanh { get; set; }
        public long? so_id_hs { get; set; }
        public long? ngay_ps { get; set; }
        public string so_hs { get; set; }
        public string ma_dtac { get; set; }
        public string ma_chi_nhanh_cap { get; set; }
        public string so_hdong { get; set; }
        public long? so_id_gcn { get; set; }
        public string so_cv_kn { get; set; }
        public string ngay_ton_that { get; set; }
        public string ngay_tbao { get; set; }
        public string ngay_duyet { get; set; }

        public string so_tn { get; set; }
        public string so_hd { get; set; }
        public string ma_trang_thai { get; set; }
        public string ten_trang_thai { get; set; }
        public decimal? tien_bt { get; set; }
        public string ngay_cap_nhat { get; set; }
        public string ngay_mo_hs { get; set; }
        public string ngay_yc_bshs { get; set; }
        public string ngay_bshs { get; set; }
        public string ngay_trinh { get; set; }
        public string ngay_tchoi { get; set; }
        public string dia_diem_xr { get; set; }
        public string nguyen_nhan { get; set; }
        public string hau_qua { get; set; }
        public string nguoi_lhe { get; set; }
        public string dthoai_lhe { get; set; }
        public string email_lhe { get; set; }
        public string moi_qh_lhe { get; set; }
        public string moi_qh_lhe_ten { get; set; }
    }
    public class HoSoOPES
    {
        public string ma_chi_nhanh { get; set; }
        public long? so_id_hs { get; set; }
        public string ngay_ps { get; set; }
        public string so_hs { get; set; }
        public string ma_chi_nhanh_cap { get; set; }
        public string so_hd { get; set; }
        public string so_hdong { get; set; }
        public string ngay_mo_hs { get; set; }
        public string ngay_trinh { get; set; }
        public string ngay_tchoi { get; set; }
        public string ngay_yc_bshs { get; set; }
        public string ngay_bshs { get; set; }
        public string ngay_cap_nhat { get; set; }
        public long? so_id_hd { get; set; }
        public long? so_id_gcn { get; set; }
        public string so_cv_kn { get; set; }
        public string nguoi_lhe { get; set; }
        public string dthoai_lhe { get; set; }
        public string email_lhe { get; set; }
        public string moi_qh_lhe { get; set; }
        public string moi_qh_lhe_ten { get; set; }
        public string ngay_tbao { get; set; }
        public string ngay_duyet { get; set; }
        public string ma_trang_thai { get; set; }
        public string ten_trang_thai { get; set; }
    }
    public class NghiepVu
    {
        public string nghiep_vu { get; set; }
        public string ma_tien_bh { get; set; }
        public decimal? uoc_bt { get; set; }
        public decimal? so_tien_yc { get; set; }
        public decimal? so_tien_bh { get; set; }
        public decimal? so_tien_tt { get; set; }
        public decimal? khau_tru { get; set; }
        public decimal? so_tien_bt { get; set; }
        public decimal? so_tien_bt_vnd { get; set; }
        public decimal? thue_bt { get; set; }
        public decimal? thue_bt_vnd { get; set; }
        public string nguyen_nhan { get; set; }
    }
    public class ChiPhikhac
    {
        public string nghiep_vu { get; set; }
    }
    public class ThuHuong
    {
        public decimal? stt { get; set; }
        public string ten_tk { get; set; }
        public string so_tk { get; set; }
        public string ma_ngan_hang { get; set; }
        public string ma_chi_nhanh { get; set; }
        public string ten_ngan_hang { get; set; }
        public string ten_chi_nhanh { get; set; }
        public decimal? so_tien { get; set; }
    }
    public class TamUng
    {
        public decimal? stt { get; set; }
        public decimal? so_tien { get; set; }
        public string ngay_duyet { get; set; }
    }
    public class ThanhToan
    {
        public decimal? stt { get; set; }
        public decimal? so_tien { get; set; }
        public string ngay_duyet { get; set; }
    }
}
