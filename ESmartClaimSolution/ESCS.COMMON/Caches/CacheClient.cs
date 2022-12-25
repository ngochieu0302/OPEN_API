using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Caches
{
    public interface ICacheClient
    {
        T Get<T>(string key, string host, int port, string password, long db = 0);
        void Set<T>(string key, T value, string host, int port, string password, TimeSpan timeSpan, long db = 0);
    }
    public class CacheClient : ICacheClient
    {
        public T Get<T>(string key, string host, int port, string password, long db = 0)
        {
            using (RedisClient redisClient = new RedisClient(host, port, password, db))
            {
                return redisClient.Get<T>(key);
            }
        }
        public void Set<T>(string key, T value, string host, int port, string password, TimeSpan timeSpan, long db = 0)
        {
            using (RedisClient redisClient = new RedisClient(host, port, password, db))
            {
                redisClient.Set<T>(key, value, timeSpan);
            }
        }



        public bool IsKeyExists(string key, string host, int port, string password, long db = 0)
        {
            using (var redisClient = new RedisClient(host, port, password, db))
            {
                if (redisClient.ContainsKey(key))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        

    }
}
