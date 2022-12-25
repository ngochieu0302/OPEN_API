using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.MAU_IN
{
    public class DoiTuongXeDB
    {
        public decimal? so_id_doi_tuong { get; set; }
        public string ten_doi_tuong { get; set; }
        public string nhom { get; set; }
        public string loai { get; set; }
        public string ten_hm { get; set; }
        public decimal? tien_bgia { get; set; }

        public decimal? tien_vtu_dx { get; set; }
        public decimal? tien_nhan_cong_dx { get; set; }
        public decimal? tien_khac_dx { get; set; }

        public decimal? tien_dx { get; set; }
        public decimal? pt_khau_hao { get; set; }
        public decimal? tien_khau_hao { get; set; }
        public decimal? pt_bao_hiem { get; set; }
        public decimal? tien_bao_hiem { get; set; }
        public decimal? pt_giam_tru { get; set; }
        public decimal? tien_giam_tru { get; set; }
        public decimal? tl_giam_gia_vtu { get; set; }
        public decimal? tien_giam_gia_vtu { get; set; }
        public decimal? tl_giam_gia_nhan_cong { get; set; }
        public decimal? tien_giam_gia_nhan_cong { get; set; }
        public decimal? tl_giam_gia_khac { get; set; }
        public decimal? tien_giam_gia_khac { get; set; }

        public decimal? tl_tien_ktru_tien_bh { get; set; }
        public decimal? tien_ktru_tien_bh { get; set; }
        public decimal? tien_mien_thuong { get; set; }
        public decimal? tien_thue { get; set; }
    }
}
