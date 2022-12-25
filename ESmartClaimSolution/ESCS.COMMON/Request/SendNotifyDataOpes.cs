using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class SendNotifyDataOpes
    {
        public SendNotifyDataOpesStateInfo state_info { get; set; }
        public SendNotifyDataOpesDataInfo data_info { get; set; }
        public SendNotifyDataOpes()
        {
            this.state_info = new SendNotifyDataOpesStateInfo();
            this.data_info = new SendNotifyDataOpesDataInfo();
        }
    }
    public class SendNotifyDataOpesStateInfo
    {
        public string status { get; set; } = "OK";
        public string message_code { get; set; }
        public string message_body { get; set; }
    }
    public class SendNotifyDataOpesDataInfo
    {
        public string user { get; set; }
        public string device { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public IDictionary<string,string> messageData { get; set; }
    }
}
