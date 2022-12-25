using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.SMS.FPT
{
    public class fpt_response_send_sms
    {
        public string MessageId { get; set; }
        public double Phone { get; set; }
        public string BrandName { get; set; }
        public string Message { get; set; }
        public string PartnerId { get; set; }
        public string Telco { get; set; }

        public decimal? error { get; set; }
        public string error_description { get; set; }
    }
}
