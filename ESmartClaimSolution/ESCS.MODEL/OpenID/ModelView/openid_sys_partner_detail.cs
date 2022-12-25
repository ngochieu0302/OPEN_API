using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_sys_partner_detail
	{
        public openid_sys_partner partner { get; set; }
        public List<openid_sys_partner_config> partner_config { get; set; }
    }
}
