using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogConnectNotify
    {
        [BsonId]
        public string id { get; set; }
        public string connectid { get; set; }
        public string ma_doi_tac { get; set; }
        public string nsd { get; set; }
        public long tg_ket_noi { get; set; }
        public long tg_ngat_ket_noi { get; set; }
        public int tthai { get; set; }
        public string tthai_hthi { get; set; }
        public LogConnectNotify()
        {
            id = Guid.NewGuid().ToString("N");
        }
    }
}
