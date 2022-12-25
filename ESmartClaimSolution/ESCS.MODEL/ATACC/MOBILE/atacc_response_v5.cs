using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class atacc_response_v5<T>
    {
        public string resultOutValue { get; set; } = "";
        public string resultmessage { get; set; } = "SUCCESS";
        public T data { get; set; }
    }
}
