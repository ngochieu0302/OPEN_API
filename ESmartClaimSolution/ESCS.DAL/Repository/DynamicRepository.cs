using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.Oracle;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.DAL.Repository.Oracle;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.DAL.Repository
{
    public interface IDynamicRepository
    {
        Task<object> ExcuteAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null);
        Task<dynamic> ExcuteDynamicAsync(BaseRequest model, HeaderRequest partner, Dictionary<string, bool> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null);
        #region Thành phần cũ
        Pagination GetPaging(BaseRequest model, HeaderRequest partner);
        Task<Pagination> GetPagingAsync(BaseRequest model, HeaderRequest partner);
        object GetScalar(BaseRequest model, HeaderRequest partner);
        Task<object> GetScalarAsync(BaseRequest model, HeaderRequest partner);
        object Get(BaseRequest model, HeaderRequest partner);
        Task<object> GetAsync(BaseRequest model, HeaderRequest partner);
        object GetList(BaseRequest model, HeaderRequest partner);
        Task<object> GetListAsync(BaseRequest model, HeaderRequest partner);
        object GetMultiple(BaseRequest model, HeaderRequest partner);
        Task<object> GetMultipleAsync(BaseRequest model, HeaderRequest partner);
        Task<DataSet> GetMultipleToDataSetAsync(IDictionary<string, object> model, HeaderRequest partner);
        Task<DataSet> GetDataSetAsync(IDictionary<string, object> model, HeaderRequest partner);
        int PostData(BaseRequest model, HeaderRequest partner);
        Task<int> PostDataAsync(BaseRequest model, HeaderRequest partner);
        Task<int> PostDataMultipleAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs);
        Task<object> PostDataMultipleScalarAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs);
        ActionConnection GetConnection(HeaderRequest model);
        #endregion
        Task<dynamic> ExcuteDynamicNewAsync(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null);
        Task<IEnumerable<T>> ExcuteListAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class;
        Task<int> ExcuteNoneQueryAsync(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null);
        Task<T> ExcuteSingleAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class;
        Task ExcuteReaderAsync(IDictionary<string, object> model, HeaderRequest partner, Action<IDataReader,List<string>> readData);
    }
    public class DynamicRepository : IDynamicRepository
    {
        private readonly IMemoryCacheManager _cacheManager;
        private readonly ICacheServer _cacheServer;
        public  DynamicRepository(ICacheServer cacheServer, IMemoryCacheManager cacheManager)
        {
            _cacheServer = cacheServer;
            _cacheManager = cacheManager;
        }
        public async Task<object> ExcuteAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (prefixs != null && prefixs.Count() > 0)
            {
                foreach (var prefix in prefixs)
                {
                    param.MapArrayValue(prefix, model.data_info);
                }
            }
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            Dictionary<string, object> outPutData = new Dictionary<string, object>();
            dynamic data = null;

            switch (conn.type_excute.ToUpper())
            {
                case "RETURN_NONE":
                    data = await service.ExcuteNoneQueryAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_SCALAR":
                    data = await service.ExcuteScalarAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_SINGLE":
                    data = await service.ExcuteSingleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return Utilities.ToObjectJson(data);
                case "RETURN_PAGING":
                    Pagination dataPaging = new Pagination();
                    data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    dataPaging.data = Utilities.ToListObjectJson(data);
                    dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return dataPaging;
                case "RETURN_LIST":
                    data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return Utilities.ToListObjectJson(data);
                case "RETURN_MULTIPLE":
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    await service.ExcuteMultipleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, grid => {
                        foreach (var p in param.parameters)
                        {
                            if (p.Key.ToLower().StartsWith("cur_"))
                            {
                                dic.Add(p.Key.Substring(4, p.Key.Length - 4), Utilities.ToObjectJson(grid.Read<object>().FirstOrDefault()));
                            }
                            if (p.Key.ToLower().StartsWith("curs_"))
                            {
                                dic.Add(p.Key.Substring(5, p.Key.Length - 5), Utilities.ToListObjectJson(grid.Read<object>().ToList()));
                            }
                        }
                    });
                    dynamic res = dic;
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return res;
                default:
                    break;
            }
            return null;
        }
        public async Task<dynamic> ExcuteDynamicAsync(BaseRequest model, HeaderRequest partner, Dictionary<string, bool> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (prefixs != null && prefixs.Count() > 0)
            {
                foreach (var prefix in prefixs)
                {
                    param.MapArrayValue(prefix.Key, model.data_info,prefix.Value);
                }
            }
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            Dictionary<string, object> outPutData = new Dictionary<string, object>();
            dynamic data = null;

            switch (conn.type_excute.ToUpper())
            {
                case "RETURN_NONE":
                    data = await service.ExcuteNoneQueryAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_SCALAR":
                    data = await service.ExcuteScalarAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_SINGLE":
                    data = await service.ExcuteSingleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_PAGING":
                    Pagination dataPaging = new Pagination();
                    data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    dataPaging.data = data;
                    dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return dataPaging;
                case "RETURN_LIST":
                    data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_MULTIPLE":
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    await service.ExcuteMultipleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, grid => {
                        foreach (var p in param.parameters)
                        {
                            if (p.Key.ToLower().StartsWith("cur_"))
                            {
                                dic.Add(p.Key.Substring(4, p.Key.Length - 4), Utilities.ToObjectJson(grid.Read<object>().FirstOrDefault()));
                            }
                            if (p.Key.ToLower().StartsWith("curs_"))
                            {
                                dic.Add(p.Key.Substring(5, p.Key.Length - 5), Utilities.ToListObjectJson(grid.Read<object>().ToList()));
                            }
                        }
                    });
                    dynamic res = dic;
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return res;
                default:
                    break;
            }
            return null;
        }
        private Dictionary<string, object> GetOutPutValue(OracleDynamicParameters param)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            if (param != null && param.parameters != null && param.parameters.Values != null)
            {
                var paramInfos = param.parameters.Values.Where(n => n.ParameterDirection != ParameterDirection.Input && n.DbType != OracleDbType.RefCursor).ToList();
                if (paramInfos != null)
                {
                    foreach (var paramInfo in paramInfos)
                    {
                        var propertyName = paramInfo.Name.ToLower().StartsWith("b_") ? paramInfo.Name.ToLower().Substring(2, paramInfo.Name.Length - 2) : paramInfo.Name.ToLower();
                        switch (paramInfo.DbType)
                        {
                            case OracleDbType.Decimal:
                                res.Add(propertyName, param.Get<OracleDecimal>(paramInfo.Name).IsNull?(decimal?)null: param.Get<OracleDecimal>(paramInfo.Name).Value);
                                break;
                            case OracleDbType.NChar:
                            case OracleDbType.NVarchar2:
                            case OracleDbType.Varchar2:
                            case OracleDbType.Char:
                                res.Add(propertyName, param.Get<OracleString>(paramInfo.Name).IsNull ? null : param.Get<OracleString>(paramInfo.Name).Value);
                                break;
                            case OracleDbType.Clob:
                            case OracleDbType.NClob:
                                res.Add(propertyName, param.Get<OracleClob>(paramInfo.Name).IsNull ? null : param.Get<OracleClob>(paramInfo.Name).Value);
                                break;
                            case OracleDbType.Date:
                                res.Add(propertyName, param.Get<OracleDate>(paramInfo.Name).IsNull ? (DateTime?)null : param.Get<OracleDate>(paramInfo.Name).Value);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return res;
        }
        #region Thành phần cũ
        public object Get(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = service.ExcuteSingle(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return Utilities.ToObjectJson(data);
        }
        public async Task<object> GetAsync(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = await service.ExcuteSingleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return Utilities.ToObjectJson(data);
        }
        public object GetScalar(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = service.ExcuteScalarAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return data;
        }
        public async Task<object> GetScalarAsync(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = await service.ExcuteScalarAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return data;
        }
        public object GetList(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = service.ExcuteMany(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return Utilities.ToListObjectJson(data);
        }
        public async Task<object> GetListAsync(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return Utilities.ToListObjectJson(data);
        }
        public object GetMultiple(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (param != null && param.parameters.Count() > 0)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(conn.pkg_name))
                {
                    conn.pkg_name = "." + conn.pkg_name;
                }
                service.ExcuteMultiple(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, grid => {
                    foreach (var p in param.parameters)
                    {
                        if (p.Key.ToLower().StartsWith("cur_"))
                        {
                            dic.Add(p.Key.Substring(4, p.Key.Length - 4), Utilities.ToObjectJson(grid.Read<object>().FirstOrDefault()));
                        }
                        if (p.Key.ToLower().StartsWith("curs_"))
                        {
                            dic.Add(p.Key.Substring(5, p.Key.Length - 5), Utilities.ToListObjectJson(grid.Read<object>().ToList()));
                        }
                    }
                });
                dynamic res = dic;
                return res;
            }
            return null;
        }
        public async Task<object> GetMultipleAsync(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (param != null && param.parameters.Count() > 0)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(conn.pkg_name))
                {
                    conn.pkg_name = "." + conn.pkg_name;
                }
                await service.ExcuteMultipleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, grid => {
                    foreach (var p in param.parameters)
                    {
                        if (p.Key.ToLower().StartsWith("cur_"))
                        {
                            dic.Add(p.Key.Substring(4, p.Key.Length - 4), Utilities.ToObjectJson(grid.Read<object>().FirstOrDefault()));
                        }
                        if (p.Key.ToLower().StartsWith("curs_"))
                        {
                            dic.Add(p.Key.Substring(5, p.Key.Length - 5), Utilities.ToListObjectJson(grid.Read<object>().ToList()));
                        }
                    }
                });
                dynamic res = dic;
                return res;
            }
            return null;
        }
        public async Task<DataSet> GetMultipleToDataSetAsync(IDictionary<string, object> model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<DataSet> service = new OracleRepository<DataSet>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            if (param != null && param.parameters.Count() > 0)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(conn.pkg_name))
                {
                    conn.pkg_name = "." + conn.pkg_name;
                }
                DataSet ds = new DataSet();
                await service.ExcuteMultipleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, grid => {
                    string[] arrTableName = new string[] { "B", "C", "D", "E","F", "G", "H", "I", "J", "K","L","M","N", "O", "P", "Q"};
                    int index_name = 0;
                    var lstCursor = param.parameters.Where(n => n.Value.DbType == OracleDbType.RefCursor);
                    if (lstCursor!=null && lstCursor.Count() >0)
                    {
                        foreach (var p in lstCursor)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable = Utilities.ToDataTable(grid.Read<object>().ToList());
                            if ((p.Value.Name == "curs_an_hien" || p.Value.Name == "curs_source"))
                            {
                                dataTable.TableName = p.Value.Name;
                            }
                            else if(p.Value.Name == "curs_chu_ky")
                            {
                                DataTable dataTableChuKy = new DataTable();
                                dataTableChuKy.TableName = arrTableName[index_name];
                                if (dataTable!=null && dataTable.Rows!=null && dataTable.Rows.Count >0)
                                {
                                    for (int i = 1; i <= dataTable.Rows.Count; i++)
                                    {
                                        dataTableChuKy.Columns.Add("ngay_"+i, typeof(String));
                                        dataTableChuKy.Columns.Add("chuc_danh_" + i, typeof(String));
                                        dataTableChuKy.Columns.Add("nguoi_ky_" + i, typeof(String));
                                    }
                                    DataRow dr = dataTableChuKy.NewRow();
                                    for (int i = 1; i <= dataTable.Rows.Count; i++)
                                    {
                                        dr["ngay_" + i] = dataTable.Rows[i - 1]["ngay"];
                                        dr["chuc_danh_" + i] = dataTable.Rows[i - 1]["chuc_danh"];
                                        dr["nguoi_ky_" + i] = dataTable.Rows[i - 1]["nguoi_ky"];
                                    }
                                    dataTableChuKy.Rows.Add(dr);
                                }
                                dataTable = dataTableChuKy;
                                index_name++;
                            }
                            else
                            {
                                if (index_name<=15)
                                {
                                    dataTable.TableName = arrTableName[index_name];
                                    index_name++;
                                }
                                else
                                {
                                    dataTable.TableName = p.Value.Name;
                                }    
                            }    
                            ds.Tables.Add(dataTable);
                        }
                    }
                });
                return ds;
            }
            return null;
        }
        public async Task<DataSet> GetDataSetAsync(IDictionary<string, object> model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<DataSet> service = new OracleRepository<DataSet>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            if (param != null && param.parameters.Count() > 0)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(conn.pkg_name))
                {
                    conn.pkg_name = "." + conn.pkg_name;
                }
                DataSet ds = new DataSet();
                await service.ExcuteMultipleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, grid => {
                    int index_name = 0;
                    foreach (var p in param.parameters)
                    {
                        if (param.parameters[p.Key].DbType == OracleDbType.RefCursor)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable = Utilities.ToDataTable(grid.Read<object>());
                            dataTable.TableName = p.Value.Name;
                            ds.Tables.Add(dataTable);
                            index_name++;
                        }
                    }
                });
                return ds;
            }
            return null;
        }
        public int PostData(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = service.ExcuteNoneQuery(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return data;
        }
        public async Task<int> PostDataAsync(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = await service.ExcuteNoneQueryAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return data;
        }
        public async Task<Pagination> GetPagingAsync(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            Pagination dataPaging = new Pagination();
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            dataPaging.data = Utilities.ToListObjectJson(data);
            dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
            return dataPaging;
        }
        public Pagination GetPaging(BaseRequest model, HeaderRequest partner)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            Pagination dataPaging = new Pagination();
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = service.ExcuteMany(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            dataPaging.data = Utilities.ToListObjectJson(data);
            dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
            return dataPaging;
        }
        public ActionConnection GetConnection(HeaderRequest partner)
        {
            string keyCache = CachePrefixKeyConstants.GetKeyCacheAction(partner.partner_code.Trim(), partner.envcode.Trim(), partner.action.Trim());
            string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            if (string.IsNullOrEmpty(json))
            {
                OracleRepository<ActionConnection> service = new OracleRepository<ActionConnection>(OpenIDConfig.ConnectString);
                OracleDynamicParameters param = new OracleDynamicParameters();
                param.Add("b_action", partner.action.Trim(), OracleDbType.Varchar2, ParameterDirection.Input);
                param.Add("b_partner_code", partner.partner_code.Trim(), OracleDbType.Varchar2, ParameterDirection.Input);
                param.Add("b_envcode", partner.envcode.Trim(), OracleDbType.Varchar2, ParameterDirection.Input);
                param.Add("cur_connect", null, OracleDbType.RefCursor, ParameterDirection.Output);
                ActionConnection conn = service.ExcuteSingle(OpenIDConfig.Schema + ".PKG_SYS_COMMON.PSP_GET_CONNECT_OPENID", param);
                if (conn == null)
                {
                    return null;
                }
                conn.SetConnect();
                _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, Utilities.EncryptByKey(JsonConvert.SerializeObject(conn), OpenIDConstants.VERSION), DateTime.Now.AddDays(30) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                return conn;
            }
            return JsonConvert.DeserializeObject<ActionConnection>(Utilities.DecryptByKey(json, OpenIDConstants.VERSION));
        }

        public async Task<int> PostDataMultipleAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");

            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (prefixs != null && prefixs.Count() > 0)
            {
                foreach (var prefix in prefixs)
                {
                    param.MapArrayValue(prefix, model.data_info);
                }
            }
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = await service.ExcuteNoneQueryAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return data;
        }
        public async Task<object> PostDataMultipleScalarAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");

            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model.data_info);
            if (prefixs != null && prefixs.Count() > 0)
            {
                foreach (var prefix in prefixs)
                {
                    param.MapArrayValue(prefix, model.data_info);
                }
            }
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            var data = await service.ExcuteScalarAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            return data;
        }
        #endregion
        public async Task<dynamic> ExcuteDynamicNewAsync(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            Dictionary<string, object> outPutData = new Dictionary<string, object>();
            dynamic data = null;

            switch (conn.type_excute.ToUpper())
            {
                case "RETURN_NONE":
                    data = await service.ExcuteNoneQueryAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_SCALAR":
                    data = await service.ExcuteScalarAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_SINGLE":
                    data = await service.ExcuteSingleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_PAGING":
                    Pagination dataPaging = new Pagination();
                    data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    dataPaging.data = data;
                    dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return dataPaging;
                case "RETURN_LIST":
                    data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return data;
                case "RETURN_MULTIPLE":
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    await service.ExcuteMultipleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, grid => {
                        foreach (var p in param.parameters)
                        {
                            if (p.Key.ToLower().StartsWith("cur_"))
                            {
                                dic.Add(p.Key.Substring(4, p.Key.Length - 4), Utilities.ToObjectJson(grid.Read<object>().FirstOrDefault()));
                            }
                            if (p.Key.ToLower().StartsWith("curs_"))
                            {
                                dic.Add(p.Key.Substring(5, p.Key.Length - 5), Utilities.ToListObjectJson(grid.Read<object>().ToList()));
                            }
                        }
                    });
                    dynamic res = dic;
                    outPutData = GetOutPutValue(param);
                    if (actionOutPutValue != null)
                        actionOutPutValue(outPutData);
                    return res;
                default:
                    break;
            }
            return null;
        }
        public async Task<IEnumerable<T>> ExcuteListAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<T> service = new OracleRepository<T>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            Dictionary<string, object> outPutData = new Dictionary<string, object>();
            dynamic data = null;
            data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            outPutData = GetOutPutValue(param);
            if (actionOutPutValue != null)
                actionOutPutValue(outPutData);
            return data;
        }
        public async Task<int> ExcuteNoneQueryAsync(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) 
        {
            return 1;
            //var conn = GetConnection(partner);
            //if (conn == null)
            //    throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            //OracleRepository<int> service = new OracleRepository<int>(conn.connectionstring);
            //OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            //if (!string.IsNullOrEmpty(conn.pkg_name))
            //{
            //    conn.pkg_name = "." + conn.pkg_name;
            //}
            //Dictionary<string, object> outPutData = new Dictionary<string, object>();
            //dynamic data = null;
            //data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            //outPutData = GetOutPutValue(param);
            //if (actionOutPutValue != null)
            //    actionOutPutValue(outPutData);
            //return data;
        }
        public async Task<T> ExcuteSingleAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<T> service = new OracleRepository<T>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            Dictionary<string, object> outPutData = new Dictionary<string, object>();
            T data = default(T);
            data = await service.ExcuteSingleAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            outPutData = GetOutPutValue(param);
            if (actionOutPutValue != null)
                actionOutPutValue(outPutData);
            return data;
        }
        public async Task<dynamic> ExcuteDynamicListAsync(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            Dictionary<string, object> outPutData = new Dictionary<string, object>();
            dynamic data = null;
            data = await service.ExcuteManyAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param);
            outPutData = GetOutPutValue(param);
            if (actionOutPutValue != null)
                actionOutPutValue(outPutData);
            return data;
        }
        public async Task ExcuteReaderAsync(IDictionary<string, object> model, HeaderRequest partner, Action<IDataReader,List<string>> readData)
        {
            var conn = GetConnection(partner);
            if (conn == null)
                throw new Exception("Thông tin kết nối đối tác không hợp lệ");
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(conn.connectionstring);
            OracleDynamicParameters param = service.GetParamWithValueByQueryNew(conn.db_name, conn.schemadb, conn.pkg_name, conn.stored_name, model);
            if (!string.IsNullOrEmpty(conn.pkg_name))
            {
                conn.pkg_name = "." + conn.pkg_name;
            }
            await service.ExcuteReaderAsync(conn.schemadb + conn.pkg_name + "." + conn.stored_name, param, readData);
        }
    }
}
