using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS_SERVICE.Common
{
    public class HttpStatus
    {
        public const string STATUS_OK = "OK";
        public const string STATUS_NOTOK = "NotOK";
    }
    public class BaseResponse<T>
    {
        public StateInfo state_info { get; set; }
        public T data_info { get; set; }
        public object out_value { get; set; }
        public BaseResponse()
        {
            state_info = new StateInfo();
        }
    }
    public class BaseResponse<T, O>
    {
        public StateInfo state_info { get; set; }
        public T data_info { get; set; }
        public O out_value { get; set; }
        public BaseResponse()
        {
            state_info = new StateInfo();
        }
    }
    public class StateInfo
    {
        public string status { get; set; }
        public string message_code { get; set; }
        public string message_body { get; set; }
        public StateInfo()
        {
            status = HttpStatus.STATUS_OK;
            message_code = "00";
            message_body = "Thành công";
        }
    }
}
