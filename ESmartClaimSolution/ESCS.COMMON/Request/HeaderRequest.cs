using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class HeaderRequest
    {
        public string partner_code { get; set; }
        public string token { get; set; }
        public string action { get; set; }
        public string envcode { get; set; }
        public string signature { get; set; }
        public bool check_ip_backlist { get; set; }
        public string ip_remote_ipv4 { get; set; }
        public string ip_remote_ipv6 { get; set; }
        public string payload { get; set; }
        public HeaderRequest()
        {
            check_ip_backlist = false;
            this.envcode = "DEV";
        }
        public HeaderRequest(string partner_code, string token, string action)
        {
            this.partner_code = partner_code;
            this.token = token;
            this.action = action;
            check_ip_backlist = false;
            this.envcode = "DEV";
        }
    }
}
