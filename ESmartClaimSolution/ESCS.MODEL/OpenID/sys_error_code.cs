using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_error_code
	{
        public string r__ { get; set; }
        public byte[] gid { get; set; }
		public string client_error_code { get; set; }
		public string server_error_code { get; set; }
		public string error_message { get; set; }
		public string package_name { get; set; }
		public string storedprocedure { get; set; }
		public Nullable<decimal> schema_id { get; set; }
		public Nullable<decimal> db_id { get; set; }
		public Nullable<decimal> server_id { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public string isactive_text { get; set; }
	}
}
