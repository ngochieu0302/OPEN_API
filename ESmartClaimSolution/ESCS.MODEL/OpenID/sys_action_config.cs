using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_action_config
	{
		public decimal actionid { get; set; }
		public string envcode { get; set; }
		public Nullable<decimal> id_server_cache { get; set; }
		public Nullable<decimal> id_server_file { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public string db_cache { get; set; }
		public string prefix_key_cache { get; set; }
		public string cache_connection_name { get; set; }
		public string cache_port { get; set; }
		public string cache_password { get; set; }
		public string param_cache { get; set; }
	}
}
