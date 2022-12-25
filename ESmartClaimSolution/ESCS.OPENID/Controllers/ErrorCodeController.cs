using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Contants;
using ESCS.MODEL.OpenID;
using ESCS.OPENID.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ESCS.OPENID.Controllers
{
    [SystemAuthen]
    public class ErrorCodeController : Controller
    {
        private readonly IOpenIDCommonService _openIDCommonService;
        private readonly IErrorCodeService _errorCodeService;
        private readonly ICacheServer _cacheServer;
        public ErrorCodeController(
            IOpenIDCommonService openIDCommonService,
            IErrorCodeService errorCodeService,
            ICacheServer cacheServer
        )
        {
            _openIDCommonService = openIDCommonService;
            _errorCodeService = errorCodeService;
            _cacheServer = cacheServer;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PageLoad()
        {
            //string session = HttpContext.Session.GetString(OpenIDConstants.SESSSION_KEY_OPENID_CMS);
            //sys_partner_cache partner = JsonConvert.DeserializeObject<sys_partner_cache>(session);
            var res = await _openIDCommonService.GetCategoryOpenId("DEV");
            return Json(res.data_info);
        }
        public async Task<IActionResult> GetPaging(sys_error_code searchModel)
        {
            var res = await _errorCodeService.GetPaging(searchModel);
            return Json(res);
        }
        public async Task<IActionResult> Save(sys_error_code model)
        {
            var res = await _errorCodeService.Save(model);
            if (!string.IsNullOrEmpty(model.server_error_code))
            {
                string keyCache = "ERROCODE:"+ model.server_error_code;
                _cacheServer.Remove(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            }
            return Json(res);
        }
    }
}
