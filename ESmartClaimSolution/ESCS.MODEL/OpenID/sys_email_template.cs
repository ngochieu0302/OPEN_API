using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_email_template
	{
		public Nullable<decimal> id { get; set; }
		public string ten { get; set; }
		public string layout { get; set; }
		public string content { get; set; }
		public string envcode { get; set; }
		public string partner_code { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
	}
}
