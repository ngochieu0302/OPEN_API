using ESCS.COMMON.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.MongoDb
{
    public interface ILogMongoService<T> where T : class
    {
        void AddLogAsync(T obj);
        Task<T> UpdateLogAsync(Expression<Func<T, bool>> expression, T obj);
        Task<bool> RemoveLogAsync(string id);
        Task<T> GetByIdAsync(string id);
    }
    public class LogMongoService<T> : ILogMongoService<T> where T : class
    {
        private ILogMongoRepository<T> _logRequestRepository;
        public LogMongoService(ILogMongoRepository<T> logRequestRepository)
        {
            _logRequestRepository = logRequestRepository;
        }
        public void AddLogAsync(T obj)
        {
            if (!MongoConnection.UseMongo)
                return;
            Task task = new Task(() =>
            {
                try
                {
                     _logRequestRepository.Add(obj);
                }
                catch
                {
                }
            });
            task.Start();
        }
        public Task<T> UpdateLogAsync(Expression<Func<T, bool>> expression, T obj)
        {
            if (!MongoConnection.UseMongo)
                return null;
            return _logRequestRepository.Update(expression, obj);
        }
        public Task<bool> RemoveLogAsync(string id)
        {
            if (!MongoConnection.UseMongo)
                return Task.FromResult(false);
            return _logRequestRepository.Remove(id);
        }
        public Task<T> GetByIdAsync(string id)
        {
            if (!MongoConnection.UseMongo)
                return null;
            return _logRequestRepository.GetById(id);
        }
    }
}
