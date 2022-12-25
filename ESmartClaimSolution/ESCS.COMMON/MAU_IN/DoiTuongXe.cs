using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ESCS.MODEL.MAU_IN
{
    public class DoiTuongXe
    {
        public string so_id_doi_tuong { get; set; }
        public string ten_dt { get; set; }
        public string pt_khao { get; set; }
        public string tien_khao { get; set; }
        public string pt_bhiem { get; set; }
        public string tien_bhiem { get; set; }
        public string pt_gtru { get; set; }
        public string tien_gtru { get; set; }
        public string tl_ggia_vtu { get; set; }
        public string tien_ggia_vtu { get; set; }
        public string tl_ggia_ncong { get; set; }
        public string tien_ggia_ncong { get; set; }
        public string tl_ggia_khac { get; set; }
        public string tien_ggia_khac { get; set; }
        public string tl_tien_ktru_bh { get; set; }
        public string tien_ktru_bh { get; set; }
        public string tien_mien_thuong { get; set; }

        public string tong_tt { get; set; }
        public string tong_sc { get; set; }
        public string tong_son { get; set; }

        public string cp_cau { get; set; }
        public string cp_keo { get; set; }
        public string cp_khac { get; set; }

        public string tong_dx { get; set; }
        public string tong_thue { get; set; }
        public string tong_cong { get; set; }
        public List<HangMuc> ThayThe { get; set; }
        public List<HangMuc> SuaChua { get; set; }
        public List<HangMuc> Son { get; set; }
        
    }
    public class HangMuc
    {
        public int index { get; set; }
        public string stt { get; set; }
        public string ten_hm { get; set; }
        public string tien_bgia { get; set; }
        public string tien_dx { get; set; }
        public string pt_khao { get; set; }
        public string ghi_chu { get; set; }
    }
    public class ChiPhiKhacDB
    {
        public int index { get; set; }
        public string stt { get; set; }
        public string ten { get; set; }
        public string ma_chi_phi { get; set; }
        public decimal? tien_dx { get; set; }
        public decimal? tien_bao_hiem { get; set; }
        public decimal? tien_giam_tru { get; set; }
        public decimal? tien_thue { get; set; }
        public decimal? so_id_doi_tuong { get; set; }
    }
    public class ChiPhiKhac
    {
        public int index { get; set; }
        public string stt { get; set; }
        public string ten { get; set; }
        public string tien_dx { get; set; }
        public string tien_bao_hiem { get; set; }
        public string tien_giam_tru { get; set; }
        public string tien_thue { get; set; }
        public string so_id_doi_tuong { get; set; }
    }
}
