using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_refresh_token
    {
        public string partner_code { get; set; }
        public string access_token { get; set; }
        public long? time_exprive { get; set; }
        public string token_type { get; set; }
        public string signature { get; set; }
        public openid_refresh_token()
        {
            token_type = "eAuthToken";
        }
    }
}
