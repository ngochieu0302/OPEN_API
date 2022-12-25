using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class ocr_data_type
    {
        public List<data_type> data { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class data_type
    {
        public int id { get; set; }
        public string image { get; set; }
        public string label { get; set; }
    }
}
