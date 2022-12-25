using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS_SERVICE.Model.Param
{
    public class pbh_tich_hop_api_log_nh_param
    {
        public string service_ma_doi_tac { get; set; }
        public string service_token { get; set; }
        public string service_nsd { get; set; }
        public string service_pas { get; set; }

        public string ma_doi_tac { get; set; }
        public string nsd { get; set; }
        public string ma_api { get; set; }
        public string data_request { get; set; }
        public string data_response { get; set; }
        public string nd_tim { get; set; }
    }
}
