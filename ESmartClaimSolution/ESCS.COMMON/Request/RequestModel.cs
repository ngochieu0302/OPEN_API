using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class RequestModel<T>
    {
        public DefineInfo define_info { get; set; }
        public T data_info { get; set; }
        public States states { get; set; }
    }
}
