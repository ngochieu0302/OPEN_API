using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogSendNotify
    {
        public string gid { get; set; }
        public string ma_doi_tac { get; set; }
        public string ten_doi_tac { get; set; }
        public string nsd { get; set; }
        public string ten_nsd { get; set; }
        public string tieu_de { get; set; }
        public string nd { get; set; }
        public string nd_tom_tat { get; set; }
        public string tg_thong_bao { get; set; }
        public string loai_thong_bao { get; set; }
        public string loai_thong_bao_hthi { get; set; }
        public string nguoi_gui { get; set; }
        public string doc_noi_dung { get; set; }
        public string doc_noi_dung_hthi { get; set; }
        public string canh_bao { get; set; }
        public string canh_bao_hthi { get; set; }
        public string tt_gui { get; set; }
        public string tt_gui_hthi { get; set; }
        public string connection_id { get; set; }
        public decimal? so_tn_chua_doc { get; set; }
        public decimal? ctiet_xem { get; set; }
        public string ctiet_hanh_dong { get; set; }
        public string ctiet_action_code { get; set; }
        public string ctiet_ma_doi_tac { get; set; }
        public string ctiet_so_id { get; set; }
        public string ung_dung { get; set; }
        public string token { get; set; }

    }
}
