using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.SMS.FPT
{
    public class fpt_response_token
    {
        public string access_token { get; set; }
        public double expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }

        public decimal? error { get; set; }
        public string error_description { get; set; }
    }
}
