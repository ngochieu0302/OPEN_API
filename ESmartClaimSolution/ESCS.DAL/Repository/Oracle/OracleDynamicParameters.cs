using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ESCS.DAL.Repository.Oracle
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private static Dictionary<Dapper.SqlMapper.Identity, Action<IDbCommand, object>> paramReaderCache = new Dictionary<SqlMapper.Identity, Action<IDbCommand, object>>();
        public Dictionary<string, ParamInfo> parameters = new Dictionary<string, ParamInfo>();
        private List<object> templates;
        public OracleDynamicParameters()
        {
        }
        public OracleDynamicParameters(object template)
        {
            AddDynamicParams(template);
        }
        public void AddDynamicParams(dynamic param)
        {
            var obj = param as object;
            if (obj != null)
            {
                var subDynamic = obj as OracleDynamicParameters;
                if (subDynamic == null)
                {
                    var dictionary = obj as IEnumerable<KeyValuePair<string, object>>;
                    if (dictionary == null)
                    {
                        templates = templates ?? new List<object>();
                        templates.Add(obj);
                    }
                    else
                    {
                        foreach (var kvp in dictionary)
                        {
                            Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                else
                {
                    if (subDynamic.parameters != null)
                    {
                        foreach (var kvp in subDynamic.parameters)
                        {
                            parameters.Add(kvp.Key, kvp.Value);
                        }
                    }

                    if (subDynamic.templates != null)
                    {
                        templates = templates ?? new List<object>();
                        foreach (var t in subDynamic.templates)
                        {
                            templates.Add(t);
                        }
                    }
                }
            }
        }
        public void Add(string name, object value = null, OracleDbType? dbType = null, ParameterDirection? direction = null, int? size = null, bool isArray = false, int? arrayBindSize = null)
        {
            parameters[Clean(name)] = new ParamInfo() { Name = name, Value = value, ParameterDirection = direction ?? ParameterDirection.Input, DbType = dbType, Size = size, IsArray = isArray, ArrayBindSize = arrayBindSize };
        }
        private static string Clean(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                switch (name[0])
                {
                    case '@':
                    case ':':
                    case '?':
                        return name.Substring(1);
                }
            }
            return name;
        }
        void SqlMapper.IDynamicParameters.AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            AddParameters(command, identity);
        }
        protected void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            if (templates != null)
            {
                foreach (var template in templates)
                {
                    var newIdent = identity.ForDynamicParameters(template.GetType());
                    Action<IDbCommand, object> appender;

                    lock (paramReaderCache)
                    {
                        if (!paramReaderCache.TryGetValue(newIdent, out appender))
                        {
                            appender = SqlMapper.CreateParamInfoGenerator(newIdent, false, false);
                            paramReaderCache[newIdent] = appender;
                        }
                    }

                    appender(command, template);
                }
            }

            foreach (var param in parameters.Values)
            {
                string name = Clean(param.Name);
                bool add = !((OracleCommand)command).Parameters.Contains(name);
                OracleParameter p;
                if (add)
                {
                    p = ((OracleCommand)command).CreateParameter();
                    p.ParameterName = name;
                }
                else
                {
                    p = ((OracleCommand)command).Parameters[name];
                }
                var val = param.Value;
                if (!param.IsArray)
                {
                    p.Value = val ?? DBNull.Value;
                }
                else
                {
                    p.Value = string.Empty;
                    p.Value = param.Value ?? DBNull.Value;
                }
                p.Direction = param.ParameterDirection;
                var s = val as string;
                if (s != null)
                {
                    if (s.Length <= 4000)
                    {
                        p.Size = 4000;
                    }
                }
                else
                {
                    if (param.ParameterDirection == ParameterDirection.InputOutput || param.ParameterDirection == ParameterDirection.Output)
                    {
                        if (param.DbType == OracleDbType.Char || param.DbType == OracleDbType.Varchar2 || param.DbType == OracleDbType.NVarchar2)
                        {
                            p.Size = 4000;
                        }
                        else
                        {
                            p.Size = int.MaxValue;
                        }
                    }
                }
                if (param.Size != null)
                {
                    p.Size = param.Size.Value;
                }
                if (param.DbType != null)
                {
                    p.OracleDbType = param.DbType.Value;
                }
                if (add)
                {
                    command.Parameters.Add(p);
                    if (param.IsArray)
                    {
                        p.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                        if (param.ArrayBindSize != null && param.ArrayBindSize != 0)
                        {
                            p.ArrayBindSize = new int[param.ArrayBindSize.Value];
                            for (int i = 0; i < p.ArrayBindSize.Count(); i++)
                            {
                                p.ArrayBindSize[i] = int.MaxValue;
                            }
                        }
                        else if (param.ArrayBindSize == 0)
                        {
                            p.Value = new object[1] { DBNull.Value };
                            p.ArrayBindSize = new int[0];
                            p.Size = 0;
                        }
                        else
                        {
                            p.ArrayBindSize = new int[1000000];
                            p.Value = new object[1] { DBNull.Value };
                            p.Size = 0;
                        }
                    }
                }
                param.AttachedParam = p;
            }
        }
        public IEnumerable<string> ParameterNames
        {
            get
            {
                return parameters.Select(p => p.Key);
            }
        }
        public T Get<T>(string name)
        {
            var val = parameters[Clean(name)].AttachedParam.Value;
            if (val == DBNull.Value)
            {
                if (default(T) != null)
                {
                    throw new ApplicationException("Attempting to cast a DBNull to a non nullable type!");
                }
                return default(T);
            }
            return (T)val;
        }
    }
}
