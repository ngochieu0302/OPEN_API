using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_action_mail
	{
		public string actioncode { get; set; }
		public string package_name { get; set; }
		public string storedprocedure { get; set; }
		public Nullable<decimal> schema_id { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public string type_exec { get; set; }
		public Nullable<decimal> actionid { get; set; }
		public string actioncode_source { get; set; }
		public string ip_remote { get; set; }
		public string base_folder { get; set; }
		public string user_remote { get; set; }
		public string pas_remote { get; set; }
		public Nullable<decimal> is_local { get; set; }
		public string type_file { get; set; }
		public Nullable<decimal> is_attach_file { get; set; }
	}
}
