using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb
{
    public class MongoConnection
    {
        public static string ConnectionString { get; set; }
        public static string Database { get; set; }
        public static bool UseMongo { get; set; }
        public static bool UseLog { get; set; }
    }
}
