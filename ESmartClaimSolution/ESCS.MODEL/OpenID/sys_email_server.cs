using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_email_server
	{
		public Nullable<decimal> id { get; set; }
		public string name { get; set; }
		public string smtp_server { get; set; }
		public string smtp_username { get; set; }
		public string smtp_password { get; set; }
		public Nullable<decimal> smtp_port { get; set; }
		public Nullable<decimal> is_use_proxy { get; set; }
		public string proxy_server { get; set; }
		public string proxy_username { get; set; }
		public string proxy_password { get; set; }
		public string envcode { get; set; }
		public string partner_code { get; set; }
		public Nullable<decimal> is_default { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
	}
}
