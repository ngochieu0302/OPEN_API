using ESCS.COMMON.Common;
using ESCS.COMMON.SMS.FPT;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogSMSFPT
    {
        [BsonId]
        public string id { get; set; }
        public long time { get; set; }
        public string base_url { get; set; }
        public string api_send_sms { get; set; }
        public string client_id { get; set; }
        public string secret { get; set; }
        public fpt_request_token request_token { get; set; }
        public fpt_response_token response_token { get; set; }
        public fpt_request_send_sms request_send_sms { get; set; }
        public fpt_response_send_sms response_send_sms { get; set; }
        public LogSMSFPT()
        {

        }
        public LogSMSFPT(fpt_request_token request_token)
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            this.request_token = request_token;
        }
        public LogSMSFPT(fpt_response_token response_token, fpt_request_send_sms request_send_sms, fpt_response_send_sms response_send_sms)
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            this.response_token = response_token;
            this.request_send_sms = request_send_sms;
            this.response_send_sms = response_send_sms;
        }
    }
}
