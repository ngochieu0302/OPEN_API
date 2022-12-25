using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class atacc_response_v2<T>
    {
        public int code { get; set; } = 1;
        public string response_message { get; set; } = "SUCCESS";
        public string errors { get; set; }
        public T data { get; set; }
    }
}
