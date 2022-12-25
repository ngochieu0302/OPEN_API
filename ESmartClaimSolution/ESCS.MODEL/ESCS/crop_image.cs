using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class crop_image
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }
        public string nguon { get; set; }
        public string loai_tai_lieu { get; set; }
        public IFormFile file { get; set; }
    }
}
