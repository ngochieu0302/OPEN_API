using ESCS.COMMON.CallApp;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogCallApp
    {
        [BsonId]
        public string id { get; set; }
        public long time { get; set; }
        public List<AnswerResponse> answers { get; set; }
        public LogCallApp()
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }
        public LogCallApp(List<AnswerResponse> answers)
        {
            id = Guid.NewGuid().ToString("N");
            time = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            this.answers = answers;
        }
    }
}
