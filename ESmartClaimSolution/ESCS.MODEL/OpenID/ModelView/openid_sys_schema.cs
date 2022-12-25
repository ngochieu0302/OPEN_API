using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_sys_schema
    {
		public Nullable<decimal> sc_id { get; set; }
		public Nullable<decimal> sc_database_id { get; set; }
		public string sc_schema { get; set; }
		public string sc_descriptions { get; set; }
		public Nullable<decimal> sc_createdate { get; set; }
		public string sc_createby { get; set; }
		public Nullable<decimal> sc_updatedate { get; set; }
		public string sc_updateby { get; set; }
		public Nullable<decimal> sc_isactive { get; set; }
		public Nullable<decimal> sc_set_default { get; set; }

		public Nullable<decimal> cf_id { get; set; }
		public Nullable<decimal> cf_schema_id { get; set; }
		public string cf_envcode { get; set; }
		public string cf_username { get; set; }
		public string cf_password { get; set; }
		public Nullable<decimal> cf_createdate { get; set; }
		public string cf_createby { get; set; }
		public Nullable<decimal> cf_updatedate { get; set; }
		public string cf_updateby { get; set; }
		public Nullable<decimal> cf_isactive { get; set; }
		
	}
}
