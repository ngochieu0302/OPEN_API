using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Excels
{
    public class MICThanhToan
    {
        public double? sott { get; set; }
        public string ten_cnhanh { get; set; }
        public string so_hd { get; set; }
        public string ngay_hd { get; set; }
        public string ten { get; set; }
        public string so_hs { get; set; }
        public double? so_tien_chua_vat { get; set; }
        public double? vat { get; set; }
        public double? tien_theo_hd { get; set; }
        public double? tien_kh_tra { get; set; }
        public string tk_cmt { get; set; }
        public string ten_ngan_hang { get; set; }
        public string dien_giai { get; set; }
        public string dthoai_kh { get; set; }
        public string gdv { get; set; }
        public string mau_hdon { get; set; }
        public string ky_hieu_hdon { get; set; }
        public string mst { get; set; }
    }
    public enum MICThanhToanEnum
    {
        NONE,
        SOTT,
        SO_HD,
        NGAY_HD,
        TEN,
        SO_HS,
        SO_TIEN_CHUA_VAT,
        VAT,
        TIEN_THEO_HD,
        TIEN_KH_TRA,
        TK_CMT,
        TEN_NGAN_HANG,
        DIEN_GIAI,
        DTHOAI_KH,
        GDV,
        MAU_HDON,
        KY_HIEU_HDON,
        MST,
    }
}
