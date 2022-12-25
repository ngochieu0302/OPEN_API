using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.Hubs
{
    public interface IUserConnectionManager
    {
        void KeepUserConnection(MessageNotify messageNotify);
        void RemoveUserConnection(string connectionId);
        List<string> GetUserConnections(string userId);
    }
    public class UserConnectionManager : IUserConnectionManager
    {
        private readonly IMongoDBContext _context;
        public UserConnectionManager(IMongoDBContext context)
        {
            _context = context;
        }
        public virtual IMongoDBService<B> GetMongoService<B>() where B : class
        {
            try
            {
                return (MongoDBService<B>)typeof(MongoDBService<B>).GetConstructor(new Type[] { typeof(MongoDBContext) }).Invoke(new object[] { this._context });
            }
            catch
            {
                return default(MongoDBService<B>);
            }
        }
        private static Dictionary<string, List<string>> userConnectionMap = new Dictionary<string, List<string>>();
        private static string userConnectionMapLocker = string.Empty;


        public List<string> GetUserConnections(string userId)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                if (userConnectionMap==null|| !userConnectionMap.ContainsKey(userId))
                {
                    return null;
                }
                conn = userConnectionMap[userId];
            }
            return conn;
        }
        public void KeepUserConnection(MessageNotify messageNotify)
        {
            lock (userConnectionMapLocker)
            {
                if (string.IsNullOrEmpty(messageNotify.ma_doi_tac)|| string.IsNullOrEmpty(messageNotify.nsd))
                {
                    return;
                }
                string key = messageNotify.ma_doi_tac +"/"+ messageNotify.nsd;



                if (!userConnectionMap.ContainsKey(key))
                {
                    userConnectionMap[key] = new List<string>();
                }
                userConnectionMap[key].Add(messageNotify.connectionid);
            }
        }
        public void RemoveUserConnection(string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                foreach (var userId in userConnectionMap.Keys)
                {
                    if (userConnectionMap.ContainsKey(userId))
                    {
                        if (userConnectionMap[userId].Contains(connectionId))
                        {
                            userConnectionMap[userId].Remove(connectionId);
                            break;
                        }
                    }
                }
            }
        }
    }
}
