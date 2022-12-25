using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_action_check
    {
        public string action_type { get; set; }
        public string actioncode { get; set; }
        public string exc_package_name { get; set; }
        public string exc_storedprocedure { get; set; }
        public Nullable<decimal> exc_schema_id { get; set; }
    }
}
