using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ESCS.DAL.Repository.Oracle
{
    public class ParamInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection ParameterDirection { get; set; }
        public OracleDbType? DbType { get; set; }
        public int? Size { get; set; }
        public IDbDataParameter AttachedParam { get; set; }
        public bool IsArray { get; set; }
        public int? ArrayBindSize { get; set; }
    }
}
