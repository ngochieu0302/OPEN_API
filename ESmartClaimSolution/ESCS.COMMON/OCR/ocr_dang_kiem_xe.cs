using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class ocr_dang_kiem_xe
    {
        public List<data_ocr_dang_kiem_xe> data { get; set; }
        public decimal? errorCode { get; set; }
        public string errorMessage { get; set; }
        public db_ocr_dang_kiem_xe GetData()
        {
            data_ocr_dang_kiem_xe dataResponse = this.data.FirstOrDefault();
            db_ocr_dang_kiem_xe dataDB = new db_ocr_dang_kiem_xe();
            data_dang_kiem_xe dataOCR = dataResponse.info;

            dataDB.bien_so_xe = dataOCR.registration_number;
            dataDB.kinh_doanh_van_tai = dataOCR.commercial_use == "False" ? "Không" : "Có";
            dataDB.loai_xe = dataOCR.type;
            dataDB.nam_san_xuat = dataOCR.manufactured_year;
            dataDB.ngay_cap = dataOCR.regis_date;
            dataDB.ngay_het_hieu_luc = dataOCR.valid_until;
            dataDB.nhan_hieu = dataOCR.mark;
            dataDB.nuoc_san_xuat = dataOCR.manufactured_country;
            dataDB.so_cho_dung = dataOCR.stand_place;
            dataDB.so_cho_nam = dataOCR.lying_place;
            dataDB.so_cho_ngoi = dataOCR.seat_place;
            dataDB.so_khung = dataOCR.chassis_number;
            dataDB.so_loai = dataOCR.model_code;
            dataDB.so_may = dataOCR.engine_number;
            dataDB.so_quan_ly = dataOCR.report_number;
            dataDB.so_seri = dataOCR.seri;
            dataDB.loai_vb = dataResponse.type == "picertificate" ? "dang_kiem_mat_sau" : "";
            return dataDB;
        }
    }
    public class data_ocr_dang_kiem_xe
    {
        public data_dang_kiem_xe info;
        public string type;
    }
    public class data_dang_kiem_xe
    {
        public string authorized_pay_load { get; set; }
        public string capacity { get; set; }
        public string chassis_number { get; set; }
        public string commercial_use { get; set; }
        public string design_pay_load { get; set; }
        public string design_towed_mass { get; set; }
        public string engine_number { get; set; }
        public string image { get; set; }
        public string inside_cargo_container_dimension { get; set; }
        public string issued_on { get; set; }
        public string issued_on_code { get; set; }
        public string life_time_limit { get; set; }
        public string lying_place { get; set; }
        public string manufactured_country { get; set; }
        public string manufactured_year { get; set; }
        public string mark { get; set; }
        public string model_code { get; set; }
        public string modification { get; set; }
        public string permissible_no { get; set; }
        public string regis_date { get; set; }
        public string registration_number { get; set; }
        public string report_number { get; set; }
        public string seat_place { get; set; }
        public string seri { get; set; }
        public string stand_place { get; set; }
        public List<string> tire_size { get; set; }
        public string type { get; set; }
        public string valid_until { get; set; }
        public string wheel_form { get; set; }
    }

    public class db_ocr_dang_kiem_xe
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
        public string kinh_doanh_van_tai { get; set; }
        public string loai_vb { get; set; }
        public string loai_xe { get; set; }
        public string nam_san_xuat { get; set; }
        public string ngay_cap { get; set; }
        public string ngay_het_hieu_luc { get; set; }
        public string nhan_hieu { get; set; }
        public string nuoc_san_xuat { get; set; }
        public string so_cho_dung { get; set; }
        public string so_cho_nam { get; set; }
        public string so_cho_ngoi { get; set; }
        public string so_khung { get; set; }
        public string so_loai { get; set; }
        public string so_may { get; set; }
        public string so_quan_ly { get; set; }
        public string so_seri { get; set; }
    }
}
