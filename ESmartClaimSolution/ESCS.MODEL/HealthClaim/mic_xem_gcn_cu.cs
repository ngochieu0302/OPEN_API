using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.HealthClaim
{
    public class mic_xem_gcn_cu
    {
        public List<mic_xem_gcn_hd_cu> hd { get; set; }
        public List<mic_xem_gcn_gcn_cu> gcn { get; set; }
        public List<mic_xem_gcn_dk_cu> dk { get; set; }
        public List<mic_xem_gcn_ls_benh_cu> ls_benh { get; set; }
        public List<mic_xem_gcn_benh_khac_cu> benh_khac { get; set; }
        public List<mic_xem_gcn_dong_bh_cu> dong_bh { get; set; }
        public List<mic_xem_gcn_tai_bh_cu> tai_bh { get; set; }
    }
    public class mic_xem_gcn_hd_cu
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? ngay_ps { get; set; }
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
        public string so_id_hdong_goc { get; set; }
        public decimal? so_id_hdong_dau { get; set; }
        public string dien_thoai_kh { get; set; }
        public string mst_kh { get; set; }
        public string email_kh { get; set; }
        public string so_cmt_kh { get; set; }
        public string loai_kh { get; set; }
    }
    public class mic_xem_gcn_gcn_cu
    {
        public string ma_chi_nhanh { get; set; }
        public string ten_chi_nhanh { get; set; }
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string so_gcn { get; set; }
        public string ten_ndbh { get; set; }
        public string dchi_ndbh { get; set; }
        public string cmt_ndbh { get; set; }
        public string ngay_sinh { get; set; }
        public string gioi_tinh { get; set; }
        public string dien_thoai { get; set; }
        public string email { get; set; }
        public string ngay_hl { get; set; }
        public string ngay_kt { get; set; }
    }

    public class mic_xem_gcn_dk_cu
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string nghiep_vu { get; set; }
        public string loai { get; set; }
        public string loaiq { get; set; }
        public string ten_ql { get; set; }
        public decimal? gioi_han_so_ngay { get; set; }
        public decimal? gioi_han_tien_ngay { get; set; }
        public decimal? ty_le_dong { get; set; }
        public decimal? muc_tn { get; set; }
        public string tgian_cho { get; set; }
        public string phi { get; set; }
        public string logic { get; set; }
    }

    public class mic_xem_gcn_ls_benh_cu
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string ma_benh { get; set; }
        public string ten_benh { get; set; }
        public string ghi_chu { get; set; }
    }
    public class mic_xem_gcn_benh_khac_cu
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public string ten_benh { get; set; }
        public string ghi_chu { get; set; }
    }
    public class mic_xem_gcn_dong_bh_cu
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string kieu_dong_bh { get; set; }
        public string loai_dong_bh { get; set; }
        public string nghiep_vu { get; set; }
        public decimal? tl_dong_bh { get; set; }
    }
    public class mic_xem_gcn_tai_bh_cu
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string kieu_tai_bh { get; set; }
        public string nghiep_vu { get; set; }
        public decimal? tl_tai_bh { get; set; }
    }
}
