using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_sys_email
    {
		public Nullable<decimal> mail_id { get; set; }
		public string mail_name { get; set; }
		public string mail_smtp_server { get; set; }
		public string mail_smtp_username { get; set; }
		public string mail_smtp_password { get; set; }
		public Nullable<decimal> mail_smtp_port { get; set; }
		public Nullable<decimal> mail_is_use_proxy { get; set; }
		public string mail_proxy_server { get; set; }
		public string mail_proxy_username { get; set; }
		public string mail_proxy_password { get; set; }
		public string mail_envcode { get; set; }
		public string mail_partner_code { get; set; }
		public Nullable<decimal> mail_is_default { get; set; }
		public Nullable<decimal> mail_createdate { get; set; }
		public string mail_createby { get; set; }
		public Nullable<decimal> mail_updatedate { get; set; }
		public string mail_updateby { get; set; }
		public Nullable<decimal> mail_isactive { get; set; }

		public Nullable<decimal> tpl_id { get; set; }
		public string tpl_ten { get; set; }
		public string tpl_layout { get; set; }
		public string tpl_content { get; set; }
		public string tpl_envcode { get; set; }
		public string tpl_partner_code { get; set; }
		public Nullable<decimal> tpl_createdate { get; set; }
		public string tpl_createby { get; set; }
		public Nullable<decimal> tpl_updatedate { get; set; }
		public string tpl_updateby { get; set; }
		public Nullable<decimal> tpl_isactive { get; set; }
	}
}
