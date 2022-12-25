using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class opes_yc_bshs
    {
        public long so_id_hs { get; set; }
        public List<opes_tai_lieu> tl_bs { get; set; }
    }
    public class opes_tai_lieu
    {
        public string ma_tl { get; set; }
        public string ten_tl { get; set; }
        public string ghi_chu { get; set; }
        public int md_uu_tien { get; set; }
        public opes_tai_lieu()
        {
            md_uu_tien = 0;
        }
    }
}
