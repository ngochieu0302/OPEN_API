using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.OpenID
{
    public class cache_search
    {
        public string key { get; set; }
        public string search { get; set; }
        public string database { get; set; }
        public string schema { get; set; }
        public int? db_index { get; set; }
        public cache_search()
        {
            db_index = 0;
        }
    }
}
