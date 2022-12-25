using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class TichHopEmailSMS
    {
        public string ma_dvi { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }
        public string check_sum { get; set; }

        public string loai { get; set; }
        public string from_mail { get; set; }
        public string title { get; set; }
        public string toi { get; set; }
        public string nd { get; set; }
    }
}
