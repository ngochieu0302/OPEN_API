using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class kb_hs_btcn_result
    {
        public List<kb_hs_btcn_result_table> Table { get; set; }
    }
    public class kb_hs_btcn_result_table
    {
        public string SO_ID { get; set; }
        public string VALUE_OUT { get; set; }
        public string EMAIL_TB { get; set; }
        public string EMAIL_BCC { get; set; }
    }
}
