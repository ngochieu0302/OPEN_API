using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class banner_qc
    {
        public string ma_doi_tac_nsd { get; set; }
        public string user_name { get; set; }
        public string pass { get; set; }

        public decimal? status { get; set; }
        public string checksum { get; set; }
    }
}
