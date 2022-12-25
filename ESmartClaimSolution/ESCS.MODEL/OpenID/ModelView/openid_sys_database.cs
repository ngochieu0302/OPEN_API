using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_sys_database
    {
		public Nullable<decimal> db_id { get; set; }
		public string db_dbname { get; set; }
		public string db_descriptions { get; set; }
		public string db_cat_db { get; set; }
		public Nullable<decimal> db_set_default { get; set; }
		public Nullable<decimal> db_createdate { get; set; }
		public string db_createby { get; set; }
		public Nullable<decimal> db_updatedate { get; set; }
		public string db_updateby { get; set; }
		public Nullable<decimal> db_isactive { get; set; }
		public Nullable<decimal> cf_id { get; set; }
		public Nullable<decimal> cf_database_id { get; set; }
		public string cf_dbname { get; set; }
		public string cf_servername { get; set; }
		public string cf_port { get; set; }
		public string cf_connectstring { get; set; }
		public string cf_sid { get; set; }
		public string cf_service_name { get; set; }
		public Nullable<decimal> cf_use_sid { get; set; }
		public string cf_envcode { get; set; }
		public Nullable<decimal> cf_createdate { get; set; }
		public string cf_createby { get; set; }
		public Nullable<decimal> cf_updatedate { get; set; }
		public string cf_updateby { get; set; }
		public Nullable<decimal> cf_isactive { get; set; }
	}
}
