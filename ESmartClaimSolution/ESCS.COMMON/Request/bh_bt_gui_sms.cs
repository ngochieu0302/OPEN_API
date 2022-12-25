using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class bh_bt_gui_sms
    {
        public string ma_doi_tac_nsd { get; set; }
        public string nsd { get; set; }
        public string ma_doi_tac { get; set; }
        public string ma_doi_tac_ql { get; set; }
        public decimal? so_id { get; set; }
        public decimal? bt { get; set; }
        public string nv { get; set; }
        public string pm { get; set; }
        public string nguon { get; set; }
        public string noi_dung { get; set; }
        public string sdt_nhan { get; set; }
        public decimal? ngay { get; set; }
        public string trang_thai { get; set; }
        public string loai { get; set; }

        public string doi_tac_sms { get; set; }
        public string base_url { get; set; }
        public string api_auth { get; set; }
        public string api_send_sms { get; set; }
        public string client_id { get; set; }
        public string secret { get; set; }
        public string brandname { get; set; }
    }
}
