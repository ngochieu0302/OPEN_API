using ESCS.COMMON.Common;
using ESCS.COMMON.Response;
using ESCS.DAL.Repository.Oracle;
using ESCS.MODEL.OpenID;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.DAL.OpenID
{
    public interface IErrorCodeRepository
    {
        Task<PaginationGenneric<sys_error_code>> GetPaging(sys_error_code search);
        Task<int> Save(sys_error_code model);
        Task<sys_error_code> Get(sys_error_code search);
    }
    public class ErrorCodeRepository : IErrorCodeRepository
    {
        public async Task<sys_error_code> Get(sys_error_code search)
        {
            string package = "PKG_SYS_ERROR_CODE";
            string storedname = "PSYS_ERROR_CODE_GET";
            OracleRepository<sys_error_code> service = new OracleRepository<sys_error_code>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, search);
            var data = await service.ExcuteSingleAsync(package + "." + storedname, param);
            return data;
        }

        public async Task<PaginationGenneric<sys_error_code>> GetPaging(sys_error_code search)
        {
            string package = "PKG_SYS_ERROR_CODE";
            string storedname = "PSYS_ERROR_CODE_PAGING";
            OracleRepository<sys_error_code> service = new OracleRepository<sys_error_code>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, search);
            PaginationGenneric<sys_error_code> dataPaging = new PaginationGenneric<sys_error_code>();
            var data = await service.ExcuteManyAsync(package + "." + storedname, param);
            dataPaging.data = data;
            dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
            return dataPaging;
        }

        public async Task<int> Save(sys_error_code model)
        {
            string package = "PKG_SYS_ERROR_CODE";
            string storedname = "PSYS_ERROR_CODE_SAVE";
            OracleRepository<sys_error_code> service = new OracleRepository<sys_error_code>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, model);
            var data = await service.ExcuteNoneQueryAsync(package + "." + storedname, param);
            return data;
        }
    }
}
