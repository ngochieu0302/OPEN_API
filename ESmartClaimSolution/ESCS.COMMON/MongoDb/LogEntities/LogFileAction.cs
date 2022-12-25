using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb.LogEntities
{
    public class LogFileAction
    {
        [BsonId]
        public long id { get; set; }
        public string ma_doi_tac { get; set; }
        public string so_id { get; set; }
        public string create_file { get; set; }
        public string create_file_sign { get; set; }
        public string remove_file { get; set; }
        public string message { get; set; }
    }
}
