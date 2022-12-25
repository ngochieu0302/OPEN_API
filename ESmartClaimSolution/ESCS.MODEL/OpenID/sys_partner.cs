using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
	public partial class sys_partner
	{
		public string code { get; set; }
		public string name { get; set; }
		public string parent_code { get; set; }
		public string organization { get; set; }
		public string companyname { get; set; }
		public string ceo_name { get; set; }
		public string address { get; set; }
		public string email { get; set; }
		public string phone { get; set; }
		public string taxcode { get; set; }
		public string contractno { get; set; }
		public string description { get; set; }
		public string website { get; set; }
		public string cat_partner { get; set; }
		public Nullable<decimal> effectivedate { get; set; }
		public Nullable<decimal> expirationdate { get; set; }
		public Nullable<decimal> createdate { get; set; }
		public string createby { get; set; }
		public Nullable<decimal> updatedate { get; set; }
		public string updateby { get; set; }
		public Nullable<decimal> isactive { get; set; }
		public Nullable<decimal> ismaster { get; set; }
		public string action_login { get; set; }
	}
}
