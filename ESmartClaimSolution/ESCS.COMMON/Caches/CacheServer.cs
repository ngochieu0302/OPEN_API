using ESCS.COMMON.Caches.interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.Caches
{
    public class CacheServer : ICacheServer
    {
        public TimeSpan _defaultExpiry;
        public string _serverName;
        public int _dataBase;
        private RedisServer _redisServer;
        public T Get<T>(string serverName, string endpoint, string key, int database = 0)
        {
            try
            {
                var value =  RedisConnectorHelper.Connection.GetDatabase(database).StringGet(key);
                if (value == RedisValue.Null)
                {
                    return default(T);
                }
                if (typeof(T).Equals(typeof(string)))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
        public bool Set<T>(string serverName, string endpoint, String key, T item, TimeSpan? expiry = null, int database = 0)
        {
            try
            {
                _redisServer = new RedisServer(serverName, endpoint);
                if (typeof(T).Equals(typeof(string)))
                {
                    RedisConnectorHelper.Connection.GetDatabase(database).StringSet(key, item?.ToString(), expiry);
                }
                else
                {
                    RedisConnectorHelper.Connection.GetDatabase(database).StringSet(key, JsonConvert.SerializeObject(item), expiry);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool Remove(string serverName, string endpoint, string key, int database = 0)
        {
            var result = RedisConnectorHelper.Connection.GetDatabase(database).KeyDelete(key);
            if (RedisCacheMaster.WriteLog)
            {
                try
                {
                    if (!string.IsNullOrEmpty(RedisCacheMaster.FolderLog) && !Directory.Exists(RedisCacheMaster.FolderLog))
                        System.IO.Directory.CreateDirectory(RedisCacheMaster.FolderLog);
                    var path = Path.Combine(RedisCacheMaster.FolderLog, "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                    if (!File.Exists(path))
                        File.Create(path).Close();
                    var endpoints = RedisConnectorHelper.Connection.GetEndPoints();
                    var server = RedisConnectorHelper.Connection.GetServer(endpoints.First());
                    var count = server.Keys().Count();
                    var log = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " - Key remove: " + key + " - Count key: " + count+ Environment.NewLine;
                    File.AppendAllText(path, log);
                }
                catch { }
            }
            return result;
        }

        [Obsolete]
        public bool RemoveKeyCacheByPattern(string endpoint, string pattern, int database = 0)
        {
            _redisServer = new RedisServer("", endpoint);
            var keys = _redisServer.GetAllKeyServerOfDatabase(endpoint, pattern, database);
            bool delSuccess = true;
            if (keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    try
                    {
                        bool check = _redisServer.GetDatabase(database).KeyDelete(key);
                        if (RedisCacheMaster.WriteLog)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(RedisCacheMaster.FolderLog) && !Directory.Exists(RedisCacheMaster.FolderLog))
                                    System.IO.Directory.CreateDirectory(RedisCacheMaster.FolderLog);
                                var path = Path.Combine(RedisCacheMaster.FolderLog, "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                                if (!File.Exists(path))
                                    File.Create(path).Close();
                                var endpoints = _redisServer.GetConnection().GetEndPoints();
                                var server = _redisServer.GetConnection().GetServer(endpoints.First());
                                var count = server.Keys().Count();
                                var log = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " - Key remove: " + key + " - Count key: " + count + Environment.NewLine;
                                File.AppendAllText(path, log);
                            }
                            catch { }
                        }
                        if (!check)
                        {
                            delSuccess = false;
                        }
                    }
                    catch { };
                }
            }
            return delSuccess;
        }


        public bool Expired(string serverName, string endpoint, string key, TimeSpan expiry, int database = 0)
        {
            return RedisConnectorHelper.Connection.GetDatabase(database).KeyExpire(key, expiry);
        }
        public RedisValueWithExpiry StringGetWithExpiry(string serverName, string endpoint, string key, int database = 0)
        {
            return RedisConnectorHelper.Connection.GetDatabase(database).StringGetWithExpiry(key, CommandFlags.None);
        }
        public double GetExpiryTimeWithConfigDB(string key)
        {
            var res = RedisConnectorHelper.Connection.GetDatabase(RedisCacheEnvironment.Database).StringGetWithExpiry(key, CommandFlags.None);
            double expireTime = res.Expiry.Value.TotalMinutes;
            return expireTime;
        }
        public bool SetWithConfigDB<T>(string key, T item, TimeSpan? expiry = null)
        {
            try
            {
                if (typeof(T).Equals(typeof(string)))
                {
                    RedisConnectorHelper.Connection.GetDatabase(RedisCacheEnvironment.Database).StringSet(key, item?.ToString(), expiry);
                }
                else
                {
                    RedisConnectorHelper.Connection.GetDatabase(RedisCacheEnvironment.Database).StringSet(key, JsonConvert.SerializeObject(item), expiry);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool RemoveWithConfigDB(string key)
        {
            return RedisConnectorHelper.Connection.GetDatabase(RedisCacheEnvironment.Database).KeyDelete(key);
        }
        public T GetWithConfigDB<T>(string key)
        {
            try
            {
                var value = RedisConnectorHelper.Connection.GetDatabase(RedisCacheEnvironment.Database).StringGet(key);
                if (value == RedisValue.Null)
                {
                    return default(T);
                }
                if (typeof(T).Equals(typeof(string)))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
        public List<string> GetKeysByPatterm(string endpoint, string pattern, int database = 0)
        {
            _redisServer = new RedisServer("", endpoint);
            var keys = _redisServer.GetAllKeyServerOfDatabase(endpoint, pattern, database);
            if (keys==null)
                return null;
            return keys.Select(n => n.ToString()).ToList();
        }
        public bool RemoveKeyCacheByPatternWithServerName(string serverName, string endpoint, string pattern, int database = 0)
        {
            _redisServer = new RedisServer(serverName, endpoint);
            var keys = _redisServer.GetAllKeyServerOfDatabase(endpoint, pattern, database);
            bool delSuccess = true;
            if (keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    bool check = _redisServer.GetDatabase(database).KeyDelete(key);
                    if (!check)
                    {
                        delSuccess = false;
                    }
                }
            }
            return delSuccess;
        }
    }
}
