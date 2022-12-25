using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class atacc_response<T>
    {
        public string resultmessage { get; set; } = "SUCCESS";
        public T resultlist { get; set; }
    }
}
