using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESCS.API.Attributes;
using ESCS.API.Hubs;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.AI;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Http;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.COMMON.SMS.MCM;
using ESCS.MODEL.ESCS;
using ESCS.MODEL.OpenID.ModelView;
using FirebaseAdmin.Messaging;
using Google.Cloud.Vision.V1;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ESCS.API.Controllers
{
    [Route("api/mcm")]
    [ApiController]
    [ESCSAuth]
    public class MCMController : BaseController
    {
        /// <summary>
        /// MCMController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public MCMController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
        }
        [HttpGet]
        [Route("callback/{PartnerCode}")]
        public async Task<IActionResult> Callback(string PartnerCode, [FromQuery] mcm_calback model)
        {
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            if (string.IsNullOrEmpty(PartnerCode))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                res.state_info.message_body = "Không xác định được đối tác hệ thống.";
                return Ok(res);
            }

            var url = HttpContext.Request.GetEncodedUrl();
            HeaderRequest header = new HeaderRequest();
            header.action = ESCSStoredProcedures.PBH_DICH_VU_MCM_LICH_GUI_CALLBACK;
            header.partner_code = PartnerCode.Trim().ToUpper();
            Dictionary<string, object> data_info = new Dictionary<string, object>();
            data_info.AddWithExists("smsid", model.SMSID);
            data_info.AddWithExists("callback", url);
            data_info.AddWithExists("trang_thai_mcm", model.SendStatus);
            var outPut = new Dictionary<string, object>();
            await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
            res.state_info.status = STATUS_OK;
            res.state_info.message_body = "Thành công";
            return Ok(res);
        }
        [HttpGet]
        [Route("callback/zalo")]
        public async Task<IActionResult> CallbackZaloAuth([FromQuery] mcm_calback model)
        {
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            HeaderRequest header = new HeaderRequest();
            header.action = ESCSStoredProcedures.PBH_DICH_VU_MCM_LICH_GUI_CALLBACK;
            Dictionary<string, object> data_info = new Dictionary<string, object>();
            data_info.AddWithExists("smsid", model.SMSID);
            data_info.AddWithExists("trang_thai_mcm", model.SendStatus);
            var outPut = new Dictionary<string, object>();
            await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
            res.state_info.status = STATUS_OK;
            res.state_info.message_body = "Thành công";
            return Ok(res);
        }
    }
}
