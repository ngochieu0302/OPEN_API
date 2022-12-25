using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class atacc_response_v4<T>
    {
        public string RspCode { get; set; } = "00";
        public string Message { get; set; } = "SUCCESS";
        public T data { get; set; }
    }
}
