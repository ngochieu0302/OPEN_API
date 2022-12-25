using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class SignatureFileConfig
    {
        public static bool Enable { get; set; }
        public static bool Online { get; set; }
        public static string Partner { get; set; }
        public static string UrlSignature { get; set; }
        public static string UrlToken { get; set; }
        public static string UsernameToken { get; set; }
        public static string PassToken { get; set; }
    }
}
