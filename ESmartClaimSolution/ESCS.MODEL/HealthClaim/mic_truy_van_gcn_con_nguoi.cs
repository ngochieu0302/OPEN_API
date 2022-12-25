using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.HealthClaim
{
    public class mic_truy_van_gcn_con_nguoi
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public List<mic_gcn_con_nguoi> hd { get; set; }

        public List<escs_gcn_con_nguoi> ConvertData()
        {
            var data = new List<escs_gcn_con_nguoi>();
            if (this.hd == null || this.hd.Count <= 0)
                return data;
            foreach (var item in this.hd)
            {
                escs_gcn_con_nguoi escs_item = new escs_gcn_con_nguoi();
                escs_item.ma_doi_tac = this.ma_doi_tac_nsd;
                escs_item.ma_chi_nhanh = this.ma_chi_nhanh_nsd;
                escs_item.ma_doi_tac_ql = this.ma_doi_tac_nsd;
                escs_item.ma_chi_nhanh_ql = item.ma_chi_nhanh;
                escs_item.ten = item.ten_ndbh;
                escs_item.so_cmt = item.so_cmt;
                escs_item.ngay_sinh = "";
                escs_item.ngay_hl = item.ngay_hl_bh + " - " + item.ngay_kt_bh;
                escs_item.ngay_hl_text = item.ngay_hl_bh;
                escs_item.ten_khach = item.ten_kh;
                escs_item.so_hd = item.so_hdong;
                escs_item.so_id_hd = item.so_id_hdong;
                escs_item.so_id_dt = item.so_id_gcn;
                escs_item.d_thoai = item.dien_thoai;
                escs_item.email = item.email;
                escs_item.gcn = item.so_gcn;
                escs_item.ma_goi_bh = "";
                escs_item.ten_dvi_cap_don = item.ten_chi_nhanh;
                escs_item.lhnv = item.ctrinh;
                escs_item.nhom_sp = item.ctrinh;
                escs_item.hd_cu = item.loai_hd;
                escs_item.hd_cu_text = item.loai_hd == "C" ? "Cũ" : "Mới";
                escs_item.goi_bh = "";
                data.Add(escs_item);
            }
            return data;
        }
    }
    public class mic_gcn_con_nguoi
    {
        public string ma_chi_nhanh { get; set; }
        public string ten_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string so_gcn { get; set; }
        public string so_hdong { get; set; }
        public string ten_kh { get; set; }
        public string ten_ndbh { get; set; }
        public string so_cmt { get; set; }
        public string dien_thoai { get; set; }
        public string email { get; set; }
        public string ngay_hl_bh { get; set; }
        public string ngay_kt_bh { get; set; }
        public string ctrinh { get; set; }
        public string loai_hd { get; set; }
    }
    public class escs_gcn_con_nguoi
    {
        public string ma_doi_tac { get; set; }
        public string ma_chi_nhanh { get; set; }
        public string ma_doi_tac_ql { get; set; }
        public string ma_chi_nhanh_ql { get; set; }
        public string ten { get; set; }
        public string so_cmt { get; set; }
        public string ngay_sinh { get; set; }
        public string ngay_hl { get; set; }
        public string ngay_hl_text { get; set; }
        public string ten_khach { get; set; }
        public string so_hd { get; set; }
        public decimal? so_id_hd { get; set; }
        public decimal? so_id_dt { get; set; }
        public string d_thoai { get; set; }
        public string email { get; set; }
        public string gcn { get; set; }
        public string ma_goi_bh { get; set; }
        public string ten_dvi_cap_don { get; set; }
        public string lhnv { get; set; }
        public string nhom_sp { get; set; }
        public string goi_bh { get; set; }
        public string hd_cu { get; set; }
        public string hd_cu_text { get; set; }
    }
}
