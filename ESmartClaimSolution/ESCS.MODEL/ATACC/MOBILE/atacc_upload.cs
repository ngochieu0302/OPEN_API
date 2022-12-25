using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class atacc_upload
    {
        public IFormFileCollection fileupload { get; set; }
        public string ma_doi_tac_nsd { get; set; }
        public string user_name { get; set; }
        public string pass { get; set; }

        public string jobId { get; set; }
        public decimal? so_id_hs { get; set; }
        public string ma_hang_muc { get; set; }
        public string user { get; set; }
        public string departmentId { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public string ma_tvv { get; set; }
        public string ten_hang_muc { get; set; }
        public string source { get; set; }
        public string type_product { get; set; }
        public string checksum { get; set; }
    }
}
