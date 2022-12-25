using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class truy_van_bt_ct
    {
        public string ma_doi_tac_nsd { get; set; }
        public string user_name { get; set; }
        public string pass { get; set; }

        public string loai { get; set; }
        public string dvi { get; set; }
        public decimal? so_id { get; set; }
        public string nv { get; set; }
        public string checksum { get; set; }
    }
}
