using ESCS.COMMON.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class BaseRequest
    {
        public BaseRequest()
        {
            //data_info = new Dictionary<string, string>();
        }
        /// <summary>
        /// Thông tin định nghĩa request của client
        /// </summary>
        public DefineInfo define_info { get; set; }
        public Dictionary<string, string> data_info { get; set; }
        public MailConfig mail_config { get; set; }
        public object mail_data { get; set; }
        public object file_data { get; set; }
        public States states { get; set; }
        public BaseRequest(DefineInfo _define_info)
        {
            define_info = _define_info;
        }
        public BaseRequest(Dictionary<string, string> _data_info)
        {
            data_info = _data_info;
        }
        public BaseRequest(object _data_info)
        {
            data_info = _data_info.ToDictionaryModel();
        }
        public BaseRequest(Dictionary<string, string> _data_info, DefineInfo _define_info = null)
        {
            if (_define_info == null)
            {
                _define_info = new DefineInfo();
            }
            define_info = _define_info;
            data_info = _data_info;
        }
        public BaseRequest(object _data_info, DefineInfo _define_info = null, States _state_info = null)
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
    public class BaseRequest<T>
    {
        public DefineInfo define_info { get; set; }
        public T data_info { get; set; }
        public States states { get; set; }
        public BaseRequest()
        {

        }
        public BaseRequest(DefineInfo _define_info)
        {
            define_info = _define_info;
        }
        public BaseRequest(T _data_info)
        {
            data_info = _data_info;
        }
        public BaseRequest(T _data_info, DefineInfo _define_info = null)
        {
            if (_define_info == null)
            {
                _define_info = new DefineInfo();
            }
            define_info = _define_info;
            data_info = _data_info;
        }
    }
    public class BaseRequestMail
    {
        public BaseRequestMail()
        {
            data_info = new Dictionary<string, string>();
        }
        public DefineInfo define_info { get; set; }
        public dynamic data_info { get; set; }
        public MailConfig mail_config { get; set; }
        public States states { get; set; }

        public BaseRequestMail(DefineInfo _define_info)
        {
            define_info = _define_info;
        }
        public BaseRequestMail(dynamic _data_info)
        {
            data_info = _data_info;
        }
        public BaseRequestMail(dynamic _data_info, DefineInfo _define_info = null)
        {
            if (_define_info == null)
            {
                _define_info = new DefineInfo();
            }
            define_info = _define_info;
            data_info = _data_info;
        }
        public BaseRequestMail(dynamic _data_info, DefineInfo _define_info = null, States _state_info = null)
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
            data_info = _data_info;
            states = _state_info;
        }
    }
}
