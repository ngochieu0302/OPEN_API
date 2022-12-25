using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.HealthClaim
{
    public class digins_xem_gcn
    {
        public List<digins_xem_gcn_hd> hd { get; set; }
        public List<digins_xem_gcn_gcn> gcn { get; set; }
        public List<digins_xem_gcn_dk> dk { get; set; }
        public List<digins_xem_gcn_dkbs> dkbs { get; set; }
    }
    public class digins_xem_gcn_hd
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
        public string so_id_hdong_goc { get; set; }
        public decimal? so_id_hdong_dau { get; set; }
        public string dien_thoai_kh { get; set; }
        public string mst_kh { get; set; }
        public string email_kh { get; set; }
        public string so_cmt_kh { get; set; }
        public string loai_kh { get; set; }
    }
    public class digins_xem_gcn_gcn
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
        public string ctrinh { get; set; }
        public string goi_bh { get; set; }
        public string ten_goi_bh { get; set; }
        public string ngay_hl { get; set; }
        public string ngay_kt { get; set; }
        public string nhom { get; set; }
    }
    public class digins_xem_gcn_dk
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string ma_qloi { get; set; }
        public string ma_qloi_ct { get; set; }
        public string ten_ql { get; set; }
        public decimal? gioi_han_so_ngay { get; set; }
        public decimal? gioi_han_tien_ngay { get; set; }
        public decimal? ty_le_dong { get; set; }
        public decimal? muc_tn { get; set; }
        public string tgian_cho { get; set; }
        public string phi { get; set; }
    }
    public class digins_xem_gcn_dkbs
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string ma_qloi { get; set; }
        public string ten { get; set; }
        public decimal? gioi_han_so_ngay { get; set; }
        public decimal? gioi_han_tien_ngay { get; set; }
        public decimal? muc_tn { get; set; }
        public decimal? tgian_cho { get; set; }
        public decimal? phi { get; set; }
        public string loai { get; set; }
    }
}
