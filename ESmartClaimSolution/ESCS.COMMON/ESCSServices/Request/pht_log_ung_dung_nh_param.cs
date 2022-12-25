using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.ESCSServices.Request
{
    public class pht_log_ung_dung_nh_param
    {
        public string service_ma_doi_tac { get; set; }
        public string service_token { get; set; }
        public string service_nsd { get; set; }
        public string service_pas { get; set; }

		public string code { get; set; }
		public string scheme { get; set; }
		public string host { get; set; }
		public string path { get; set; }
		public string query_string { get; set; }
		public string headers { get; set; }
		public string body_request { get; set; }
		public string body_response { get; set; }
		public string body_search { get; set; }
		public long time_request { get; set; }
		public long time_response { get; set; }
	}
}
