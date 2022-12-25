using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.SMS.FPT
{
    public class fpt_request_send_sms
    {
        public string access_token { get; set; }
        public string session_id { get; set; }
        public string BrandName { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string RequestId { get; set; }
        public fpt_request_send_sms(string access_token, string BrandName, string Phone, string Message)
        {
            this.access_token = access_token;
            this.session_id = Guid.NewGuid().ToString("N");
            this.BrandName = BrandName;
            this.Phone = Phone;
            this.Message = Message;
            this.RequestId = "CLOUDAPI";
        }
    }
}
