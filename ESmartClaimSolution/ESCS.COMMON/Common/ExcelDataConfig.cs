using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    /// <summary>
    /// cur: ds_hang
    ///field: ngay_hieu_luc
    ///type: date
    ///format: dd/MM/yyyy
    ///required: true
    ///max:
    ///min: 
    ///minlength: 
    ///maxlength: 
    ///result: yyyyMMddHHmmss
    /// </summary>
    public class ExcelDataConfig
    {
        public string cur { get; set; }
        public string field { get; set; }
        public string type { get; set; }
        public string format { get; set; }
        public bool? required { get; set; }
        public decimal? max { get; set; }
        public decimal? min { get; set; }
        public decimal? minlength { get; set; }
        public decimal? maxlength { get; set; }
        public string result { get; set; }
        public int? row_index { get; set; }
        public int? col_index { get; set; }
    }
}
