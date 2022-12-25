using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class ocr_bao_gia_gara
    {
        public List<data_ocr_bao_gia_gara> data { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class data_ocr_bao_gia_gara
    {
        public data_bao_gia_gara info { get; set; }
        public string type { get; set; }
    }
    public class data_bao_gia_gara
    {
        public string estimated_delivery_date { get; set; }
        public string name_of_garage { get; set; }
        public string quotation_date { get; set; }
        public string sub_total { get; set; }
        public List<Table> table { get; set; }
        public string total_amount { get; set; }
        public string vat_amount { get; set; }
    }
    public class Table
    {
        public string description { get; set; }
        public string quantity { get; set; }
        public string unit_price { get; set; }
        public string percent_discount { get; set; }
        public string discount { get; set; }
        public string amount_total { get; set; }
        public string tax { get; set; }
        
    }
}
