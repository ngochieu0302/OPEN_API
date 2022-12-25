using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_access_token
    {
        public string partner_code { get; set; }
        public string evncode { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public long? time_exprive { get; set; }
        public string type { get; set; }
        public string signature { get; set; }
        public long? time_connect { get; set; }
        public openid_access_token()
        {
            type = "access_token";
        }
    }
}
