using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Response
{
    
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
    public class out_value
    {
        public string tong_so_dong { get; set; }
    }
}
