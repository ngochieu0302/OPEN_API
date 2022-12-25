using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_sys_action
    {
        public int r__ { get; set; }
        public string envcode { get; set; }
        public decimal? trang { get; set; }
        public decimal? so_dong { get; set; }
        public string search { get; set; }
        public string clear_cache_name { get; set; }
        #region action
        public Nullable<decimal> ac_actionid { get; set; }
		public string ac_action_name { get; set; }
		public string ac_action_type { get; set; }
		public string ac_parent_actioncode { get; set; }
		public Nullable<decimal> ac_order_exc { get; set; }
		public string ac_is_async { get; set; }
		public string ac_type_cache { get; set; }
		public Nullable<decimal> ac_max_rq_cache { get; set; }
		public Nullable<decimal> ac_max_time_cache { get; set; }
		public Nullable<decimal> ac_time_live_cache { get; set; }
		public string ac_type_ddos { get; set; }
		public Nullable<decimal> ac_max_rq_ddos { get; set; }
		public Nullable<decimal> ac_max_time_ddos { get; set; }
		public Nullable<decimal> ac_time_lock { get; set; }
		public string ac_json_input { get; set; }
		public string ac_json_output { get; set; }
		public Nullable<decimal> ac_createdate { get; set; }
		public string ac_createby { get; set; }
		public Nullable<decimal> ac_updatedate { get; set; }
		public string ac_updateby { get; set; }
		public Nullable<decimal> ac_isactive { get; set; }
		public string ac_actions_clear_cache { get; set; }
		public string ac_send_notify { get; set; }
		public string ac_check_authen { get; set; }
		public string ac_is_file{ get; set; }
		public string ac_create_file{ get; set; }
		public string ac_create_file_sign{ get; set; }
		public string ac_remove_file { get; set; }
		public string ac_is_internal { get; set; }
		#endregion
		#region action config
		public Nullable<decimal> acf_actionid { get; set; }
		public string acf_envcode { get; set; }
		public Nullable<decimal> acf_id_server_cache { get; set; }
		public Nullable<decimal> acf_id_server_file { get; set; }
		public Nullable<decimal> acf_createdate { get; set; }
		public string acf_createby { get; set; }
		public Nullable<decimal> acf_updatedate { get; set; }
		public string acf_updateby { get; set; }
		public Nullable<decimal> acf_isactive { get; set; }
		public string acf_db_cache { get; set; }
		public string acf_prefix_key_cache { get; set; }
		public string acf_cache_connection_name { get; set; }
		public string acf_cache_port { get; set; }
		public string acf_cache_password { get; set; }
		public string acf_param_cache { get; set; }
		#endregion
		#region excute
		public string exc_createby { get; set; }
		public Nullable<decimal> exc_updatedate { get; set; }
		public string exc_updateby { get; set; }
		public Nullable<decimal> exc_isactive { get; set; }
		public string exc_isactive_text { get; set; }
		public string exc_type_exec { get; set; }
		public Nullable<decimal> exc_actionid { get; set; }
		public string exc_actioncode { get; set; }
		public string exc_package_name { get; set; }
		public string exc_storedprocedure { get; set; }
		public decimal exc_schema_id { get; set; }
		public Nullable<decimal> exc_createdate { get; set; }
        public string exc_actioncode_source { get; set; }
        #endregion
        #region file
        public string file_actioncode { get; set; }
        public string file_package_name { get; set; }
        public string file_storedprocedure { get; set; }
        public decimal? file_schema_id { get; set; }
        public string file_type_exec { get; set; }
        public decimal? file_actionid { get; set; }
		public string file_actioncode_source { get; set; }
		public string file_ip_remote { get; set; }
		public string file_base_folder { get; set; }
		public string file_user_remote { get; set; }
		public string file_pas_remote { get; set; }
		public decimal? file_is_local { get; set; }
		public string file_type_file { get; set; }
		public decimal? file_change_size { get; set; }
		public decimal? file_max_content_length { get; set; }
		public decimal? file_max_width { get; set; }
		public decimal? file_is_duplicate_mini { get; set; }
		public decimal? file_max_width_file_mini { get; set; }
		public decimal? file_max_content_mini { get; set; }
		public string file_prefix_mini { get; set; }
		public string file_extensions_file { get; set; }
		public decimal? file_isactive { get; set; }
		public decimal? file_createdate { get; set; }
		public decimal? file_updatedate { get; set; }
		public string file_createby { get; set; }
		public string file_updateby { get; set; }
        #endregion
        #region mail
        public string mail_actioncode { get; set; }
        public string mail_package_name { get; set; }
        public string mail_storedprocedure { get; set; }
        public string mail_type_exec { get; set; }
        public decimal? mail_actionid { get; set; }
        public string mail_actioncode_source { get; set; }
		public decimal? mail_schema_id { get; set; }
		public string mail_ip_remote { get; set; }
		public string mail_base_folder { get; set; }
		public string mail_user_remote { get; set; }
		public string mail_pas_remote { get; set; }
		public decimal? mail_is_local { get; set; }
		public string mail_type_file { get; set; }
		public decimal? mail_is_attach_file { get; set; }
		public decimal? mail_createdate { get; set; }
		public string mail_createby { get; set; }
		public decimal? mail_updatedate { get; set; }
		public string mail_updateby { get; set; }
		public decimal? mail_isactive { get; set; }
		#endregion
		#region database
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
		#endregion
		#region database config
		public Nullable<decimal> dbcf_id { get; set; }
		public Nullable<decimal> dbcf_database_id { get; set; }
		public string dbcf_dbname { get; set; }
		public string dbcf_servername { get; set; }
		public string dbcf_port { get; set; }
		public string dbcf_connectstring { get; set; }
		public string dbcf_sid { get; set; }
		public string dbcf_service_name { get; set; }
		public Nullable<decimal> dbcf_use_sid { get; set; }
		public string dbcf_envcode { get; set; }
		public Nullable<decimal> dbcf_createdate { get; set; }
		public string dbcf_createby { get; set; }
		public Nullable<decimal> dbcf_updatedate { get; set; }
		public string dbcf_updateby { get; set; }
		public Nullable<decimal> dbcf_isactive { get; set; }
		#endregion
		#region schema
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
		#endregion
		#region schema config
		public Nullable<decimal> scf_id { get; set; }
		public Nullable<decimal> scf_schema_id { get; set; }
		public string scf_envcode { get; set; }
		public string scf_username { get; set; }
		public string scf_password { get; set; }
		public Nullable<decimal> scf_createdate { get; set; }
		public string scf_createby { get; set; }
		public Nullable<decimal> scf_updatedate { get; set; }
		public string scf_updateby { get; set; }
		public Nullable<decimal> scf_isactive { get; set; }
        #endregion
        
    }
}
