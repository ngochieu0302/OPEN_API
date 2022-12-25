using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.OCR
{
    public class ocr_response_read_ocr
    {
        public List<ocr_response> data { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class ocr_response
    {
        public string type { get; set; }
    }
}
