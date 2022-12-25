using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ESCS.COMMON.OCR
{
    public class ocr_dang_ky_xe
    {
        public List<data_ocr_dang_ky_xe> data { get; set; }
        public decimal? errorCode { get; set; }
        public string errorMessage { get; set; }
        public db_ocr_dang_ky_xe GetData()
        {
            List<data_ocr_dang_ky_xe> dataResponse = this.data;
            db_ocr_dang_ky_xe dataDB = new db_ocr_dang_ky_xe();

            foreach (var item in dataResponse)
            {
                if (item.type == "vehicle_registration_back")
                {
                    data_dang_ky_xe dataOCR = item.info;
                    dataDB.bien_so_xe = dataOCR.plate;
                    dataDB.dia_chi = dataOCR.address;
                    dataDB.dung_tich = dataOCR.capacity;
                    dataDB.loai_xe = "";
                    dataDB.mau_son = dataOCR.color;
                    dataDB.ngay_cap = dataOCR.last_issue_date;
                    dataDB.nhan_hieu = dataOCR.brand;
                    dataDB.so_cho_dung = dataOCR.stand;
                    dataDB.so_cho_ngoi = dataOCR.sit;
                    dataDB.so_khung = dataOCR.chassis;
                    dataDB.so_loai = dataOCR.model;
                    dataDB.so_may = dataOCR.engine;
                    dataDB.ten_nguoi = dataOCR.name;
                    if (!string.IsNullOrEmpty(dataDB.bien_so_xe) && !string.IsNullOrEmpty(dataDB.ten_nguoi))
                        dataDB.loai_vb = "mat_sau_dang_ky_xe";
                    else
                        dataDB.loai_vb = "";
                }
            }
            return dataDB;
        }
    }
    public class data_ocr_dang_ky_xe
    {
        public data_dang_ky_xe info;
        public string type;
    }
    public class data_dang_ky_xe
    {
        public string address { get; set; }
        public string address_district { get; set; }
        public string address_district_code { get; set; }
        public string address_town { get; set; }
        public string address_town_code { get; set; }
        public string address_ward { get; set; }
        public string address_ward_code { get; set; }
        public string brand { get; set; }
        public string capacity { get; set; }
        public string chassis { get; set; }
        public string color { get; set; }
        public string engine { get; set; }
        public string first_issue_date { get; set; }
        public string image { get; set; }
        public string issued_at { get; set; }
        public string issued_at_code { get; set; }
        public string last_issue_date { get; set; }
        public string lie { get; set; }
        public string model { get; set; }
        public string name { get; set; }
        public string pay_load { get; set; }
        public string plate { get; set; }
        public string sit { get; set; }
        public string stand { get; set; }
        public string year_of_manufacture { get; set; }
        public string id { get; set; }
    }

    public class db_ocr_dang_ky_xe
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
        public string bien_so_xe { get; set; }
        public string dia_chi { get; set; }
        public string dung_tich { get; set; }
        public string loai_vb { get; set; }
        public string loai_xe { get; set; }
        public string mau_son { get; set; }
        public string ngay_cap { get; set; }
        public string nhan_hieu { get; set; }
        public string so_cho_dung { get; set; }
        public string so_cho_ngoi { get; set; }
        public string trong_tai { get; set; }
        public string so_khung { get; set; }
        public string so_loai { get; set; }
        public string so_may { get; set; }
        public string ten_nguoi { get; set; }
    }
}
