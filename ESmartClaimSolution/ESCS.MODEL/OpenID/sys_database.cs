using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_database
	{
		public decimal id { get; set; }
		public string dbname { get; set; }
		public string descriptions { get; set; }
		public string cat_db { get; set; }
		public Nullable<decimal> set_default { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
	}
}
