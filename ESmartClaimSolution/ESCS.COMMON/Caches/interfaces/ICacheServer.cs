using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Caches.interfaces
{
    public interface ICacheServer
    {
        RedisValueWithExpiry StringGetWithExpiry(string serverName, string endpoint, string key, int database = 0);
        double GetExpiryTimeWithConfigDB(string key);
        T Get<T>(string serverName, string endpoint, string key, int database = 0);
        T GetWithConfigDB<T>(string key);
        bool Set<T>(string serverName, string endpoint, String key, T item, TimeSpan? expiry = null, int database = 0);
        bool SetWithConfigDB<T>(String key, T item, TimeSpan? expiry = null);
        bool Remove(string serverName, string endpoint, string key, int database = 0);
        bool RemoveKeyCacheByPattern(string endpoint, string pattern, int database = 0);
        bool RemoveWithConfigDB(string key);
        bool Expired(string serverName, string endpoint, string key, TimeSpan expiry, int database = 0);
        bool RemoveKeyCacheByPatternWithServerName(string serverName, string endpoint, string pattern, int database = 0);
        List<string> GetKeysByPatterm(string endpoint, string pattern, int database = 0);
    }
}
