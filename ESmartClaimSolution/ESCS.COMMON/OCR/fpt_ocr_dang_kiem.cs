using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class fpt_ocr_dang_kiem
    {
        public decimal? errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<data_fpt_ocr_dang_kiem> data { get; set; }
        public db_fpt_ocr_dang_kiem GetData()
        {
            if (this.data == null || this.data.Count <= 0)
                return null;
            data_fpt_ocr_dang_kiem dataOCR = data.FirstOrDefault();
            db_fpt_ocr_dang_kiem dataDB = new db_fpt_ocr_dang_kiem();
            dataDB.bien_so_xe = dataOCR.plate_number;
            dataDB.kinh_doanh_van_tai = dataOCR.chassis_number;
            dataDB.loai_xe = dataOCR.type;
            dataDB.nam_san_xuat = dataOCR.year_country!=null? dataOCR.year_country.Split(",")[0]:"";
            dataDB.ngay_cap = dataOCR.doi;
            dataDB.ngay_het_hieu_luc = dataOCR.valid_until;
            dataDB.nhan_hieu = dataOCR.mark;
            dataDB.nuoc_san_xuat = "";
            dataDB.so_cho_dung = "";
            dataDB.so_cho_nam = "";
            dataDB.so_cho_ngoi = "";
            dataDB.so_khung = dataOCR.chassis_number;
            dataDB.so_loai = dataOCR.model_code;
            dataDB.so_may = dataOCR.engine_number;
            dataDB.so_quan_ly = dataOCR.inspection_number;
            dataDB.so_seri = dataOCR.seri_number;
            if (dataOCR.template == "giay-dang-kiem-mat-truoc")
                dataDB.loai_vb = "dang_kiem_mat_sau";
            else
                dataDB.loai_vb = "";
            return dataDB;
        }
    }
    public class data_fpt_ocr_dang_kiem
    {
        public string plate_number { get; set; }
        public string plate_number_prob { get; set; }
        public string inspection_number { get; set; }
        public string inspection_number_prob { get; set; }
        public string type { get; set; }
        public string type_prob { get; set; }
        public string mark { get; set; }
        public string mark_prob { get; set; }
        public string model_code { get; set; }
        public string model_code_prob { get; set; }
        public string engine_number { get; set; }
        public string engine_number_prob { get; set; }
        public string chassis_number { get; set; }
        public string chassis_number_prob { get; set; }
        public string wheel_formula { get; set; }
        public string wheel_formula_prob { get; set; }
        public string wheel_tread { get; set; }
        public string wheel_tread_prob { get; set; }
        public string overall_dimension { get; set; }
        public string overall_dimension_prob { get; set; }
        public string year_country { get; set; }
        public string year_country_prob { get; set; }
        public string wheel_base { get; set; }
        public string wheel_base_prob { get; set; }
        public string kerb_mass { get; set; }
        public string kerb_mass_prob { get; set; }
        public string total_mass { get; set; }
        public string total_mass_prob { get; set; }
        public string seat { get; set; }
        public string seat_prob { get; set; }
        public string type_fuel { get; set; }
        public string type_fuel_prob { get; set; }
        public string engine_displacement { get; set; }
        public string engine_displacement_prob { get; set; }
        public string max_output { get; set; }
        public string max_output_prob { get; set; }
        public string seri_number { get; set; }
        public string seri_number_prob { get; set; }
        public string tires { get; set; }
        public string tires_prob { get; set; }
        public string inspection_report_number { get; set; }
        public string inspection_report_number_prob { get; set; }
        public string valid_until { get; set; }
        public string valid_until_prob { get; set; }
        public string doi { get; set; }
        public string doi_prob { get; set; }
        public string commercial_use { get; set; }
        public string commercial_use_prob { get; set; }
        public string modification { get; set; }
        public string modification_prob { get; set; }
        public string equipped { get; set; }
        public string equipped_prob { get; set; }
        public string inspection_stamp { get; set; }
        public string inspection_stamp_prob { get; set; }
        public string overall_score { get; set; }
        public string template { get; set; }
    }
    public class db_fpt_ocr_dang_kiem
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        public string ung_dung { get; set; }
        public string so_id { get; set; }
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
