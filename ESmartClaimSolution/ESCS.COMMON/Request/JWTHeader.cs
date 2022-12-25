using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class JWTHeader
    {
        public string typ { get; set; }
        public string alg { get; set; }
        public string cty { get; set; }
        public string partner { get; set; }
        public string cat { get; set; }//public, private
        public string token { get; set; }
        public string action { get; set; }
        public string user { get; set; }
        public string envcode { get; set; }
    }
}
