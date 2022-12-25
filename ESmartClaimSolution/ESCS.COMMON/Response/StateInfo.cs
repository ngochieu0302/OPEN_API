using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Response
{
    public class StateInfo
    {
        public string status { get; set; }
        public string message_code { get; set; }
        public string message_body { get; set; }
        public string signature { get; set; }
        public List<string> errors { get; set; }
        public StateInfo()
        {
            status = "OK";
        }
    }

}
