using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class MailConfig
    {
        public string type { get; set; }
        public string title { get; set; }
        public string mailto { get; set; }
        public string bcc { get; set; }
    }
}
