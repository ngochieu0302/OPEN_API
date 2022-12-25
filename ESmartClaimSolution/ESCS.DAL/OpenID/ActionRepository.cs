using ESCS.COMMON.Common;
using ESCS.COMMON.Response;
using ESCS.DAL.Repository.Oracle;
using ESCS.MODEL.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Types;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.DAL.OpenID
{
    public interface IActionRepository
    {
        Task<PaginationGenneric<openid_sys_action>> GetPaging(openid_sys_action search);
        Task<int> Save(openid_sys_action model);
        Task<int> SaveNew(openid_sys_action model);
        Task<int> CheckExistAction(openid_action_check model);
        Task<openid_sys_action_detail> GetDetail(sys_action action);
        Task<IEnumerable<openid_sys_action>> GenCode();
    }
    public class ActionRepository : IActionRepository
    {
        public async Task<PaginationGenneric<openid_sys_action>> GetPaging(openid_sys_action search)
        {
            string package = "PKG_BUS_ACTION";
            string storedname = "PSYS_ACTION_PAGING";
            OracleRepository<openid_sys_action> service = new OracleRepository<openid_sys_action>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, search);
            PaginationGenneric<openid_sys_action> dataPaging = new PaginationGenneric<openid_sys_action>();
            var data = await service.ExcuteManyAsync(package + "." + storedname, param);
            dataPaging.data = data;
            dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
            return dataPaging;
        }
        public async Task<IEnumerable<openid_sys_action>> GenCode()
        {
            string package = "PKG_BUS_ACTION";
            string storedname = "PSYS_ACTION_GENCODE";
            OracleRepository<openid_sys_action> service = new OracleRepository<openid_sys_action>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, null);
            var data = await service.ExcuteManyAsync(package + "." + storedname, param);
            return data;
        }
        public async Task<int> Save(openid_sys_action model)
        {
            string package = "PKG_BUS_ACTION";
            string storedname = "PSYS_ACTION_SAVE";
            OracleRepository<openid_sys_action> service = new OracleRepository<openid_sys_action>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, model);
            var data = await service.ExcuteNoneQueryAsync(package + "." + storedname, param);
            return data;
        }
        public async Task<int> SaveNew(openid_sys_action model)
        {
            string package = "PKG_BUS_ACTION";
            string storedname = "PSYS_ACTION_SAVE_NEW";
            OracleRepository<openid_sys_action> service = new OracleRepository<openid_sys_action>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, model);
            var data = await service.ExcuteNoneQueryAsync(package + "." + storedname, param);
            return data;
        }
        public async Task<int> CheckExistAction(openid_action_check model)
        {
            string package = "PKG_BUS_ACTION";
            string storedname = "PSYS_ACTION_CHECK";
            OracleRepository<openid_sys_action> service = new OracleRepository<openid_sys_action>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, model);
            var data = await service.ExcuteScalarAsync(package + "." + storedname, param);
            return Convert.ToInt32(data);
        }
        public async Task<openid_sys_action_detail> GetDetail(sys_action action)
        {
            string package = "PKG_BUS_ACTION";
            string storedname = "PSYS_ACTION_DETAIL";
            OracleRepository<openid_sys_action> service = new OracleRepository<openid_sys_action>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, action);
            openid_sys_action_detail detail = new openid_sys_action_detail();
            await service.ExcuteMultipleAsync(package + "." + storedname, param, gridReader => {
                detail.action = gridReader.Read<sys_action>().FirstOrDefault();
                detail.action_config = gridReader.Read<sys_action_config>().FirstOrDefault();
                detail.action_exc_db = gridReader.Read<sys_action_exc_db>().FirstOrDefault();
                detail.action_file = gridReader.Read<sys_action_file>().FirstOrDefault();
                if (detail.action_file!=null && !string.IsNullOrEmpty(detail.action_file.user_remote))
                {
                    detail.action_file.user_remote = Utilities.Decrypt(detail.action_file.user_remote);
                }
                if (detail.action_file != null && !string.IsNullOrEmpty(detail.action_file.pas_remote))
                {
                    detail.action_file.pas_remote = Utilities.Decrypt(detail.action_file.pas_remote);
                }
                detail.action_mail = gridReader.Read<sys_action_mail>().FirstOrDefault();
                if (detail.action_mail != null && !string.IsNullOrEmpty(detail.action_mail.user_remote))
                {
                    detail.action_mail.user_remote = Utilities.Decrypt(detail.action_mail.user_remote);
                }
                if (detail.action_mail != null && !string.IsNullOrEmpty(detail.action_mail.pas_remote))
                {
                    detail.action_mail.pas_remote = Utilities.Decrypt(detail.action_mail.pas_remote);
                }
            });
            return detail;
        }
    }
}
