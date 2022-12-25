using ESCS.COMMON.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class BaseRequestOpenId : BaseRequest
    {
        public BaseRequestOpenId()
        {
            define_info = new DefineInfo();
            states = new States()
            {
                status = "OK",
                request_time = DateTime.Now.ToString("yyyyMMddHHmmss"),
                execute_time = DateTime.Now.ToString("yyyyMMddHHmmss"),
                response_time = ""
            };
        }
        public BaseRequestOpenId(Dictionary<string,string> data)
        {
            define_info = new DefineInfo();
            states = new States()
            {
                status = "OK",
                request_time = DateTime.Now.ToString("yyyyMMddHHmmss"),
                execute_time = DateTime.Now.ToString("yyyyMMddHHmmss"),
                response_time = ""
            };
            data_info = data;
        }
    }

    public class BaseRequestEscs
    {
        public BaseRequestEscs()
        {
            
        }
        /// <summary>
        /// Thông tin định nghĩa request của client
        /// </summary>
        public DefineInfo define_info { get; set; }
        /// <summary>
        /// Thông tin định nghĩa đối tác
        /// </summary>
        //public UserInfo user_info { get; set; }
        /// <summary>
        /// Thông tin dữ liệu
        /// </summary>
        public dynamic data_info { get; set; }
        /// <summary>
        /// Chữ ký dữ liệu
        /// </summary>
        //public string signature { get; set; }
        /// <summary>
        /// Thông tin request và response
        /// </summary>
        public States states { get; set; }
        //public BaseRequest()
        //{
        //    user_info = new UserInfo();
        //}
        public BaseRequestEscs(DefineInfo _define_info)
        {
            define_info = _define_info;
            //user_info = new UserInfo();
        }
        public BaseRequestEscs(dynamic _data_info)
        {
            data_info = _data_info;
        }
        
        public BaseRequestEscs(dynamic _data_info, DefineInfo _define_info = null)
        {
            if (_define_info == null)
            {
                _define_info = new DefineInfo();
            }
            define_info = _define_info;
            data_info = _data_info;
            //user_info = new UserInfo();
        }
        public BaseRequestEscs(dynamic _data_info, DefineInfo _define_info = null, States _state_info = null)
        {
            if (_define_info == null)
            {
                _define_info = new DefineInfo();
            }
            if (_state_info == null)
            {
                _state_info = new States();
            }
            define_info = _define_info;
            data_info = _data_info.ToDictionaryModel();
            states = _state_info;
        }
    }
}
