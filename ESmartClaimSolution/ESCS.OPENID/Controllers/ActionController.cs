using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Response;
using ESCS.MODEL.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using ESCS.OPENID.Attributes;
using ESCS.OPENID.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using X.PagedList;

namespace ESCS.OPENID.Controllers
{
    [SystemAuthen]
    public class ActionController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>

        private readonly IOpenIDCommonService _openIDCommonService;
        private readonly IActionService _actionService;
        private readonly ICacheServer _cacheServer;
        IHubContext<NotifyMessageHub> _notificationHubContext;
        public ActionController(
            IOpenIDCommonService openIDCommonService,
            IActionService actionService,
            ICacheServer cacheServer,
            IHubContext<NotifyMessageHub> notificationHubContext)
        {
            _notificationHubContext = notificationHubContext;
            _openIDCommonService = openIDCommonService;
            _actionService = actionService;
            _cacheServer = cacheServer;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PageLoad()
        {
            sys_partner_cache partner = GetUser();
            var res = await _openIDCommonService.GetCategoryOpenId(partner.config_envcode);
            return Json(res.data_info);
        }
        [HttpPost]
        public async Task<IActionResult> GetPaging(openid_sys_action searchModel)
        {
            searchModel.envcode = GetUser().config_envcode;
            var res = await _actionService.GetPaging(searchModel);
            return Json(res);
        }
        public async Task<IActionResult> Save(openid_sys_action model)
        {
            model.acf_envcode = GetUser().config_envcode;
            var res = await _actionService.Save(model);
            if (!string.IsNullOrEmpty(model.exc_actioncode))
            {
                string keyCache = CachePrefixKeyConstants.GetKeyCacheAction("*", "*", model.exc_actioncode.ToUpper());
                _cacheServer.RemoveKeyCacheByPattern(RedisCacheMaster.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            }
            return Json(res);
        }
        public async Task<IActionResult> SaveNew(openid_sys_action model)
        {
            model.acf_envcode = GetUser().config_envcode;
            var res = await _actionService.SaveNew(model);
            if (!string.IsNullOrEmpty(model.exc_actioncode))
            {
                string keyCache = CachePrefixKeyConstants.GetKeyCacheAction("*", "*", model.exc_actioncode.ToUpper());
                _cacheServer.RemoveKeyCacheByPattern(RedisCacheMaster.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
                string keyCacheNoneAuthen = CachePrefixKeyConstants.GetKeyCacheActionNoneAuthen();
                _cacheServer.RemoveKeyCacheByPattern(RedisCacheMaster.Endpoint, keyCacheNoneAuthen, RedisCacheMaster.DatabaseIndex);
            }
            return Json(res);
        }
        public IActionResult ClearCache(openid_sys_action model)
        {
            if (!string.IsNullOrEmpty(model.exc_storedprocedure) || !string.IsNullOrEmpty(model.file_storedprocedure) || !string.IsNullOrEmpty(model.mail_storedprocedure))
            {
                string storedprocedure = "";
                if (!string.IsNullOrEmpty(model.exc_storedprocedure))
                {
                    storedprocedure = model.exc_storedprocedure;
                }
                if (!string.IsNullOrEmpty(model.file_storedprocedure))
                {
                    storedprocedure = model.file_storedprocedure;
                }
                if (!string.IsNullOrEmpty(model.mail_storedprocedure))
                {
                    storedprocedure = model.mail_storedprocedure;
                }
                string package = "";
                if (!string.IsNullOrEmpty(model.exc_package_name))
                {
                    package = model.exc_package_name;
                }
                if (!string.IsNullOrEmpty(model.file_package_name))
                {
                    package = model.file_package_name;
                }
                if (!string.IsNullOrEmpty(model.mail_package_name))
                {
                    package = model.mail_package_name;
                }
                string actioncode = "";
                if (!string.IsNullOrEmpty(model.exc_actioncode))
                {
                    actioncode = model.exc_actioncode;
                }
                if (!string.IsNullOrEmpty(model.file_actioncode))
                {
                    actioncode = model.file_actioncode;
                }
                if (!string.IsNullOrEmpty(model.mail_actioncode))
                {
                    actioncode = model.mail_actioncode;
                }

                string keyCache = CachePrefixKeyConstants.GetKeyCacheParamStored("*", "*", storedprocedure, package);
                _cacheServer.RemoveKeyCacheByPattern(RedisCacheMaster.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);

                string keyCacheAction = CachePrefixKeyConstants.GetKeyCacheAction("*", "*", actioncode);
                _cacheServer.RemoveKeyCacheByPattern(RedisCacheMaster.Endpoint, keyCacheAction, RedisCacheMaster.DatabaseIndex);
            }
            return Json("Thành công");
        }
        public async Task<IActionResult> getParamStored()
        {
            sys_partner_cache partner = GetUser();
            var res = await _openIDCommonService.GetCategoryOpenId(partner.config_envcode);
            return Json(res.data_info);
        }
        public async Task<IActionResult> GenCode()
        {
            sys_partner_cache partner = GetUser();
            var lstAction = await _actionService.GenCode();
            return Content(GenCodeConstant(lstAction));
        }
        private string GenCodeConstant(IEnumerable<openid_sys_action> actions)
        {
            string code = string.Empty;
            code += "//Created:" + DateTime.Now.ToString("dd/MM/yyyy") + "\n";
            code += "//Author: thanhnx.escs\n";
            code += "namespace ESCS.COMMON.ESCSStoredProcedures\n";
            code += "{\n";
            code += "\tusing System;\n";
            code += "\tusing System.Collections.Generic;\n";
            code += "\n";
            code += "\tpublic partial class StoredProcedure\n";
            code += "\t{\n";
            foreach (var action in actions)
            {
                code += "\t\t/// <summary>\n";
                code += "\t\t/// "+ action.ac_action_name + "\n";
                code += "\t\t/// </summary>\n";
                code += "\t\tpublic const string "+ action.exc_storedprocedure + " = \""+action.exc_actioncode.Trim()+"\";\n";
            }
            code += "\t}\n";
            code += "}";
            return code;
        }

        [HttpPost]
        public async Task<IActionResult> GetDetail(sys_action model)
        {
            var res = await _actionService.GetDetail(model);
            return Json(res);
        }
    }
}