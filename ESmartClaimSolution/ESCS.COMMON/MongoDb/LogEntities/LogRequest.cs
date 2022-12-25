using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogRequestSearch : LogRequest
    {
        public int? trang { get; set; }
        public int? so_dong { get; set; }
        public string tim { get; set; }
    }
    public class LogRequest
    {
        public string id { get; set; }
        public string schema { get; set; }
        public string host { get; set; }
        public string path { get; set; }
        public string query_string { get; set; }
        public string body { get; set; }
        public long time_request { get; set; }
        public Dictionary<string, string> headers { get; set; }
        public LogRequest()
        {
            time_request = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            headers = new Dictionary<string, string>();
        }
    }
}
