using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class ocr_bang_ke_chi_tiet
    {
        public List<ocr_bang_ke> data { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class ocr_bang_ke {
        public Info info { get; set; }
        public List<int> pages { get; set; }
        public string type { get; set; }
    }

    public class Info
    {
        public string address { get; set; }
        public string medical_facility { get; set; }
        public string patient_name { get; set; }
        public string pid { get; set; }
        public TableBangKe table { get; set; }
        public object table_date { get; set; }
        public object table_number { get; set; }
        public object total_payment { get; set; }
    }
    public class TableBangKe
    {
        public List<string> image_table { get; set; }
        public List<List<List<InfoTable>>> info_table { get; set; }
        public List<int> page_table { get; set; }
    }
    public class InfoTable
    {
        public string value { get; set; }
    }
}
