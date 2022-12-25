using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class OpenIDConfig
    {
        public static string ConnectString { get; set; }
        public static string DbName { get; set; }
        public static string Schema { get; set; }
        public static string RedisOrMemoryCache { get; set; }
        public static int TimeLiveAccessTokenMinute { get; set; }
        public static int TimeCantUseRefreshToken { get; set; }
        public static int TimeLiveDataCacheMinute { get; set; }
        public static int CommandTimeOut { get; set; }
        public static string KeyHashPayloadToken { get; set; }
        public static bool IsWriteLog { get; set; }
        public static bool EncryptStoredProcedure { get; set; }
        public static LowercaseContractResolver LowercaseContractResolver { get; set; }
    }
}
