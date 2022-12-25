using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.SMS.MCM
{
    public class mcm_gui_sms
    {
        public mcm_dich_vu dich_vu { get; set; }
        public List<mcm_lich_gui> lich_gui { get; set; }
        public List<mcm_lich_gui_param> lich_gui_param { get; set; }
    }
    public class mcm_lich_gui
    {
        public string ma_doi_tac { get; set; }
        public decimal? bt { get; set; }
        public string tempid { get; set; }
        public string dthoai { get; set; }
        public string ten_chien_dich { get; set; }
        public string channel { get; set; }
        public string nd_sms { get; set; }
        public string param_zalo { get; set; }
        public string param_viber { get; set; }
        public string param_sms { get; set; }
        public string nv { get; set; }
        public decimal? so_id { get; set; }
        public decimal? trang_thai { get; set; }
        public string callback { get; set; }
        public DateTime callback_ngay { get; set; }
        public DateTime ngay { get; set; }
        public decimal? tgian_gui { get; set; }
        public string nsd { get; set; }
        public string ma_file { get; set; }
        public string url_file { get; set; }
    }
    public class mcm_lich_gui_param
    {
        public string ma_doi_tac { get; set; }
        public decimal? bt { get; set; }
        public string param_code { get; set; }
        public string gia_tri { get; set; }
        public string loai { get; set; }
        public string stt { get; set; }
    }
}
