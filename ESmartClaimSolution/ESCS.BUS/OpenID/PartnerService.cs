using ESCS.COMMON.Response;
using ESCS.DAL.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.BUS.OpenID
{
    public interface IPartnerService
    {
        Task<PaginationGenneric<openid_sys_partner>> GetPaging(openid_sys_partner search);
        Task<openid_sys_partner_detail> GetDetail(openid_sys_partner search);
    }
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partnerRepository;
        public PartnerService(IPartnerRepository partnerRepository)
        {
            _partnerRepository = partnerRepository;
        }
        public async Task<PaginationGenneric<openid_sys_partner>> GetPaging(openid_sys_partner search)
        {
            return await _partnerRepository.GetPaging(search);
        }
        public async Task<openid_sys_partner_detail> GetDetail(openid_sys_partner search)
        {
            return await _partnerRepository.GetDetail(search);
        }
    }
}
