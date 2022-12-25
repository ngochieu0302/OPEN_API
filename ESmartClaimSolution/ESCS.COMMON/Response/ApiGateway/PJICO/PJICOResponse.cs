using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Response.ApiGateway.PJICO
{
    public class PJICOResponse<T>
    {
        public int returnCode { get; set; }
        public string errMess { get; set; }
        public T data { get; set; }
    }
}
