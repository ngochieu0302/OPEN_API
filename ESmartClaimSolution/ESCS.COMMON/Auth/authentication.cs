using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Auth
{
    public class authentication
    {
        public string access_token { get; set; }
        public string refesh_token { get; set; }
        public string environment { get; set; }
        public long? time_exprive { get; set; }
        public long? time_connect { get; set; }
        public string token_type { get; set; } = "eAuthToken";
    }
}
