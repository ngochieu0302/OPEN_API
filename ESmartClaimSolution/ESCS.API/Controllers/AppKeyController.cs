using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.API.Hubs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ESCS.COMMON.Request;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.Response;
using Microsoft.AspNetCore.Hosting;
using ESCS.MODEL.ESCS;
using System.Threading.Tasks;
using System.Collections.Generic;
using ESCS.COMMON.Caches;
using ESCS.COMMON.ExtensionMethods;
using ESCS.MODEL.OpenID.ModelView;
using Newtonsoft.Json.Linq;
using ESCS.COMMON.Contants;
using System;
using ESCS.COMMON.Common;
using System.Linq;
using ESCS.COMMON.Http;
using System.Net;

namespace ESCS.API.Controllers
{
    [ApiController]
    [Route("api/app")]
    public class AppKeyController : BaseController
    {
        private static readonly long TimeExpiry = 20230623;//20230630;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogMongoService<LogSendNotify> _logNotify;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AppKeyController(
          ICacheServer cacheServer,
          IDynamicService dynamicService,
          IOpenIdService openIdService,
          ILogMongoService<LogException> logRequestService,
          IHubContext<PartnerNotifyHub> partnerNotifyHub,
          IErrorCodeService errorCodeService,
          IDataProtectionProvider provider,
          IUserConnectionManager userConnectionManager,
          ILogMongoService<LogSendNotify> logNotify,
          ILogMongoService<LogContent> logContent,
          IWebHostEnvironment webHostEnvironment)
      : base(cacheServer, dynamicService, openIdService,
           logRequestService, partnerNotifyHub,
           errorCodeService, provider)
        {
            _userConnectionManager = userConnectionManager;
            _logNotify = logNotify;
            _logContent = logContent;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public string Get()
        {
            return Utilities.EncryptByKey(Utilities.Base64Encode(TimeExpiry.ToString()), ESCSConstants.HASHKEY);
        }
        /// <summary>
        /// Lấy thông tin version app
        /// </summary>
        /// <returns></returns>
        [Route("get-version")]
        [HttpPost]
        public async Task<IActionResult> GetVersion()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            header.envcode = "DEV";
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            #endregion
            #region Lấy thông tin action
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                return Ok(res);
            }
            if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                return NotFound();
            #endregion
            #region Thực thi type excute db
            if (action.action_type != "EXCUTE_DB")
                throw new Exception("Không xác định loại Action Type");

            //Gọi api sang bên đối tác lấy version nếu có
            if (action.actionapicode != ESCSStoredProcedures.PHT_MOBILE_APP_LKE)
                throw new Exception("Hành động không hợp lệ");

            var outPut = new Dictionary<string, object>();
            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; });
            return Ok(res);
            #endregion
        }
        [Route("check-connect-stored-shared")]
        [HttpGet]
        public string CheckConnectStoredShared()
        {
            NetworkCredential credentials = new NetworkCredential("admin", "ESCS@#123@");
            var netWorkname = @"\\10.9.8.73\wwwroot\FILE_CAM_XOA";
            try
            {
                using (new ConnectToSharedFolder(netWorkname, credentials))
                {
                    return "Kết nối thành công: "+ netWorkname;
                }
            }
            catch(Exception ex)
            {
                return "Kết nối không thành công: "+ netWorkname + " - "+ ex.Message;
            }

        }
    }
}
