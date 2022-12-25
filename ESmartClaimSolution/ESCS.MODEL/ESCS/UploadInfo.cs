using ESCS.COMMON.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class UploadInfo
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        public string ma_doi_tac { get; set; }
        public decimal? so_id { get; set; }
        public decimal? so_id_dt { get; set; }
        public decimal? so_id_doi_tuong { get; set; }
        public string nv { get; set; }
        public string pm { get; set; }
        public string ung_dung { get; set; }
        public List<FileInfoData> files { get; set; }
    }
}
