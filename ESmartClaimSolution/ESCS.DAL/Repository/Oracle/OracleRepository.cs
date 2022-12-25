using Dapper;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExceptionHandlers;
using ESCS.COMMON.ExtensionMethods;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ESCS.DAL.Repository.Oracle
{
    
    public class OracleRepository<T> : IOracleRepository<T> where T : class
    {
        private readonly string connectionString;
        private readonly ICacheServer _cacheServer;
        public OracleRepository(string _connectionString)
        {
            connectionString = _connectionString;
            _cacheServer = new CacheServer();
        }
        public virtual T ExcuteSingle(string storeName, IDynamicParameters dyParam)
        {
            T result = null;
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        result = SqlMapper.QuerySingleOrDefault<T>(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return result;
        }
        public virtual async Task<T> ExcuteSingleAsync(string storeName, IDynamicParameters dyParam)
        {
            T result = null;
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        result = await SqlMapper.QuerySingleOrDefaultAsync<T>(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure, commandTimeout: OpenIDConfig.CommandTimeOut);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return result;
        }
        public virtual object ExcuteScalar(string storeName, IDynamicParameters dyParam)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        return SqlMapper.ExecuteScalar(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return null;
        }
        public virtual async Task<object> ExcuteScalarAsync(string storeName, IDynamicParameters dyParam)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        return await SqlMapper.ExecuteScalarAsync(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure, commandTimeout: OpenIDConfig.CommandTimeOut);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return null;
        }
        public virtual IEnumerable<T> ExcuteMany(string storeName, IDynamicParameters dyParam)
        {
            List<T> result = new List<T>();
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        var data = SqlMapper.Query<T>(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure);
                        if (data != null)
                        {
                            result = data.ToList();
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return result;
        }
        public virtual async Task<IEnumerable<T>> ExcuteManyAsync(string storeName, IDynamicParameters dyParam)
        {
            List<T> result = new List<T>();
            try
            {

                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        var data = await SqlMapper.QueryAsync<T>(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure, commandTimeout: OpenIDConfig.CommandTimeOut);
                        if (data != null)
                        {
                            result = data.ToList();
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return result;
        }
        public async Task ExcuteReaderAsync(string storeName, OracleDynamicParameters dyParam, Action<IDataReader,List<string>> readData)
        {
            var listcursor = new List<string>();
            if (dyParam != null && dyParam.parameters != null && dyParam.parameters.Count > 0)
            {
                listcursor = dyParam.parameters.Where(n => n.Key.ToLower().StartsWith("cur_") || n.Key.ToLower().StartsWith("curs_")).Select(n => n.Key.ToLower()).ToList();
            }
            if (listcursor == null)
                listcursor = new List<string>();
            
            using (var conn = GetConnection())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    try
                    {
                        var rd = await SqlMapper.ExecuteReaderAsync(cnn: conn, sql: storeName, param: dyParam, commandType: CommandType.StoredProcedure, commandTimeout: OpenIDConfig.CommandTimeOut);
                        readData(rd, listcursor);
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new OracleDbException(ex);
                    }
                }
                conn.Close();
            }
        }
        public virtual int ExcuteNoneQuery(string storeName, IDynamicParameters dyParam)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        int result = SqlMapper.Execute(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure);
                        return result;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return 0;
        }
        public virtual async Task<int> ExcuteNoneQueryAsync(string storeName, IDynamicParameters dyParam)
        {
            int result = -1;
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        result = await SqlMapper.ExecuteAsync(conn, storeName, param: dyParam, commandType: CommandType.StoredProcedure, commandTimeout: OpenIDConfig.CommandTimeOut);
                        return result;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            return result;
        }
        public virtual void ExcuteMultiple(string storeName, IDynamicParameters dyParam, Action<GridReader> action)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        using (var multi = conn.QueryMultiple(storeName, dyParam, null, null, CommandType.StoredProcedure))
                        {
                            action(multi);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public virtual async Task ExcuteMultipleAsync(string storeName, IDynamicParameters dyParam, Action<GridReader> action)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        using (var multi = await conn.QueryMultipleAsync(storeName, dyParam, null, OpenIDConfig.CommandTimeOut, CommandType.StoredProcedure))
                        {
                            action(multi);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }

        }
        public IDbConnection GetConnection()
        {
            if (connectionString != null)
            {
                var conn = new OracleConnection(connectionString);
                return conn;
            }
            return null;
        }
        #region Method mapping parameter
        public List<OpenIDOracleParameter> SyncParamWithValue(string stored_procedure, string schema, string b_stored_name)
        {
            var _params = new OracleDynamicParameters();
            _params.Add("b_owner", schema, OracleDbType.Varchar2, ParameterDirection.Input);
            _params.Add("b_stored_name", b_stored_name, OracleDbType.Varchar2, ParameterDirection.Input);
            _params.Add("cur_common", null, OracleDbType.RefCursor, ParameterDirection.Output);
            List<OpenIDOracleParameter> result = new List<OpenIDOracleParameter>();
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        var data = SqlMapper.Query<OpenIDOracleParameter>(conn, stored_procedure, param: _params, commandType: CommandType.StoredProcedure);
                        if (data != null)
                        {
                            result = data.ToList();
                        }
                    }
                    conn.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public List<OpenIDOracleParameter> SyncParamWithValueByQuery(string schema, string pkg_name, string b_sp_name)
        {
            var _params = new OracleDynamicParameters();
            _params.Add("b_schema", schema, OracleDbType.Varchar2, ParameterDirection.Input);
            _params.Add("b_pkg_name", pkg_name, OracleDbType.Varchar2, ParameterDirection.Input);
            _params.Add("b_sp_name", b_sp_name, OracleDbType.Varchar2, ParameterDirection.Input);
            string query = @"select LOWER(argument_name) as ArgumentName,
                                CASE
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_VAR' THEN 'VARCHAR2'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_LVAR' THEN 'VARCHAR2'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_NVAR' THEN 'NVARCHAR2'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_LNVAR' THEN 'NVARCHAR2'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_NCLOB' THEN 'NCLOB'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_BLOB' THEN 'BLOB'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_NUM' THEN 'NUMBER'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_BOOL' THEN 'BOOLEAN'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_DATE' THEN 'DATE'
                                 WHEN UPPER(data_type) = 'PL/SQL TABLE' and UPPER(type_subname) = 'ARR_NGAY' THEN 'NUMBER'
                                 ELSE UPPER(data_type)
                               END as TypeOracle,
                               CASE WHEN UPPER(data_type) = 'PL/SQL TABLE' THEN 1 ELSE 0 END as IsArray,
                               Position,
                               UPPER(in_out) as InOut,

                               LOWER(case when :b_pkg_name is null then :b_sp_name else :b_pkg_name||'.'||:b_sp_name end) as StoredName
                            from ALL_ARGUMENTS where LOWER(owner) = LOWER(:b_schema)
                            and LOWER(object_name) = LOWER(:b_sp_name)
                            and (:b_pkg_name is null OR (:b_pkg_name is not null AND LOWER(PACKAGE_NAME)=LOWER(:b_pkg_name)))
                            and argument_name is not null
                            order by position";
            List<OpenIDOracleParameter> result = new List<OpenIDOracleParameter>();
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        var data = SqlMapper.Query<OpenIDOracleParameter>(conn, query, param: _params, null, true, 500, commandType: CommandType.Text);
                        if (data != null)
                        {
                            result = data.ToList();
                        }
                    }
                    conn.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        /// <summary>
        /// Mapping dùng model
        /// </summary>
        /// <param name="stored_procedure"></param>
        /// <param name="schema"></param>
        /// <param name="b_stored_name"></param>
        /// <param name="param_value"></param>
        /// <returns></returns>
        public OracleDynamicParameters GetParamWithValue(string stored_procedure, string schema, string b_stored_name, object param_value)
        {
            var _params = new OracleDynamicParameters();
            _params.Add("b_owner", schema, OracleDbType.Varchar2, ParameterDirection.Input);
            _params.Add("b_stored_name", b_stored_name, OracleDbType.Varchar2, ParameterDirection.Input);
            _params.Add("cur_common", null, OracleDbType.RefCursor, ParameterDirection.Output);
            List<OpenIDOracleParameter> result = new List<OpenIDOracleParameter>();
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        var data = SqlMapper.Query<OpenIDOracleParameter>(conn, stored_procedure, param: _params, commandType: CommandType.StoredProcedure);
                        if (data != null)
                        {
                            result = data.ToList();
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
            result.ForEach(n => n.GetCsType());
            OracleDynamicParameters result_type = new OracleDynamicParameters();
            bool implementICollection = param_value.GetType().GetInterfaces()
                            .Any(x => x.IsGenericType &&
                            x.GetGenericTypeDefinition() == typeof(ICollection<>));
            if (param_value != null && param_value.GetType().GetTypeInfo().IsClass && !implementICollection)
            {
                foreach (var item in result)
                {
                    if (item.IsArray == 0)
                    {
                        string property_name = item.ArgumentName.StartsWith("b_") ? item.ArgumentName.Replace("b_", "") : item.ArgumentName;
                        object value = param_value.GetType().GetProperty(property_name)?.GetValue(param_value, null);
                        result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                    }
                    else
                    {
                        string property_name = item.ArgumentName.StartsWith("a_") ? item.ArgumentName.Replace("a_", "") : item.ArgumentName;
                        result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs, null, true);
                    }
                }
            }
            else if (param_value != null && param_value.GetType().GetTypeInfo().IsClass && implementICollection)
            {
                foreach (var item in result)
                {
                    if (item.IsArray == 1)
                    {
                        string property_name = item.ArgumentName.StartsWith("a_") ? item.ArgumentName.Replace("a_", "") : item.ArgumentName;
                        result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs, null, true);
                    }
                    else
                    {
                        string property_name = item.ArgumentName.StartsWith("b_") ? item.ArgumentName.Replace("b_", "") : item.ArgumentName;
                        object value = param_value.GetType().GetProperty(property_name)?.GetValue(param_value, null);
                        result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                    }
                }
            }
            return result_type;
        }
        public OracleDynamicParameters GetParamWithValueByQuery(string dbname, string schema, string pkg_name, string b_stored_name, object param_value)
        {
            List<OpenIDOracleParameter> result = new List<OpenIDOracleParameter>();
            string keyCache = CachePrefixKeyConstants.GetKeyCacheParamStored(dbname, schema, b_stored_name, pkg_name);
            string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            if (!string.IsNullOrEmpty(json))
            {
                result = JsonConvert.DeserializeObject<List<OpenIDOracleParameter>>(json);
            }
            else
            {
                result = SyncParamWithValueByQuery(schema, pkg_name, b_stored_name);
                result.ForEach(n => n.GetCsType());
                _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(10) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }
            
            OracleDynamicParameters result_type = new OracleDynamicParameters();
            bool implementICollection = param_value.GetType().GetInterfaces()
                            .Any(x => x.IsGenericType &&
                            x.GetGenericTypeDefinition() == typeof(ICollection<>));
            if (param_value != null && param_value.GetType().GetTypeInfo().IsClass && !implementICollection)
            {
                foreach (var item in result)
                {
                    if (item.IsArray == 0)
                    {
                        string property_name = item.ArgumentName;
                        if (item.ArgumentName.StartsWith("b_"))
                        {
                            property_name = item.ArgumentName.Substring(2, item.ArgumentName.Length - 2);
                        }
                        object value = param_value.GetType().GetProperty(property_name)?.GetValue(param_value, null);
                        result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                    }
                    else
                    {
                        string property_name = item.ArgumentName.StartsWith("a_") ? item.ArgumentName.Replace("a_", "") : item.ArgumentName;
                        result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs, null, true);
                    }
                }
            }
            else if (param_value != null && param_value.GetType().GetTypeInfo().IsClass && implementICollection)
            {
                foreach (var item in result)
                {
                    if (item.IsArray == 1)
                    {
                        string property_name = item.ArgumentName.StartsWith("a_") ? item.ArgumentName.Replace("a_", "") : item.ArgumentName;
                        result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs, null, true);
                    }
                    else
                    {
                        string property_name = item.ArgumentName.StartsWith("b_") ? item.ArgumentName.Replace("b_", "") : item.ArgumentName;
                        object value = param_value.GetType().GetProperty(property_name)?.GetValue(param_value, null);
                        result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                    }
                }
            }
            return result_type;
        }
        public OracleDynamicParameters GetParamWithValueByQueryNew(string dbname, string schema, string pkg_name, string b_stored_name, IDictionary<string,object> param_value)
        {
            List<OpenIDOracleParameter> result = new List<OpenIDOracleParameter>();
            string keyCache = CachePrefixKeyConstants.GetKeyCacheParamStored(dbname, schema, b_stored_name, pkg_name);
            string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            if (!string.IsNullOrEmpty(json))
            {
                result = JsonConvert.DeserializeObject<List<OpenIDOracleParameter>>(Utilities.DecryptByKey(json, OpenIDConstants.VERSION));
            }
            else
            {
                result = SyncParamWithValueByQuery(schema, pkg_name, b_stored_name);
                result.ForEach(n => n.GetCsType());
                _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, Utilities.DecryptByKey(JsonConvert.SerializeObject(result), OpenIDConstants.VERSION), DateTime.Now.AddDays(10) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }
            OracleDynamicParameters result_type = new OracleDynamicParameters();
            foreach (var item in result)
            {
                object value = DBNull.Value;
                int? size = null;
                if (item.IsArray == 1)
                {
                    value = param_value.GetValue(item.ArgumentName, out size, item.TypeCs);
                }
                else
                {
                    value = param_value.GetValue(item.ArgumentName, out size, item.TypeCs);
                    
                }
                if (value == null || value.ToString() == "")
                {
                    result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs, null, item.IsArray == 1);
                }
                else
                {
                    result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs, null, item.IsArray == 1, size);
                }
            }
            return result_type;
        }
        /// <summary>
        /// Mapping không dùng model
        /// </summary>
        /// <param name="stored_procedure"></param>
        /// <param name="schema"></param>
        /// <param name="b_stored_name"></param>
        /// <param name="param_value"></param>
        /// <returns></returns>
        public OracleDynamicParameters GetParamWithValue(string stored_procedure, string schema, string b_stored_name, Dictionary<string, string> param_value)
        {
            List<OpenIDOracleParameter> result = new List<OpenIDOracleParameter>();
            if (OracleRepositoryConstant.listParam != null)
            {
                if (OracleRepositoryConstant.listParam.ContainsKey(b_stored_name))
                {
                    result = JsonConvert.DeserializeObject<List<OpenIDOracleParameter>>(OracleRepositoryConstant.listParam[b_stored_name].ToString());
                }
                else
                {
                    result = SyncParamWithValue(stored_procedure, schema, b_stored_name);
                    OracleRepositoryConstant.listParam.Add(b_stored_name, JsonConvert.SerializeObject(result));
                }
            }
            else
            {
                OracleRepositoryConstant.listParam = new Hashtable();
                result = SyncParamWithValue(stored_procedure, schema, b_stored_name);
                OracleRepositoryConstant.listParam.Add(b_stored_name, JsonConvert.SerializeObject(result));
            }

            result.ForEach(n => n.GetCsType());
            OracleDynamicParameters result_type = new OracleDynamicParameters();
            if (param_value != null)
            {
                foreach (var item in result)
                {
                    if (item.IsArray == 0)
                    {
                        string property_name = item.ArgumentName.StartsWith("b_") ? item.ArgumentName.Substring(2, item.ArgumentName.Length - 2) : item.ArgumentName;
                        if (item.TypeCs == OracleDbType.Decimal)
                        {
                            decimal? value = (!param_value.ContainsKey(property_name) || string.IsNullOrEmpty(param_value[property_name])) ? null : (decimal?)Convert.ToDecimal(param_value[property_name]);
                            if (value != null)
                            {
                                result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                            }
                            else
                            {
                                result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                            }

                        }
                        else if (item.TypeCs == OracleDbType.Date)
                        {
                            DateTime? value = (!param_value.ContainsKey(property_name) || string.IsNullOrEmpty(param_value[property_name])) ? null : (DateTime?)DateTime.ParseExact(param_value[property_name], OracleRepositoryConstant.FORMAT_DATE, CultureInfo.InvariantCulture);
                            if (value != null)
                            {
                                result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                            }
                            else
                            {
                                result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                            }
                        }
                        else
                        {
                            string value = (!param_value.ContainsKey(property_name) || string.IsNullOrEmpty(param_value[property_name])) ? null : param_value[property_name];
                            if (value != null)
                            {
                                result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                            }
                            else
                            {
                                result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                            }
                        }
                    }
                    else
                    {
                        string property_name = item.ArgumentName.StartsWith("a_") ? item.ArgumentName.Substring(2, item.ArgumentName.Length - 2) : item.ArgumentName;
                        result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs, null, true);
                    }
                }
            }
            else
            {
                foreach (var item in result)
                {
                    result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                }
            }
            return result_type;
        }
        public OracleDynamicParameters GetParamWithValueByQuery(string dbname, string schema, string pkg_name, string b_stored_name, Dictionary<string, string> param_value)
        {
            List<OpenIDOracleParameter> result = new List<OpenIDOracleParameter>();
            string keyCache = CachePrefixKeyConstants.GetKeyCacheParamStored(dbname, schema, b_stored_name, pkg_name);
            string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint,keyCache, RedisCacheMaster.DatabaseIndex);
            if (!string.IsNullOrEmpty(json))
            {
                result = JsonConvert.DeserializeObject<List<OpenIDOracleParameter>>(json);
            }
            else
            {
                result = SyncParamWithValueByQuery(schema, pkg_name, b_stored_name);
                result.ForEach(n => n.GetCsType());
                _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(10) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }
            
            OracleDynamicParameters result_type = new OracleDynamicParameters();
            if (param_value != null)
            {
                foreach (var item in result)
                {
                    if (item.IsArray == 0)
                    {
                        string property_name = item.ArgumentName.StartsWith("b_") ? item.ArgumentName.Substring(2, item.ArgumentName.Length - 2) : item.ArgumentName;
                        if (item.TypeCs == OracleDbType.Decimal)
                        {
                            decimal? value = (!param_value.ContainsKey(property_name) || string.IsNullOrEmpty(param_value[property_name])) ? null : (decimal?)Convert.ToDecimal(param_value[property_name]);
                            if (value != null)
                            {
                                result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                            }
                            else
                            {
                                result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                            }

                        }
                        else if (item.TypeCs == OracleDbType.Date)
                        {
                            DateTime? value = (!param_value.ContainsKey(property_name) || string.IsNullOrEmpty(param_value[property_name])) ? null : (DateTime?)DateTime.ParseExact(param_value[property_name], OracleRepositoryConstant.FORMAT_DATE, CultureInfo.InvariantCulture);
                            if (value != null)
                            {
                                result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                            }
                            else
                            {
                                result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                            }
                        }
                        else
                        {
                            string value = (!param_value.ContainsKey(property_name) || string.IsNullOrEmpty(param_value[property_name])) ? null : param_value[property_name];
                            if (value != null)
                            {
                                result_type.Add(item.ArgumentName, value, item.TypeCs, item.InOutCs);
                            }
                            else
                            {
                                result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                            }
                        }
                    }
                    else
                    {
                        string property_name = item.ArgumentName.StartsWith("a_") ? item.ArgumentName.Substring(2, item.ArgumentName.Length - 2) : item.ArgumentName;
                        result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs, null, true);
                    }
                }
            }
            else
            {
                foreach (var item in result)
                {
                    result_type.Add(item.ArgumentName, DBNull.Value, item.TypeCs, item.InOutCs);
                }
            }
            return result_type;
        }
        #endregion
        private string GenerateKeyCacheParamStored(string prefix, string dbname, string schema, string package, string stored)
        {
            if (string.IsNullOrEmpty(package))
                return prefix + "." + dbname.ToUpper() + "." + schema.ToUpper() + "." + stored.ToUpper();
            else
                return prefix + "." + dbname.ToUpper() + "." + schema.ToUpper() + "." + package.ToUpper() + "." + stored.ToUpper();
        }
    }
}
