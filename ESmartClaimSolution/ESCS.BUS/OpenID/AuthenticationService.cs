using ESCS.COMMON.Auth;
using ESCS.COMMON.Common;
using ESCS.COMMON.Response;
using ESCS.DAL.OpenID;
using ESCS.MODEL.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.BUS.OpenID
{
    public interface IAuthenticationService
    {
        Task<BaseResponse<sys_partner_cache>> Login(account user);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        public AuthenticationService(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }
        public async Task<BaseResponse<sys_partner_cache>> Login(account user)
        {
            user.password = Utilities.Sha256Hash(user.password);
            return await _authenticationRepository.Login(user);
        }
    }
}
