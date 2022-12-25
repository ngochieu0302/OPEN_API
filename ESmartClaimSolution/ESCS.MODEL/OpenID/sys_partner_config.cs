using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_partner_config
	{
		public decimal id { get; set; }
		public string partner_code { get; set; }
		public string envcode { get; set; }
		public string ip_cors { get; set; }
		public string token { get; set; }
		public string secret_key { get; set; }
		public string password { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public string username { get; set; }
        public long? sesstion_time_live { get; set; }
		public string username_cms { get; set; }
		public string password_cms { get; set; }
		public string blacklist_ip { get; set; }
	}
}
