using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogResponse
    {
        public string id { get; set; }
        public string schema { get; set; }
        public string host { get; set; }
        public string path { get; set; }
        public string query_string { get; set; }
        public string body { get; set; }
        public long time_response { get; set; }
        public Dictionary<string, string> headers { get; set; }
        public LogResponse()
        {
            time_response = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            headers = new Dictionary<string, string>();
        }
    }
}
