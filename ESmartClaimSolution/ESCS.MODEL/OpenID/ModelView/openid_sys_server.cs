using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_sys_server
    {
		public Nullable<decimal> sv_id { get; set; }
		public string sv_servername { get; set; }
		public string sv_cat_server { get; set; }
		public Nullable<decimal> sv_createdate { get; set; }
		public string sv_createby { get; set; }
		public Nullable<decimal> sv_updatedate { get; set; }
		public string sv_updateby { get; set; }
		public Nullable<decimal> sv_isactive { get; set; }
		public Nullable<decimal> cf_id { get; set; }
		public Nullable<decimal> cf_server_id { get; set; }
		public string cf_envcode { get; set; }
		public string cf_server_ip { get; set; }
		public Nullable<decimal> cf_createdate { get; set; }
		public string cf_createby { get; set; }
		public Nullable<decimal> cf_updatedate { get; set; }
		public string cf_updateby { get; set; }
		public Nullable<decimal> cf_isactive { get; set; }
		public Nullable<decimal> cf_set_default { get; set; }
	}
}
