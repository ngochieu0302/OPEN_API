using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class DefineInfo
    {
        public string accept { get; set; }
        public string accept_encoding { get; set; }
        public string host { get; set; }
        public string referer { get; set; }
        public string user_agent { get; set; }
        public string origin { get; set; }
        public string ip_remote_ipv4 { get; set; }
        public string ip_remote_ipv6 { get; set; }
        public long? time { get; set; }
    }
}
