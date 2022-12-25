using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class ht_email_mau_gui
    {
        [JsonIgnore]
        public IFormFile file { get; set; }
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        public string ma_doi_tac { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string url { get; set; }
        public string url_banner { get; set; }
        public decimal? ngay_ht { get; set; }
        public decimal? ap_dung { get; set; }
        public string action { get; set; }
        public string ma_files { get; set; }
        public string ten_files { get; set; }
        public string nv { get; set; }
        public string pm { get; set; }
        public string tai_khoan_gui { get; set; }
        public decimal? so_lan_lap { get; set; }
        public decimal? tg_lap_p { get; set; }
        public string tu_ht_dt { get; set; }
        public doi_tac_ql_gui_email[] arr { get; set; }
    }
    public class doi_tac_ql_gui_email
    {
        public string ma_doi_tac_ql { get; set; }
        public string email_gui { get; set; }
    }
}
