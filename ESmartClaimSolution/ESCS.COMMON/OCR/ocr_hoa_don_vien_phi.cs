using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class ocr_hoa_don_vien_phi
    {
        public List<ocr_hoa_don> data { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public db_ocr_hoa_don_vien_phi GetData()
        {
            ocr_hoa_don dataResponse = this.data.FirstOrDefault();
            db_ocr_hoa_don_vien_phi dataDB = new db_ocr_hoa_don_vien_phi();
            Invoice dataOCR = dataResponse.info;
            string tien_str = "", thue_str = "", tong_tien_str = "";
            if (dataOCR.sub_total != null)
                tien_str = dataOCR.sub_total.Replace(",", "").Replace(".", "").Replace(" ", "");
            if (dataOCR.vat_amount != null)
                thue_str = dataOCR.vat_amount.Replace(",", "").Replace(".", "").Replace(" ", "");
            if (dataOCR.total_amount != null)
                tong_tien_str = dataOCR.total_amount.Replace(",", "").Replace(".", "").Replace(" ", "");

            decimal so_tien = 0; decimal thue = 0; decimal tong_tien = 0;
            var ktra_so_tien = Decimal.TryParse(tien_str, out so_tien);
            if (!ktra_so_tien)
                so_tien = 0;
            var ktra_thue = Decimal.TryParse(thue_str, out thue);
            if (!ktra_thue)
                thue = 0;
            var ktra_tong_tien = Decimal.TryParse(tong_tien_str, out tong_tien);
            if (!ktra_tong_tien)
                tong_tien = 0;

            if (so_tien == 0)
                so_tien = tong_tien;

            dataDB.ten_dvi_phat_hanh = dataOCR.supplier;
            dataDB.dchi_dvi_phat_hanh = dataOCR.supplier_address;
            dataDB.mst_dvi_phat_hanh = dataOCR.tax_code;
            dataDB.ngay_ct = dataOCR.date;
            dataDB.mau_hdon = dataOCR.form;
            dataDB.ky_hieu_hdon = dataOCR.serial_no;
            dataDB.so_hdon = dataOCR.invoice_no;
            dataDB.tien = so_tien;
            dataDB.thue = thue;
            dataDB.tl_thue = dataOCR.vat_rate;
            dataDB.website_tra_cuu = dataOCR.lookup_website;
            dataDB.ma_tra_cuu = dataOCR.lookup_code;
            return dataDB;
        }
    }
    public class ocr_hoa_don
    {
        public Invoice info { get; set; }
        public List<int> pages { get; set; }
        public string type { get; set; }
    }
    public class Invoice
    {
        public string buyer_name { get; set; }
        public string date { get; set; }
        public string form { get; set; }
        public string image { get; set; }
        public string image_table { get; set; }
        public string invoice_no { get; set; }
        public string lookup_code { get; set; }
        public string lookup_website { get; set; }
        public string payment_method { get; set; }
        public string purchaser_name { get; set; }
        public string serial_no { get; set; }
        public string sub_total { get; set; }
        public string supplier { get; set; }
        public string supplier_address { get; set; }
        public List<List<InfoHoaDon>> table { get; set; }
        public string tax_code { get; set; }
        public string total_amount { get; set; }
        public string vat_amount { get; set; }
        public string vat_rate { get; set; }
    }
    public class InfoHoaDon
    {
        public string label { get; set; }
        public string value { get; set; }
    }
    public class db_ocr_hoa_don_vien_phi
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }
        public decimal? so_id { get; set; }
        public decimal? bt { get; set; }
        public string dvi_ph { get; set; }
        public string ten_dvi_phat_hanh { get; set; }
        public string dchi_dvi_phat_hanh { get; set; }
        public string mst_dvi_phat_hanh { get; set; }
        public string ngay_ct { get; set; }
        public string mau_hdon { get; set; }
        public string ky_hieu_hdon { get; set; }
        public string so_hdon { get; set; }
        public decimal? tien { get; set; }
        public decimal? thue { get; set; }
        public string tl_thue { get; set; }
        public string ten_dvi_nhan { get; set; }
        public string mst_dvi_nhan { get; set; }
        public string dchi_dvi_nhan { get; set; }
        public string website_tra_cuu { get; set; }
        public string ma_tra_cuu { get; set; }
    }
}
