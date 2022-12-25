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
    public interface IPartnerRepository
    {
        Task<PaginationGenneric<openid_sys_partner>> GetPaging(openid_sys_partner search);
        Task<openid_sys_partner_detail> GetDetail(openid_sys_partner search);
    }
    public class PartnerRepository : IPartnerRepository
    {
        public async Task<PaginationGenneric<openid_sys_partner>> GetPaging(openid_sys_partner search)
        {
            string package = "PKG_BUS_PARTNER";
            string storedname = "PSYS_PARTNER_PAGING";
            OracleRepository<openid_sys_partner> service = new OracleRepository<openid_sys_partner>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, search);
            PaginationGenneric<openid_sys_partner> dataPaging = new PaginationGenneric<openid_sys_partner>();
            var data = await service.ExcuteManyAsync(package + "." + storedname, param);
            dataPaging.data = data;
            dataPaging.tong_so_dong = param.Get<OracleDecimal>("b_tong_so_dong").Value;
            return dataPaging;
        }
        public async Task<openid_sys_partner_detail> GetDetail(openid_sys_partner action)
        {
            string package = "PKG_BUS_PARTNER";
            string storedname = "PSYS_PARTNER_DETAIL";
            OracleRepository<openid_sys_partner> service = new OracleRepository<openid_sys_partner>(OpenIDConfig.ConnectString);
            OracleDynamicParameters param = service.GetParamWithValueByQuery(OpenIDConfig.DbName, OpenIDConfig.Schema, package, storedname, action);
            openid_sys_partner_detail detail = new openid_sys_partner_detail();
            await service.ExcuteMultipleAsync(package + "." + storedname, param, gridReader => {
                detail.partner = gridReader.Read<openid_sys_partner>().FirstOrDefault();
                detail.partner_config = gridReader.Read<openid_sys_partner_config>().ToList();
            });
            return detail;
        }
    }
}
