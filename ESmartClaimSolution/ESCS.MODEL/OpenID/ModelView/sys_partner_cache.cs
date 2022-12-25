using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class sys_partner_cache
    {
        public string key_cache { get; set; }
        public string partner_code { get; set; }
        public string partner_name { get; set; }
        public string partner_parent_code { get; set; }
        public string partner_organization { get; set; }
        public string partner_companyname { get; set; }
        public string partner_ceo_name { get; set; }
        public string partner_address { get; set; }
        public string partner_email { get; set; }
        public string partner_phone { get; set; }
        public string partner_taxcode { get; set; }
        public string partner_contractno { get; set; }
        public string partner_description { get; set; }
        public string partner_website { get; set; }
        public string partner_cat_partner { get; set; }
        public long? partner_effectivedate { get; set; }
        public long? partner_expirationdate { get; set; }
        public long? partner_createdate { get; set; }
        public string partner_createby { get; set; }
        public long? partner_updatedate { get; set; }
        public string partner_updateby { get; set; }
        public int? partner_isactive { get; set; }
        public int? partner_ismaster { get; set; }
        public string partner_action_login { get; set; }

        public long? config_id { get; set; }
        public string config_partner_code { get; set; }
        public string config_envcode { get; set; }
        public string config_ip_cors { get; set; }
        public string config_token { get; set; }
        public string config_secret_key { get; set; }
        public string config_password { get; set; }
        public long? config_createdate { get; set; }
        public string config_createby { get; set; }
        public long? config_updatedate { get; set; }
        public string config_updateby { get; set; }
        public int? config_isactive { get; set; }
        public string config_username { get; set; }
        public int? config_session_time_live { get; set; }
        public string config_username_cms { get; set; }
        public string config_password_cms { get; set; }
        public string config_blacklist_ip { get; set; }
    }
}
