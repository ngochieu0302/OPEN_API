using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class SendNotifyDataMIC
    {
        public string phoneNumber { get; set; }
        public string content { get; set; }
        public string title { get; set; }
        public string action { get; set; }
        public string id { get; set; }
        public SendNotifyDataMIC()
        {
            this.action = "1";
        }
    }
}
