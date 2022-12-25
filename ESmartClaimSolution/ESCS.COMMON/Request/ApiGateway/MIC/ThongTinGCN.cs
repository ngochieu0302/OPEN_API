using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class ThongTinGCN
    {
        public string ma_dvi { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }
        public string ma_chi_nhanh { get; set; }
        public string so_id_hd { get; set; }
        public string so_id_gcn { get; set; }
    }
}
