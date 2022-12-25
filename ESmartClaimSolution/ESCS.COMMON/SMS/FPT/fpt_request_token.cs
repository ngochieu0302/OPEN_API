using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.SMS.FPT
{
    public class fpt_request_token
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string scope { get; set; }
        public string session_id { get; set; }
        public string grant_type { get; set; }
        public fpt_request_token(string client_id, string client_secret)
        {
            this.client_id = client_id;
            this.client_secret = client_secret;
            this.scope = "send_brandname_otp send_brandname";
            this.session_id = Guid.NewGuid().ToString("N");
            this.grant_type = "client_credentials";
        }
    }
}
