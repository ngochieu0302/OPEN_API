using ESCS.COMMON.Common;
using ESCS.COMMON.Response;
using ESCS.DAL.Repository.Oracle;
using ESCS.MODEL.OpenID.ModelView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.DAL.OpenID
{
    public interface IOpenIDCommonRepository
    {
        Task<BaseResponse<openid_category_result>> GetCategoryOpenId(string envcode);
    }
    public class OpenIDCommonRepository : IOpenIDCommonRepository
    {
        public async Task<BaseResponse<openid_category_result>> GetCategoryOpenId(string envcode)
        {
            string package = "PKG_BUS_COMMON";
            string storedname = "PSYS_GET_DM";
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, new { envcode  = envcode });
            openid_category_result data = new openid_category_result();
            await service.ExcuteMultipleAsync(package + "." + storedname, param, grid =>
            {
                data.server = grid.Read<openid_sys_server>();
                data.database = grid.Read<openid_sys_database>();
                data.schema = grid.Read<openid_sys_schema>();
            });
            string json = JsonConvert.SerializeObject(data);
            BaseResponse<openid_category_result> res = new BaseResponse<openid_category_result>();
            res.data_info = data;
            res.state_info.status = "OK";
            res.state_info.message_code = "200";
            res.state_info.message_body = "Thành công";
            return res;
        }
    }
}
