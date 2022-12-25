using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class fpt_ocr_gplx
    {
        public decimal? errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<data_fpt_ocr_gplx> data { get; set; }
        public db_fpt_ocr_gplx GetData()
        {
            if (this.data == null || this.data.Count <= 0)
                return null;
            data_fpt_ocr_gplx dataOCR = data.FirstOrDefault();
            db_fpt_ocr_gplx dataDB = new db_fpt_ocr_gplx();
            dataDB.hang = dataOCR.@class;
            dataDB.ngay_cap = dataOCR.date;
            dataDB.ngay_het_hieu_luc = dataOCR.doe;
            dataDB.ngay_sinh = dataOCR.dob;
            dataDB.noi_cu_tru = dataOCR.address;
            dataDB.quoc_tich = dataOCR.nation;
            dataDB.nhan_hieu = dataOCR.address;
            dataDB.so = dataOCR.id;
            dataDB.so_cho_ngoi = "";
            dataDB.ten_nguoi = dataOCR.name;
            if (!string.IsNullOrEmpty(dataDB.so) && !string.IsNullOrEmpty(dataDB.ten_nguoi))
                dataDB.loai_vb = "giay_phep_lai_xe_mat_truoc";
            else
                dataDB.loai_vb = "";
            return dataDB;
        }
    }
    public class data_fpt_ocr_gplx
    {
        public string id { get; set; }
        public string id_prob { get; set; }
        public string name { get; set; }
        public string name_prob { get; set; }
        public string dob { get; set; }
        public string dob_prob { get; set; }
        public string nation { get; set; }
        public string nation_prob { get; set; }
        public string address { get; set; }
        public string address_prob { get; set; }
        public string place_issue { get; set; }
        public string place_issue_prob { get; set; }
        public string date { get; set; }
        public string date_prob { get; set; }
        public string @class { get; set; }
        public string class_prob { get; set; }
        public string doe { get; set; }
        public string doe_prob { get; set; }
        public string overall_score { get; set; }
        public string type { get; set; }
    }
    public class db_fpt_ocr_gplx
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        public string ung_dung { get; set; }
        public string so_id { get; set; }
        public string hang { get; set; }
        public string loai_vb { get; set; }
        public string ngay_cap { get; set; }
        public string ngay_het_hieu_luc { get; set; }
        public string ngay_sinh { get; set; }
        public string noi_cu_tru { get; set; }
        public string quoc_tich { get; set; }
        public string nhan_hieu { get; set; }
        public string so { get; set; }
        public string so_cho_ngoi { get; set; }
        public string ten_nguoi { get; set; }
    }
}
