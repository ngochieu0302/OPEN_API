using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.CallApp
{
    public class AnswerResponse
    {
        public string action { get; set; }
        public AnswerPhone from { get; set; }
        public AnswerPhone to { get; set; }
        public string customData { get; set; }
        public AnswerResponse(string from, string to, bool appToApp = true)
        {
            action = "connect";
            this.from = new AnswerPhone(from);
            if (appToApp)
            {
                this.to = new AnswerPhone(to);
            }
            else
            {
                this.to = new AnswerPhone(to, "external");
            }
            this.customData = "test-custom-data";
        }
    }
    public class AnswerPhone
    {
        public string type { get; set; }
        public string number { get; set; }
        public string alias { get; set; }
        public AnswerPhone(string phone, string type = "internal")
        {
            this.type = type;
            this.number = phone;
            this.alias = phone;
        }
    }
}
