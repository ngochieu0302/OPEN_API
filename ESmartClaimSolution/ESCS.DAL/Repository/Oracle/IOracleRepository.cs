using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ESCS.DAL.Repository.Oracle
{
    public interface IOracleRepository<T> where T : class
    {
        T ExcuteSingle(string storeName, IDynamicParameters dyParam);
        Task<T> ExcuteSingleAsync(string storeName, IDynamicParameters dyParam);
        object ExcuteScalar(string storeName, IDynamicParameters dyParam);
        Task<object> ExcuteScalarAsync(string storeName, IDynamicParameters dyParam);
        IEnumerable<T> ExcuteMany(string storeName, IDynamicParameters dyParam);
        Task<IEnumerable<T>> ExcuteManyAsync(string storeName, IDynamicParameters dyParam);
        int ExcuteNoneQuery(string storeName, IDynamicParameters dyParam);
        Task<int> ExcuteNoneQueryAsync(string storeName, IDynamicParameters dyParam);
        void ExcuteMultiple(string storeName, IDynamicParameters dyParam, Action<GridReader> action);
        Task ExcuteMultipleAsync(string storeName, IDynamicParameters dyParam, Action<GridReader> action);
        IDbConnection GetConnection();
    }
}
