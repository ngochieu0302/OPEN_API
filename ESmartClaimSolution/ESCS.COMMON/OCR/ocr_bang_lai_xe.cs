using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class ocr_bang_lai_xe
    {
        public List<data_ocr_bang_lai_xe> data { get; set; }
        public decimal? errorCode { get; set; }
        public string errorMessage { get; set; }
        public db_ocr_bang_lai_xe GetData()
        {
            data_ocr_bang_lai_xe dataResponse = this.data.FirstOrDefault();
            db_ocr_bang_lai_xe dataDB = new db_ocr_bang_lai_xe();
            data_bang_lai_xe dataOCR = dataResponse.info;
            dataDB.so = dataOCR.id;
            dataDB.hang = dataOCR.@class;
            dataDB.ngay_cap = dataOCR.issue_date;
            dataDB.ngay_het_hieu_luc = dataOCR.due_date;
            dataDB.ngay_sinh = dataOCR.dob;
            dataDB.noi_cu_tru = dataOCR.address;
            dataDB.quoc_tich = dataOCR.nationality;
            dataDB.ten_nguoi = dataOCR.name;
            dataDB.loai_vb = dataResponse.type == "driving_license" ? "giay_phep_lai_xe_mat_sau" : "";
            return dataDB;
        }
    }
    public class data_ocr_bang_lai_xe
    {
        public data_bang_lai_xe info;
        public string type;
    }
    public class data_bang_lai_xe
    {
        public string id { get; set; }
        public string name { get; set; }
        public string dob { get; set; }
        public string @class { get; set; }
        public string nationality { get; set; }
        public string issue_date { get; set; }
        public string due_date { get; set; }
        public string address { get; set; }
        public string address_district { get; set; }
        public string address_district_code { get; set; }
        public string address_town { get; set; }
        public string address_town_code { get; set; }
        public string address_ward { get; set; }
        public string address_ward_code { get; set; }
        public string image { get; set; }
    }
    public class db_ocr_bang_lai_xe
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        public string ma_doi_tac { get; set; }
        public string ung_dung { get; set; }
        public decimal? so_id { get; set; }
        public decimal? so_id_doi_tuong { get; set; }
        public decimal? bt { get; set; }
        public string hang { get; set; }
        public string loai_vb { get; set; }
        public string ngay_cap { get; set; }
        public string ngay_het_hieu_luc { get; set; }
        public string ngay_sinh { get; set; }
        public string noi_cu_tru { get; set; }
        public string quoc_tich { get; set; }
        public string so { get; set; }
        public string ten_nguoi { get; set; }
    }
}
