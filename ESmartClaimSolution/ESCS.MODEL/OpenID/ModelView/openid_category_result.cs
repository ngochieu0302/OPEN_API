using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID.ModelView
{
    public class openid_category_result
    {
        public IEnumerable<openid_sys_server> server { get; set; }
        public IEnumerable<openid_sys_database> database { get; set; }
        public IEnumerable<openid_sys_schema> schema { get; set; }
    }
}
