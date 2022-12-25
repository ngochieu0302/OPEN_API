using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class kiem_tra_ket_noi
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }
        public string ma_doi_tac { get; set; }
        public long? so_id { get; set; }
        public List<can_bo_ket_noi> ds_ket_noi { get; set; }
    }
    public class can_bo_ket_noi
    {
        public string dvi_gdinh { get; set; }
        public string ma_gdv { get; set; }
        public string ten_gdv { get; set; }
        public int connected { get; set; }
    }
}
