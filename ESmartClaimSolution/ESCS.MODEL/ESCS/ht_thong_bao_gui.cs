using ESCS.COMMON.MongoDb.LogEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class ht_thong_bao_gui
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

        public LogSendNotify GetLog()
        {
            LogSendNotify _log = new LogSendNotify();
            _log.gid = this.gid;
            _log.ma_doi_tac = this.ma_doi_tac;
            _log.ten_doi_tac = this.ten_doi_tac;
            _log.nsd = this.nsd;
            _log.ten_nsd = this.ten_nsd;
            _log.tieu_de = this.tieu_de;
            _log.nd = this.nd;
            _log.nd_tom_tat = this.nd_tom_tat;
            _log.tg_thong_bao = this.tg_thong_bao;
            _log.loai_thong_bao = this.loai_thong_bao;
            _log.loai_thong_bao_hthi = this.loai_thong_bao_hthi;
            _log.nguoi_gui = this.nguoi_gui;
            _log.doc_noi_dung = this.doc_noi_dung;
            _log.doc_noi_dung_hthi = this.doc_noi_dung_hthi;
            _log.canh_bao = this.canh_bao;
            _log.canh_bao_hthi = this.canh_bao_hthi;
            _log.tt_gui = this.tt_gui;
            _log.tt_gui_hthi = this.tt_gui_hthi;
            _log.connection_id = this.connection_id;
            _log.so_tn_chua_doc = this.so_tn_chua_doc;
            _log.ctiet_xem = this.ctiet_xem;
            _log.ctiet_hanh_dong = this.ctiet_hanh_dong;
            _log.ctiet_action_code = this.ctiet_action_code;
            _log.ctiet_ma_doi_tac = this.ctiet_ma_doi_tac;
            _log.ctiet_so_id = this.ctiet_so_id;
            _log.ung_dung = this.ung_dung;
            _log.token = this.token;
            return _log;
        }
    }
}
