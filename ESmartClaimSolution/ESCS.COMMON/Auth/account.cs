using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Auth
{
    /// <summary>
    /// Login OpenId
    /// </summary>
    public class account
    {
        public string partner_code { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    public class account_change_pass
    {
        public string partner_code { get; set; }
        public string username { get; set; }
        public string old_password { get; set; }
        public string new_password { get; set; }
        public string re_new_password { get; set; }
    }
}
