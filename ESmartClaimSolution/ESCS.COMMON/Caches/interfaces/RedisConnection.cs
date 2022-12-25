using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Caches.interfaces
{
    public static class RedisConnection
    {
        public static readonly string conn = "";
        private static RedisCacheClient _securityDatabase;

        public static ICacheClient Connection
        {
            get
            {
                if (_securityDatabase == null)
                {
                    _securityDatabase = new RedisCacheClient(conn);
                }
                return _securityDatabase;
            }
        }
    }
}
