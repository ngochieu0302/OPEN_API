using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_environment
	{
		public string envname { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public string envcode { get; set; }
	}
}
