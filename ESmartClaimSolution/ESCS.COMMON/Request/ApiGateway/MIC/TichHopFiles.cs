using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class TichHopFiles
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        public string ma_doi_tac { get; set; }
        public string ma_chi_nhanh { get; set; }
        public string so_id_hd { get; set; }
        public string so_id_gcn { get; set; }
        public List<KQDanhSachFile> files { get; set; }
    }
}
