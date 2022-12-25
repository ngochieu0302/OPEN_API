using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.Caches
{
    public class RedisServer
    {
        public string _serverName;
        private readonly string _endPoints;
        private ConnectionMultiplexer _connection;

        public RedisServer(string serverName, string endPoints)
        {
            _serverName = serverName;
            _endPoints = endPoints;
        }
        public ConnectionMultiplexer GetConnection()
        {
            if (_connection == null)
            {
                _connection = ConnectionMultiplexer.Connect(_endPoints);
            }
            return _connection;
        }
        public void Close()
        {
            try
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                _connection = null;
            }
            finally
            {
                _connection = null;
            }
        }
        ~RedisServer()
        {
            Close();
        }
        public IDatabase GetDatabase(int database = 0)
        {
            return GetConnection().GetDatabase(database);
        }

        public IServer GetServer(string host, int port)
        {
            return GetConnection().GetServer(host, port);
        }

        public RedisKey[] GetAllKeyServerOfDatabase(string endPoint, string pattern, int database = 0)
        {
            GetConnection();
            var host = endPoint.Split(',')[0].Split(':')[0];
            var port = Convert.ToInt32(endPoint.Split(',')[0].Split(':')[1]);
            IDatabase db = _connection.GetDatabase(database);
            return _connection.GetServer(host, port).Keys(database: db.Database, pattern: pattern+"*")?.ToArray();
        }
    }
}
