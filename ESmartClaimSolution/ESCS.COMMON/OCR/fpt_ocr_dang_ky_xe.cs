using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class fpt_ocr_dang_ky_xe
    {
        public decimal? errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<data_fpt_ocr_dang_ky_xe> data { get; set; }
        public db_fpt_ocr_dang_ky_xe GetData()
        {
            if (this.data == null || this.data.Count <= 0)
                return null;
            data_fpt_ocr_dang_ky_xe dataOCR = data.FirstOrDefault();
            db_fpt_ocr_dang_ky_xe dataDB = new db_fpt_ocr_dang_ky_xe();
            dataDB.bien_so_xe = dataOCR.plate_number;
            dataDB.dia_chi = dataOCR.address;
            dataDB.dung_tich = dataOCR.vehicle_capacity;
            dataDB.loai_xe = dataOCR.vehicle_type;
            dataDB.mau_son = dataOCR.vehicle_color;
            dataDB.ngay_cap = dataOCR.registration_date;
            dataDB.nhan_hieu = dataOCR.make;
            dataDB.so_cho_dung = dataOCR.no_stand;
            dataDB.so_cho_ngoi = dataOCR.no_seat;
            dataDB.so_khung = dataOCR.chassis_number;
            dataDB.so_loai = dataOCR.model;
            dataDB.so_may = dataOCR.engine_number;
            dataDB.ten_nguoi = dataOCR.name;
            if (!string.IsNullOrEmpty(dataDB.bien_so_xe) && !string.IsNullOrEmpty(dataDB.ten_nguoi))
                dataDB.loai_vb = "dang_ky_mat_sau";
            else
                dataDB.loai_vb = "";
            return dataDB;
        }
    }
    public class data_fpt_ocr_dang_ky_xe
    {
        public string name { get; set; }
        public string name_prob { get; set; }
        public string birth_year { get; set; }
        public string birth_year_prob { get; set; }
        public string engine_number { get; set; }
        public string engine_number_prob { get; set; }
        public string address { get; set; }
        public string address_prob { get; set; }
        public string chassis_number { get; set; }
        public string chassis_number_prob { get; set; }
        public string make { get; set; }
        public string make_prob { get; set; }
        public string vehicle_type { get; set; }
        public string vehicle_type_prob { get; set; }
        public string vehicle_color { get; set; }
        public string vehicle_color_prob { get; set; }
        public string no_seat { get; set; }
        public string no_seat_prob { get; set; }
        public string model { get; set; }
        public string model_prob { get; set; }
        public string vehicle_capacity { get; set; }
        public string vehicle_capacity_prob { get; set; }
        public string plate_number { get; set; }
        public string plate_number_prob { get; set; }
        public string registration_date { get; set; }
        public string registration_date_prob { get; set; }
        public string gross_weight { get; set; }
        public string gross_weight_prob { get; set; }
        public string no_stand { get; set; }
        public string no_stand_prob { get; set; }
        public string no_lie { get; set; }
        public string no_lie_prob { get; set; }
        public string doe { get; set; }
        public string doe_prob { get; set; }
        public string plate_color { get; set; }
        public string plate_color_prob { get; set; }
        public string overall_score { get; set; }
        public string type { get; set; }
    }
    public class db_fpt_ocr_dang_ky_xe
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        public string ung_dung { get; set; }
        public string so_id { get; set; }
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
        public string so_khung { get; set; }
        public string so_loai { get; set; }
        public string so_may { get; set; }
        public string ten_nguoi { get; set; }
    }
}
