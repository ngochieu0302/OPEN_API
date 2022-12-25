using ESCS.COMMON.Auth;
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
    public interface IAuthenticationRepository
    {
        Task<BaseResponse<sys_partner_cache>> Login(account user);
    }
    public class AuthenticationRepository : IAuthenticationRepository
    {
        public async Task<BaseResponse<sys_partner_cache>> Login(account user)
        {
            string package = "PKG_BUS_AUTHEN";
            string storedname = "PSYS_LOGIN_CMS";
            OracleRepository<dynamic> service = new OracleRepository<dynamic>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, user);
            dynamic data = await service.ExcuteSingleAsync(package + "." + storedname, param);
            string json = JsonConvert.SerializeObject(data);
            BaseResponse<sys_partner_cache> res = new BaseResponse<sys_partner_cache>();
            res.data_info = JsonConvert.DeserializeObject<sys_partner_cache>(json);
            res.state_info.status = "OK";
            res.state_info.message_code = "200";
            res.state_info.message_body = "Thành công";
            return res;
        }
    }
}
