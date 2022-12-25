using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Excels
{
    public class ExcelCursor
    {
        public string cursor_name { get; set; }
        public bool is_array { get; set; }
        public List<ExcelParam> param { get; set; }
    }
}
