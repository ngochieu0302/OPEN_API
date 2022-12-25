using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.MongoDb
{
    public interface IMongoDBService<TEntity> : IMongoDBRepository<TEntity> where TEntity : class
    {
    }
    public class MongoDBService<TEntity> : MongoDBRepository<TEntity>, IMongoDBService<TEntity> where TEntity : class
    {
        public MongoDBService(IMongoDBContext context) : base(context)
        {

        }
    }
}
