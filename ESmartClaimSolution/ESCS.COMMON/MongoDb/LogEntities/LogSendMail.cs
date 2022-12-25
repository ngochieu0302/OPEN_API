using ESCS.COMMON.Common;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogSendMail
    {
        [BsonId]
        public string id { get; set; }
        public long time { get; set; }
        public string title { get; set; }
        public EmailConfig config { get; set; }
        public string body { get; set; }
        public string status { get; set; }
        public LogSendMail()
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }
        public LogSendMail(string title, string body, EmailConfig config, string status)
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            this.title = title;
            this.body = body;
            this.config = config;
            this.status = status;
        }
    }
}
