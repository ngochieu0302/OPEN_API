using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_action
	{
		public Nullable<decimal> actionid { get; set; }
		public string action_name { get; set; }
		public string action_type { get; set; }
		public string parent_actioncode { get; set; }
		public Nullable<decimal> order_exc { get; set; }
		public string is_async { get; set; }
		public string type_cache { get; set; }
		public Nullable<decimal> max_rq_cache { get; set; }
		public Nullable<decimal> max_time_cache { get; set; }
		public Nullable<decimal> time_live_cache { get; set; }
		public string type_ddos { get; set; }
		public Nullable<decimal> max_rq_ddos { get; set; }
		public Nullable<decimal> max_time_ddos { get; set; }
		public Nullable<decimal> time_lock { get; set; }
		public string json_input { get; set; }
		public string json_output { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public byte[] gid { get; set; }
		public string actions_clear_cache { get; set; }
		public string send_notify { get; set; }
		public string check_authen { get; set; }
		public string is_file { get; set; }
		public string create_file { get; set; }
		public string create_file_sign { get; set; }
		public string remove_file { get; set; }
		public string is_internal { get; set; }
	}
}
