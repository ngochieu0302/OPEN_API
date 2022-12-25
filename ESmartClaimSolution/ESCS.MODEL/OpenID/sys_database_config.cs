using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_database_config
	{
		public decimal id { get; set; }
		public Nullable<decimal> database_id { get; set; }
		public string dbname { get; set; }
		public string servername { get; set; }
		public string port { get; set; }
		public string connectstring { get; set; }
		public string sid { get; set; }
		public string service_name { get; set; }
		public Nullable<decimal> use_sid { get; set; }
		public string envcode { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
	}
}
