using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogContent
    {
        [BsonId]
        public string id { get; set; }
        public long time { get; set; }
        public string content { get; set; }
        public LogContent()
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }
        public LogContent(string content)
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            this.content = content;
        }
    }
}
