using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.SMS.MCM
{
    public class mcm_request
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string Phone { get; set; }
        public List<string> Channels { get; set; }
        public List<object> Data { get; set; }
    }
    public class mcm_request_zalo
    {
        public string OAID { get; set; }
        public string TempID { get; set; }
        public List<string> Params { get; set; }
        public string campaignid { get; set; }
        public string CallbackUrl { get; set; }
    }
    public class mcm_request_sms_single
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string Phone { get; set; }

        public string Content { get; set; }
        public int? IsUnicode { get; set; }
        public int? SmsType { get; set; }
        public string Brandname { get; set; }
        public string RequestId { get; set; }
        public string campaignid { get; set; }
        public string CallbackUrl { get; set; }

    }
    public class mcm_request_sms
    {
        public string Content { get; set; }
        public int? IsUnicode { get; set; }
        public int? SmsType { get; set; }
        public string Brandname { get; set; }
        public string RequestId { get; set; }
        public string campaignid { get; set; }
        public string CallbackUrl { get; set; }
    }
}
