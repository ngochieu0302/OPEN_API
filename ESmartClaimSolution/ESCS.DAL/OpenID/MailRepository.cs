using ESCS.COMMON.Common;
using ESCS.DAL.Repository.Oracle;
using ESCS.MODEL.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.DAL.OpenID
{
    public interface IMailRepository
    {
        Task<IEnumerable<sys_email_template>> GetAll();
    }
    public class MailRepository : IMailRepository
    {
        public async Task<IEnumerable<sys_email_template>> GetAll()
        {
            string package = "PKG_SYS_EMAIL_TEMPLATE";
            string storedname = "PSYS_EMAIL_TEMPLATE_ALL";
            OracleRepository<sys_email_template> service = new OracleRepository<sys_email_template>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, new { });
            var data = await service.ExcuteManyAsync(package + "." + storedname, param);
            return data;
        }
    }
}
