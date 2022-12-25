using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogException
    {
        public string type { get; set; }
        public string exception { get; set; }
        public long createdate { get; set; }
        public bool isreaded { get; set; }
        public LogException()
        {

        }
        public LogException(string _type, string _exception)
        {
            createdate = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            type = _type;
            exception = _exception;
            isreaded = false;
        }
    }
}
