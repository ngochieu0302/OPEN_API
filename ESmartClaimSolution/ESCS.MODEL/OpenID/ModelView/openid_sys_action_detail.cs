using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_sys_action_detail
    {
        public sys_action action { get; set; }
        public sys_action_config action_config { get; set; }
        public sys_action_exc_db action_exc_db { get; set; }
        public sys_action_file action_file { get; set; }
        public sys_action_mail action_mail { get; set; }
    }
}
