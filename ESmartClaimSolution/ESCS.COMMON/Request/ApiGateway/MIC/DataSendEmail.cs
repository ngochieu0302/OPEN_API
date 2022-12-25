using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class DataSendEmail
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_dvi { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }
        public string loai { get; set; }
        public string from_mail { get; set; }
        public string title { get; set; }
        public string toi { get; set; }
        public string nd { get; set; }
    }
}
