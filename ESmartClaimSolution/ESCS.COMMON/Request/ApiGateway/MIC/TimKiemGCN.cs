using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class TimKiemGCN
    {
        public string ma_dvi { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }

        public string ma_chi_nhanh { get; set; }
        public string so_hdong { get; set; }
        public string so_gcn { get; set; }
        public string bien_so_xe { get; set; }
        public string so_khung { get; set; }
        public string so_may { get; set; }
        public string cmt_kh { get; set; }
        public string mst_kh { get; set; }
        public string ten_kh { get; set; }
    }
}
