using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Request;
using ESCS.DAL.Repository;
using ESCS.MODEL.OpenID.ModelView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.BUS.Services
{
    public interface IOpenIdService
    {
        Task<object> GetPartnerWithConfig(BaseRequest model);
        Task<object> GetActionNoneAuthen(BaseRequest model);
        Task<object> LogAccount(BaseRequest model);
        Task<object> AddConnection(BaseRequest model);
        Task<object> UpdateConnection(BaseRequest model);
        Task<object> GetConnection(BaseRequest model);
        Task<object> ChangePassApi(BaseRequest model);
        Task<object> DisConnection(BaseRequest model);
    }
    public class OpenIdService : IOpenIdService
    {
        private readonly ICacheServer _cacheServer;
        private readonly IOpenIdRepository _openIdRepository;
        public OpenIdService(
            ICacheServer cacheServer,
            IOpenIdRepository openIdRepository
        )
        {
            _cacheServer = cacheServer;
            _openIdRepository = openIdRepository;
        }
        public async Task<object> GetPartnerWithConfig(BaseRequest model)
        {
            return await _openIdRepository.GetPartnerWithConfig(model);
        }
        public async Task<object> GetActionNoneAuthen(BaseRequest model)
        {
            return await _openIdRepository.GetActionNoneAuthen(model);
        }
        public async Task<object> GetConnection(BaseRequest model)
        {
            return await _openIdRepository.GetConnection(model);
        }
        public async Task<object> LogAccount(BaseRequest model)
        {
            return await _openIdRepository.LogAccount(model);
        }
        public async Task<object> AddConnection(BaseRequest model)
        {
            return await _openIdRepository.AddConnection(model);
        }
        public async Task<object> DisConnection(BaseRequest model)
        {
            return await _openIdRepository.DisConnection(model);
        }
        public async Task<object> UpdateConnection(BaseRequest model)
        {
            return await _openIdRepository.UpdateConnection(model);
        }

        public async Task<object> ChangePassApi(BaseRequest model)
        {
            return await _openIdRepository.ChangePassApi(model);
        }
    }
}
