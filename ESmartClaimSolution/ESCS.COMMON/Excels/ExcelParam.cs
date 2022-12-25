using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Excels
{
    public class ExcelParam
    {
        public string field { get; set; }
        public string type { get; set; } = ExcelType.TEXT;
        public string format { get; set; }
    }
}
