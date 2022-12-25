using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_connection
	{
        public string tk_ma { get; set; }
        public string gid { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string access_token { get; set; }
		public Nullable<long> time_connect { get; set; }
		public Nullable<decimal> time_logout { get; set; }
		public Nullable<long> time_exprive_access_token { get; set; }
		public string refresh_token { get; set; }
		public Nullable<long> time_exprive_refresh_token { get; set; }
		public Nullable<int> time_remaining { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public string partner_code { get; set; }
        public string ip_connect { get; set; }
        public string envcode { get; set; }
        public string parent_access_token { get; set; }
		public Nullable<decimal> time_refresh { get; set; }
	}
}
