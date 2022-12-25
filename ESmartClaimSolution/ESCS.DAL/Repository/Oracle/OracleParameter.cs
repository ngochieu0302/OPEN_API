using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ESCS.DAL.Repository.Oracle
{

    public class OpenIDOracleParameter
    {
        public string ArgumentName { get; set; }
        public string TypeOracle { get; set; }
        public Nullable<int> Position { get; set; }
        public string InOut { get; set; }
        public OracleDbType TypeCs { get; set; }
        public ParameterDirection InOutCs { get; set; }
        public Nullable<int> IsArray { get; set; }
        public Nullable<DateTime> TimeChange { get; set; }
        public string StoredName { get; set; }
        public void GetCsType()
        {
            switch (InOut)
            {
                case "IN":
                    this.InOutCs = ParameterDirection.Input;
                    break;
                case "OUT":
                    this.InOutCs = ParameterDirection.Output;
                    break;
                case "IN/OUT":
                    this.InOutCs = ParameterDirection.InputOutput;
                    break;
                default:
                    break;
            }
            switch (this.TypeOracle.ToUpper())
            {
                case "BFILE":
                    this.TypeCs = OracleDbType.BFile;
                    break;
                case "BLOB":
                    this.TypeCs = OracleDbType.Blob;
                    break;
                case "CHAR":
                    this.TypeCs = OracleDbType.Char;
                    break;
                case "CLOB":
                    this.TypeCs = OracleDbType.Clob;
                    break;
                case "DATE":
                    this.TypeCs = OracleDbType.Date;
                    break;
                case "FLOAT":
                    this.TypeCs = OracleDbType.Decimal;
                    break;
                case "INTEGER":
                    this.TypeCs = OracleDbType.Decimal;
                    break;
                case "INTERVAL YEAR TO MONTH":
                    this.TypeCs = OracleDbType.Int32;
                    break;
                case "INTERVAL DAY TO SECOND":
                    this.TypeCs = OracleDbType.TimeStamp;
                    break;
                case "LONG":
                    this.TypeCs = OracleDbType.Long;
                    break;
                case "LONG RAW":
                    this.TypeCs = OracleDbType.Blob;
                    break;
                case "NCHAR":
                    this.TypeCs = OracleDbType.NChar;
                    break;
                case "NCLOB":
                    this.TypeCs = OracleDbType.NClob;
                    break;
                case "NUMBER":
                    this.TypeCs = OracleDbType.Decimal;
                    break;
                case "NVARCHAR2":
                    this.TypeCs = OracleDbType.NVarchar2;
                    break;
                case "RAW":
                    this.TypeCs = OracleDbType.Blob;
                    break;
                case "ROWID":
                    this.TypeCs = OracleDbType.NVarchar2;
                    break;
                case "TIMESTAMP":
                    this.TypeCs = OracleDbType.TimeStamp;
                    break;
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    this.TypeCs = OracleDbType.TimeStampLTZ;
                    break;
                case "TIMESTAMP WITH TIME ZONE":
                    this.TypeCs = OracleDbType.TimeStampTZ;
                    break;
                case "UNSIGNED INTEGER":
                    this.TypeCs = OracleDbType.Decimal;
                    break;
                case "VARCHAR2":
                    this.TypeCs = OracleDbType.Varchar2;
                    break;
                case "REF CURSOR":
                    this.TypeCs = OracleDbType.RefCursor;
                    break;
                default:
                    break;
            }
        }
    }
}
