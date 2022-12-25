using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class bh_file_video
    {
        [JsonIgnore]
        public string ma_doi_tac_nsd { get; set; }
        [JsonIgnore]
        public string ma_chi_nhanh_nsd { get; set; }
        [JsonIgnore]
        public string nsd { get; set; }
        [JsonIgnore]
        public string pas { get; set; }
        public string ma_doi_tac { get; set; }
        public decimal? so_id { get; set; }
        public decimal? so_id_doi_tuong { get; set; }
        public decimal? bt { get; set; }
        public string duong_dan { get; set; }
        public string ten { get; set; }
        public string link_video { get; set; }
        public string ngay { get; set; }
    }
}
