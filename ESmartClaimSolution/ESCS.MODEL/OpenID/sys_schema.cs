using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_schema
	{
		public decimal id { get; set; }
		public Nullable<decimal> database_id { get; set; }
		public string schema { get; set; }
		public string descriptions { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public Nullable<decimal> set_default { get; set; }
	}
}
