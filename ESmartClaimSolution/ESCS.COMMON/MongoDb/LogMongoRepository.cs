using ESCS.COMMON.MongoDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.MongoDb
{
    public interface ILogMongoRepository<TEntity> : IMongoDBRepository<TEntity> where TEntity : class
    {
    }
    public class LogMongoRepository<TEntity> : MongoDBRepository<TEntity>, ILogMongoRepository<TEntity> where TEntity : class
    {
        public LogMongoRepository(IMongoDBContext context):base(context)
        {

        }
    }
}
