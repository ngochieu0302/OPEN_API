using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Caches
{
    public class RedisCacheMaster
    {
        public static string ConnectionName { get; set; }
        public static string Host { get; set; }
        public static int Port { get; set; }
        public static string Password { get; set; }
        public static string Endpoint { get; set; }
        public static int DatabaseIndex { get; set; }
        public static bool WriteLog { get; set; }
        public static string FolderLog { get; set; }
    }
}
