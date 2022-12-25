using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_action_file
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
		public Nullable<decimal> change_size { get; set; }
		public Nullable<decimal> max_content_length { get; set; }
		public Nullable<decimal> max_width { get; set; }
		public Nullable<decimal> is_duplicate_mini { get; set; }
		public Nullable<decimal> max_width_file_mini { get; set; }
		public Nullable<decimal> max_content_mini { get; set; }
		public string prefix_mini { get; set; }
		public string extensions_file { get; set; }
	}
}
