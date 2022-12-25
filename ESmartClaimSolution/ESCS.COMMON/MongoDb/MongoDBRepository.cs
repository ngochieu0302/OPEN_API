using ESCS.COMMON.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.MongoDb
{
    public interface IMongoDBRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> Add(TEntity obj);
        Task<TEntity> GetById(string id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Update(Expression<Func<TEntity, bool>> expression, TEntity obj);
        Task<bool> Remove(string id);
        Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> expression);
        PagedCollection<TEntity> GetPaging(Expression<Func<TEntity, bool>> expression, int pageindex = 1, int pagesize = 20);
        Task CreateIndex(params Expression<Func<TEntity, object>>[] fields);
        IMongoQueryable<TEntity> All { get; }
    }
    public abstract class MongoDBRepository<TEntity> : IMongoDBRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoDatabase Database;
        protected readonly IMongoCollection<TEntity> DbSet;
        public IMongoQueryable<TEntity> All
        {
            get { return DbSet.AsQueryable(); }
        }
        public MongoDBRepository(IMongoDBContext context)
        {
            Database = context.Database;
            DbSet = Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }
        public virtual async Task<TEntity> Add(TEntity obj)
        {
            try
            {
                await DbSet.InsertOneAsync(obj);
                return obj;
            }
            catch
            {
                return null;
            }
        }
        public virtual async Task<TEntity> GetById(string id)
        {
            var data = await DbSet.Find(FilterId(id)).SingleOrDefaultAsync();
            return data;
        }
        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }
        public async virtual Task<TEntity> Update(Expression<Func<TEntity, bool>> expression, TEntity obj)
        {
            await DbSet.ReplaceOneAsync(expression, obj);
            return obj;
        }
        public async virtual Task<bool> Remove(string id)
        {
            var result = await DbSet.DeleteOneAsync(FilterId(id));
            return result.IsAcknowledged;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        private static FilterDefinition<TEntity> FilterId(string key)
        {
            return Builders<TEntity>.Filter.Eq("_id", key);
        }
        public virtual async Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> expression = null)
        {
            var filter = Builders<TEntity>.Filter.Empty;
            if (expression!=null)
            {
                filter = Builders<TEntity>.Filter.Where(expression);
            }
            var all = await DbSet.FindAsync(filter);
            return all.ToList();
        }
        [Obsolete]
        public PagedCollection<TEntity> GetPaging(Expression<Func<TEntity, bool>> expression, int pageindex = 1, int pagesize = 20)
        {
            var filter = Builders<TEntity>.Filter.Empty;
            if (expression != null)
            {
                filter = Builders<TEntity>.Filter.Where(expression);
            }
            PagedCollection<TEntity> pagedCollection = new PagedCollection<TEntity>();
            pagedCollection.tong_so_dong = DbSet.Count(filter) > 0 ? Convert.ToInt64(DbSet.Count(filter).ToString()) : 0;
            pagedCollection.data =  DbSet.Find(filter).Limit(pagesize).Skip(pagesize * (pageindex - 1)).ToList();
            pagedCollection.trang = pageindex;
            pagedCollection.so_dong_tren_trang = pagesize;
            return pagedCollection;
        }
        [Obsolete]
        public async Task CreateIndex(params Expression<Func<TEntity, object>>[] fields)
        {
            List<IndexKeysDefinition<TEntity>> keys = new List<IndexKeysDefinition<TEntity>>();
            foreach (var field in fields)
            {
                keys.Add(Builders<TEntity>.IndexKeys.Ascending(field));
            }
            if (keys!=null && keys.Count>0)
            {
                var indexDefinition = Builders<TEntity>.IndexKeys.Combine(keys.ToArray());
                await DbSet.Indexes.CreateOneAsync(indexDefinition);
            }
        }
    }
    public interface IMongoDBContext
    {
        IMongoDatabase Database { get; }
    }
    public class MongoDBContext : IMongoDBContext
    {
        public MongoDBContext()
        {
            var client = new MongoClient(MongoConnection.ConnectionString);
            if (client != null)
                Database = client.GetDatabase(MongoConnection.Database);
        }
        public IMongoDatabase Database { get; }
    }
    public class PagedCollection<T>
    {
        public IEnumerable<T> data;
        public long tong_so_dong;
        public long so_dong_tren_trang;
        public long trang;
    }
}
