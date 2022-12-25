using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class CoreApiConfig
    {
        public static List<ApiConfig> Items { get; set; }
    }
    public class CoreApiConfigContants
    {
        public const string MIC = "MIC";
        public const string OPES = "OPES";
        public const string PJICO = "PJICO";
        public const string DIGINS = "DIGINS";
        public const string TMIV = "TMIV";
    }
    public class ApiConfig
    {
        public string Partner { get; set; }
        public string Domain { get; set; }
        public string Domain2 { get; set; }
        public string Secret { get; set; }
        public string DomainMailSMS { get; set; }
        public string SecretMailSMS { get; set; }
        public string NsdSMS { get; set; }
        public string PassSMS { get; set; }
        public string DomainNotify { get; set; }
        public EndPointConfig EndPoint { get; set; }
        public AuthenNotifyConfig AuthenNotify { get; set; }
    }
    public class EndPointConfig
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Partner { get; set; }
        public string Password { get; set; }
    }
    public class AuthenNotifyConfig 
    {
        public string Token { get; set; }
    }

}
