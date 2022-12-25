using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC
{
    public class rq_push_data_abic_create
    {
        public string ma_dvi { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }
        public string type { get; set; }
        public string nhom { get; set; }
        public string so_hd { get; set; }
        public string ngay_d_date { get; set; }
        public string ngay_c_date { get; set; }
        public decimal? ngay_d { get; set; }
        public decimal? ngay_c { get; set; }
        public string hash { get; set; }
    }
}
