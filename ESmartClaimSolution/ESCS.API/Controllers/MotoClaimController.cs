using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.Auth;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Oracle;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.API.Hubs;
using ESCS.MODEL.ESCS;
using ESCS.MODEL.OpenID.ModelView;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using FirebaseAdmin.Messaging;
using System.Collections;
using System.IO;
using System.Web;
using ESCS.API.Attributes;
using ESCS.COMMON.Http;
using ESCS.COMMON.SMS.FPT;
using System.Data;
using ESCS.COMMON.SignaturePDF;
using ESCS.COMMON.Request.ApiGateway.MIC;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using ESCS.COMMON.SMS.MCM;
using ESCS.MODEL.HealthClaim;
using ESCS.COMMON.QRCodeManager;
using Microsoft.Extensions.Logging;
using ESCS.COMMON.ESCSServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ESCS.COMMON.Request.ApiGateway.OPES;

/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    /// <summary>
    /// Excute stored procedure
    /// </summary>
    [Route("api/motoclaim")]
    [ApiController]
    [ESCSAuth]
    public class MotoClaimController : BaseController
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogMongoService<LogSendNotify> _logNotify;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly ILogMongoService<LogSMSFPT> _logSMSFPT;
        private readonly ILogMongoService<LogFileAction> _logFileAction;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        /// <summary>
        /// ServiceController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public MotoClaimController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider,
            IUserConnectionManager userConnectionManager,
            ILogMongoService<LogSendNotify> logNotify,
            ILogMongoService<LogSMSFPT> logSMSFPT,
            ILogMongoService<LogFileAction> logFileAction,
            ILogMongoService<LogContent> logContent,
            IWebHostEnvironment env,
            ILogger<MotoClaimController> logger)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _userConnectionManager = userConnectionManager;
            _logNotify = logNotify;
            _logSMSFPT = logSMSFPT;
            _logContent = logContent;
            _logFileAction = logFileAction;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Lấy số hồ sơ
        /// Kiểm tra phân cấp
        /// </summary>
        /// <returns></returns>
        [Route("get-number-claim")]
        [HttpPost]
        public async Task<IActionResult> LaySoHS()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            var action = _dynamicService.GetConnection(header);
            if (action == null || action.actionapicode != ESCSStoredProcedures.PBH_BT_XE_MAY_GD_CAP_NHAT_SO_HS)
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
            #region Lấy dữ liệu cần chuyển
            BaseResponse<dynamic> response = new BaseResponse<dynamic>();
            response.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                response.out_value = outValue;
            });
            return Ok(response);
            #endregion
        }
        /// <summary>
        /// Hủy hồ sơ
        /// </summary>
        /// <returns></returns>
        [Route("destroy")]
        [HttpPost]
        public async Task<IActionResult> Destroy()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_GD_HUY && header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_HUY)
                throw new Exception("Không xác định hành động phê duyệt");
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { resData.out_value = outValue; });
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Gỡ hủy hồ sơ
        /// </summary>
        /// <returns></returns>
        [Route("undestroy")]
        [HttpPost]
        public async Task<IActionResult> UnDestroy()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_GD_GO_HUY && header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_GO_HUY)
                throw new Exception("Không xác định hành động phê duyệt");
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { resData.out_value = outValue; });
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Chuyển thanh toán
        /// </summary>
        /// <returns></returns>
        [Route("transfer-payment")]
        [HttpPost]
        public async Task<IActionResult> TransferPayment()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_CHUYEN_THANH_TOAN)
            {
                throw new Exception("Không xác định hành động phê duyệt");
            }
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                resData.out_value = outValue;
            });
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Hủy chuyển thanh toán
        /// </summary>
        /// <returns></returns>
        [Route("un-transfer-payment")]
        [HttpPost]
        public async Task<IActionResult> UnTransferPayment()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_HUY_CHUYEN_THANH_TOAN)
            {
                throw new Exception("Không xác định hành động phê duyệt");
            }
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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
            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                resData.out_value = outValue;
            });

            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Đóng hồ sơ
        /// </summary>
        /// <returns></returns>
        [Route("close-claim")]
        [HttpPost]
        public async Task<IActionResult> CloseClaim()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_DONG_HS)
            {
                throw new Exception("Không xác định hành động phê duyệt");
            }
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { resData.out_value = outValue; });
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Hủy chuyển thanh toán
        /// </summary>
        /// <returns></returns>
        [Route("un-close-claim")]
        [HttpPost]
        public async Task<IActionResult> UnCloseClaim()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_HUY_DONG_HS)
            {
                throw new Exception("Không xác định hành động phê duyệt");
            }
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                resData.out_value = outValue;
            });
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }

        /// <summary>
        /// Chuyển thanh toán + Đóng hồ sơ
        /// </summary>
        /// <returns></returns>
        [Route("mobile-close-claim")]
        [HttpPost]
        public async Task<IActionResult> MobileCloseClaim()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_MOBILE_CHUYEN_THANH_TOAN)
            {
                throw new Exception("Không xác định hành động phê duyệt");
            }
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { resData.out_value = outValue; });
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Hủy đóng thanh toán + Hủy chuyển thanh toán
        /// </summary>
        /// <returns></returns>
        [Route("mobile-un-close-claim")]
        [HttpPost]
        public async Task<IActionResult> MobileUnCloseClaim()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_MOBILE_HUY_CHUYEN_THANH_TOAN_TT)
            {
                throw new Exception("Không xác định hành động phê duyệt");
            }
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                resData.out_value = outValue;
            });
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Gen lại file
        /// </summary>
        /// <returns></returns>
        [Route("re-gennerate-file")]
        [HttpPost]
        public async Task<IActionResult> ReGeneratefile()
        {
            HeaderRequest header = Request.GetHeader();
            var data_info = Request.GetData(out var define_info, out string payload);
            header.envcode = "DEV";
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_GEN_FILE)
                throw new Exception("Không xác định ACTION_CODE");
            var danhSach = await _dynamicService.ExcuteListAsync<file_generate>(data_info, header, outValue => { });
            var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            var ma_chi_nhanh_nsd = data_info.GetString("ma_chi_nhanh_nsd");
            var nsd = data_info.GetString("nsd");
            var pas = data_info.GetString("pas");
            int dem = 0;
            if (danhSach != null && danhSach.Count() > 0)
            {
                foreach (var item in danhSach)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(ma_chi_nhanh_nsd) || string.IsNullOrEmpty(nsd)
                            || string.IsNullOrEmpty(pas) || string.IsNullOrEmpty(item.ma_doi_tac) || string.IsNullOrEmpty(item.so_id)
                            || string.IsNullOrEmpty(item.ngay_duyet) || (string.IsNullOrEmpty(item.create_file) && string.IsNullOrEmpty(item.create_file_sign)
                            && string.IsNullOrEmpty(item.remove_file)))
                            continue;
                        IDictionary<string, object> data_info_base = new Dictionary<string, object>();
                        data_info_base.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                        data_info_base.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                        data_info_base.AddWithExists("nsd", nsd);
                        data_info_base.AddWithExists("pas", pas);
                        data_info_base.AddWithExists("ma_doi_tac", item.ma_doi_tac);
                        data_info_base.AddWithExists("so_id", item.so_id);
                        data_info_base.AddWithExists("pm", "BT");
                        data_info_base.AddWithExists("ngay_ky", item.ngay_duyet);
                        data_info_base.AddWithExists("create_file", item.create_file);
                        data_info_base.AddWithExists("create_file_sign", item.create_file_sign);
                        data_info_base.AddWithExists("remove_file", item.remove_file);
                        var success = await ReFileAction(header, data_info_base);
                        if (success)
                            dem = dem + 1;
                    }
                    catch
                    {

                    }
                }
            }
            //
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            res.state_info.status = STATUS_OK;
            res.state_info.message_body = "Số file gen thành công " + dem;
            return Ok(res);
        }
        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("update-vat")]
        [HttpPost]
        public async Task<IActionResult> UpdateVAT()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
            #endregion
            #region Lấy thông tin action
            if (header.action != ESCSStoredProcedures.PBH_BT_XE_MAY_HS_SO_TIEN_THUE_NH)
                throw new Exception("Không xác định hành động phê duyệt");
            var action = _dynamicService.GetConnection(header);
            if (action == null)
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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

            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                resData.out_value = outValue;
            });
            var gen_file = data_info.GetString("gen_file");
            if (!string.IsNullOrEmpty(gen_file) && gen_file == "C")
                FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Bổ sung chứng từ
        /// </summary>
        /// <returns></returns>
        [Route("document")]
        [HttpPost]
        public async Task<IActionResult> Document()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner);
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = token.evncode;
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
            if (action.actionapicode != ESCSStoredProcedures.PBH_BT_HO_SO_GIAY_TO_LUU)
                throw new Exception("Không xác định action api");

            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; });
            try
            {
                var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                var so_id = data_info.GetString("so_id");
                var nv = data_info.GetString("nv");

                if (!string.IsNullOrEmpty(ma_doi_tac_nsd) && !string.IsNullOrEmpty(so_id) && !string.IsNullOrEmpty(nv))
                {
                    var filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE");
                    if (!Directory.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote()))
                        Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());

                    filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE", "BSCT" + nv + ma_doi_tac_nsd + so_id + ".png");
                    if (!System.IO.File.Exists(Path.Combine(ma_doi_tac_nsd, filePath)))
                    {
                        var textQRCode = AppSettings.QRBSHSCodeLink + "?hash=" + Utilities.EncryptByKey("ma_doi_tac=" + ma_doi_tac_nsd + "&nv=" + nv + "&so_id=" + so_id + "&loai=BSCT", AppSettings.KeyEryptData);
                        QRCodeUtils.GenerateQRCode(textQRCode, Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());
                        var headerClone = header.Clone();
                        headerClone.action = ESCSStoredProcedures.PBH_BT_HS_QRCODE_NH;
                        var data = new Dictionary<string, object>();
                        data.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                        data.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                        data.AddWithExists("nsd", data_info.GetString("nsd"));
                        data.AddWithExists("pas", data_info.GetString("pas"));
                        data.AddWithExists("so_id", so_id);
                        data.AddWithExists("nv", nv);
                        data.AddWithExists("loai", "BSCT");
                        data.AddWithExists("nguon", "");
                        data.AddWithExists("url_file", filePath);
                        await _dynamicService.ExcuteDynamicNewAsync(data, headerClone);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Ok(res);
            #endregion
        }
        /// <summary>
        /// Gửi thông báo tách luồng
        /// </summary>
        /// <param name="action"></param>
        /// <param name="headerBase"></param>
        /// <param name="data_info_base"></param>
        private void SendNotify(ActionConnection action, HeaderRequest headerBase, IDictionary<string, object> data_info_base)
        {

            if (action == null || string.IsNullOrEmpty(action.send_notify))
            {
                return;
            }
            var types = action.send_notify.Split(';');
            Task taskSendNotify = new Task(async () =>
            {
                try
                {
                    if (types.Contains("NOTIFY"))
                    {
                        #region SignalR
                        HeaderRequest header = headerBase.Clone();
                        header.action = ESCSStoredProcedures.PHT_THONG_BAO_GUI;
                        Dictionary<string, object> data_info = new Dictionary<string, object>();
                        data_info.AddWithExists("ma_doi_tac_nsd", data_info_base.GetString("ma_doi_tac_nsd"));
                        data_info.AddWithExists("ma_chi_nhanh_nsd", data_info_base.GetString("ma_chi_nhanh_nsd"));
                        data_info.AddWithExists("nsd", data_info_base.GetString("nsd"));
                        data_info.AddWithExists("pas", data_info_base.GetString("pas"));
                        data_info.AddWithExists("token", data_info_base.GetString("token"));
                        data_info.AddWithExists("nguon_api", data_info_base.GetString("nguon_api"));
                        data_info.AddWithExists("loai_thong_bao", "NOTIFY");

                        BaseResponse<IEnumerable<ht_thong_bao_gui>> res = new BaseResponse<IEnumerable<ht_thong_bao_gui>>();
                        var outPut = new Dictionary<string, object>();
                        res.data_info = await _dynamicService.ExcuteListAsync<ht_thong_bao_gui>(data_info, header, outValue => { });
                        try
                        {
                            if (res.data_info == null || res.data_info.Count() <= 0)
                            {
                                return;
                            }
                            //Thông báo gửi web
                            IEnumerable<ht_thong_bao_gui> notifyWeb = res.data_info.Where(n => n.ung_dung == "WEB");
                            if (notifyWeb != null)
                            {
                                foreach (var tb in notifyWeb)
                                {
                                    string key = "HUBCONNECTION:" + tb.ma_doi_tac + ":" + tb.nsd;
                                    List<string> dsConnectionUser = _cacheServer.Get<List<string>>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, key, RedisCacheMaster.DatabaseIndex);
                                    if (dsConnectionUser != null)
                                    {
                                        foreach (var connId in dsConnectionUser)
                                        {
                                            tb.connection_id = connId;
                                            await _partnerNotifyHub.Clients.Client(connId).SendAsync("sendToUser", tb);
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        #endregion
                        #region Fire base
                        try
                        {
                            IEnumerable<ht_thong_bao_gui> notifyMobile = res.data_info.Where(n => n.ung_dung == "MOBILE");
                            if (notifyMobile != null)
                            {
                                var query = from ms in notifyMobile
                                            group ms by new
                                            {
                                                gid = ms.gid,
                                                //Thông tin người nhận
                                                ma_doi_tac_nhan = ms.ma_doi_tac,
                                                ten_doi_tac_nhan = ms.ten_doi_tac,
                                                nsd_nhan = ms.nsd,
                                                ten_nsd_nhan = ms.ten_nsd,
                                                //Thông tin thông báo
                                                tieu_de = ms.tieu_de,
                                                nd = ms.nd,
                                                nd_tom_tat = ms.nd_tom_tat,
                                                tg_thong_bao = ms.tg_thong_bao,
                                                loai_thong_bao = ms.loai_thong_bao,
                                                loai_thong_bao_hthi = ms.loai_thong_bao_hthi,
                                                nguoi_gui = ms.nguoi_gui,
                                                doc_noi_dung = ms.doc_noi_dung,
                                                doc_noi_dung_hthi = ms.doc_noi_dung_hthi,
                                                canh_bao = ms.canh_bao,
                                                canh_bao_hthi = ms.canh_bao_hthi,
                                                tt_gui = ms.tt_gui,
                                                tt_gui_hthi = ms.tt_gui_hthi,
                                                connection_id = ms.connection_id,
                                                so_tn_chua_doc = ms.so_tn_chua_doc,
                                                //Thông tin hồ sơ
                                                ctiet_xem = ms.ctiet_xem,
                                                ctiet_hanh_dong = ms.ctiet_hanh_dong,
                                                ctiet_action_code = ms.ctiet_action_code,
                                                ma_doi_tac = ms.ctiet_ma_doi_tac,
                                                so_id = ms.ctiet_so_id,

                                                content_available = "1",
                                                mutable_content = "1",
                                                priority = "high",
                                                vibrate = true,
                                                lights = true
                                            } into gr
                                            select gr;
                                foreach (var item in query)
                                {
                                    MulticastMessage message = new MulticastMessage();
                                    message.Tokens = item.Select(n => n.token).ToList();
                                    message.Data = item.Key.ToDictionaryModel();
                                    var apns = new ApnsConfig();
                                    apns.Aps = new Aps() { Sound = "default" };
                                    message.Apns = apns;
                                    message.Notification = new Notification()
                                    {
                                        Title = item.Key.tieu_de,
                                        Body = item.Key.nd_tom_tat
                                    };

                                    var config = CoreApiConfig.Items.Where(n => n.Partner == item.Key.ma_doi_tac_nhan).FirstOrDefault();
                                    if (config != null && config.Partner == CoreApiConfigContants.OPES)
                                    {
                                        SendNotifyDataOpesDataInfo dataRequest = new SendNotifyDataOpesDataInfo();
                                        dataRequest.user = item.Key.ma_doi_tac_nhan + "/" + item.Key.nsd_nhan;
                                        dataRequest.device = "";
                                        dataRequest.title = item.Key.tieu_de;
                                        dataRequest.content = item.Key.nd_tom_tat;
                                        dataRequest.messageData = item.Key.ToDictionaryModel();
                                        var resNotify = await ApiGatewayOPES.SendNotify(item.Key.ma_doi_tac_nhan, dataRequest);
                                    }
                                    else
                                    {
                                        var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(true);
                                    }
                                }
                            }

                            IEnumerable<ht_thong_bao_gui> notifyMobileKH = res.data_info.Where(n => n.ung_dung == "MOBILE_KH");
                            if (notifyMobileKH != null)
                            {
                                var query = from ms in notifyMobileKH
                                            group ms by new
                                            {
                                                gid = ms.gid,
                                                //Thông tin người nhận
                                                ma_doi_tac_nhan = ms.ma_doi_tac,
                                                ten_doi_tac_nhan = ms.ten_doi_tac,
                                                nsd_nhan = ms.nsd,
                                                ten_nsd_nhan = ms.ten_nsd,
                                                //Thông tin thông báo
                                                tieu_de = ms.tieu_de,
                                                nd = ms.nd,
                                                nd_tom_tat = ms.nd_tom_tat,
                                                tg_thong_bao = ms.tg_thong_bao,
                                                loai_thong_bao = ms.loai_thong_bao,
                                                loai_thong_bao_hthi = ms.loai_thong_bao_hthi,
                                                nguoi_gui = ms.nguoi_gui,
                                                doc_noi_dung = ms.doc_noi_dung,
                                                doc_noi_dung_hthi = ms.doc_noi_dung_hthi,
                                                canh_bao = ms.canh_bao,
                                                canh_bao_hthi = ms.canh_bao_hthi,
                                                tt_gui = ms.tt_gui,
                                                tt_gui_hthi = ms.tt_gui_hthi,
                                                connection_id = ms.connection_id,
                                                so_tn_chua_doc = ms.so_tn_chua_doc,
                                                //Thông tin hồ sơ
                                                ctiet_xem = ms.ctiet_xem,
                                                ctiet_hanh_dong = ms.ctiet_hanh_dong,
                                                ctiet_action_code = ms.ctiet_action_code,
                                                ma_doi_tac = ms.ctiet_ma_doi_tac,
                                                so_id = ms.ctiet_so_id,

                                                content_available = "1",
                                                mutable_content = "1",
                                                priority = "high",
                                                vibrate = true,
                                                lights = true
                                            } into gr
                                            select gr;
                                foreach (var item in query)
                                {
                                    MulticastMessage message = new MulticastMessage();
                                    message.Tokens = item.Select(n => n.token).ToList();
                                    message.Data = item.Key.ToDictionaryModel();
                                    var apns = new ApnsConfig();
                                    apns.Aps = new Aps() { Sound = "default" };
                                    message.Apns = apns;
                                    message.Notification = new Notification()
                                    {
                                        Title = item.Key.tieu_de,
                                        Body = item.Key.nd_tom_tat
                                    };

                                    var config = CoreApiConfig.Items.Where(n => n.Partner == item.Key.ma_doi_tac_nhan).FirstOrDefault();
                                    if (config != null && config.Partner == CoreApiConfigContants.OPES)
                                    {
                                        SendNotifyDataOpesDataInfo dataRequest = new SendNotifyDataOpesDataInfo();
                                        dataRequest.user = item.Key.ma_doi_tac_nhan + "/" + item.Key.nsd_nhan;
                                        dataRequest.device = "";
                                        dataRequest.title = item.Key.tieu_de;
                                        dataRequest.content = item.Key.nd_tom_tat;
                                        dataRequest.messageData = item.Key.ToDictionaryModel();
                                        await ApiGatewayOPES.SendNotify(item.Key.ma_doi_tac_nhan, dataRequest);
                                    }
                                    else if (config != null && config.Partner == CoreApiConfigContants.MIC)
                                    {
                                        SendNotifyDataMIC dataRequest = new SendNotifyDataMIC();
                                        dataRequest.phoneNumber = item.Key.nsd_nhan;
                                        dataRequest.content = item.Key.nd_tom_tat;
                                        dataRequest.title = item.Key.tieu_de;
                                        dataRequest.id = item.Key.so_id;
                                        await ApiGateway.CallApiSendNotify(item.Key.ma_doi_tac_nhan, dataRequest);
                                    }
                                    else
                                    {
                                        var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(true);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //_logContent.AddLogAsync(new LogContent() { content = "Đã có lỗi xảy ra trong quá trình gửi notify firebase: " + ex.Message });
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {

                }
            });
            taskSendNotify.Start();
        }
        /// <summary>
        /// Gửi SMS tách luồng
        /// </summary>
        /// <param name="action"></param>
        /// <param name="headerBase"></param>
        /// <param name="data_info_base"></param>
        private void SendSMS(ActionConnection action, HeaderRequest headerBase, IDictionary<string, object> data_info_base)
        {
            if (action == null || string.IsNullOrEmpty(action.send_notify))
                return;
            if (action.send_notify == null || !action.send_notify.Contains("SMS"))
                return;
            Task taskSendSMS = new Task(async () =>
            {
                try
                {
                    HeaderRequest header = headerBase.Clone();
                    header.action = ESCSStoredProcedures.PBH_BT_GUI_SMS_LKE_GUI;
                    Dictionary<string, object> data_info = new Dictionary<string, object>();
                    data_info.AddWithExists("ma_doi_tac_nsd", data_info_base.GetString("ma_doi_tac_nsd"));
                    data_info.AddWithExists("ma_chi_nhanh_nsd", data_info_base.GetString("ma_chi_nhanh_nsd"));
                    data_info.AddWithExists("nsd", data_info_base.GetString("nsd"));
                    data_info.AddWithExists("pas", data_info_base.GetString("pas"));
                    BaseResponse<IEnumerable<bh_bt_gui_sms>> res = new BaseResponse<IEnumerable<bh_bt_gui_sms>>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteListAsync<bh_bt_gui_sms>(data_info, header, outValue => { });
                    if (res.data_info == null || res.data_info.Count() <= 0)
                        return;
                    string ma_doi_tac_nsd = data_info_base.GetString("ma_doi_tac_nsd");
                    string nsd = data_info.GetString("nsd");
                    var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();

                    if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
                    {
                        foreach (var item in res.data_info)
                        {
                            try
                            {
                                item.ma_doi_tac_nsd = ma_doi_tac_nsd;
                                var dataRequest = ApiGateway.RequestApiSendSMS(item);
                                string json = await ApiGateway.CallApiSendSMS(ma_doi_tac_nsd, dataRequest);
                                AddLog(header, ma_doi_tac_nsd, nsd, "SEND_SMS", JsonConvert.SerializeObject(dataRequest), json);
                            }
                            catch
                            {

                            }
                        }
                    }
                    else if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
                    {
                        foreach (var item in res.data_info)
                        {
                            try
                            {
                                item.ma_doi_tac_nsd = ma_doi_tac_nsd;
                                var dataRequest = ApiGatewayOPES.RequestApiSendSMS(item);
                                string json = await ApiGatewayOPES.CallApiSendSMS(ma_doi_tac_nsd, dataRequest);
                                AddLog(header, ma_doi_tac_nsd, nsd, "SEND_SMS", JsonConvert.SerializeObject(dataRequest), json);
                            }
                            catch (Exception ex)
                            {
                                item.ma_doi_tac_nsd = ma_doi_tac_nsd;
                                var dataRequest = ApiGatewayOPES.RequestApiSendSMS(item);
                                AddLog(header, ma_doi_tac_nsd, nsd, "SEND_SMS", JsonConvert.SerializeObject(dataRequest), ex.Message);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            foreach (var item in res.data_info)
                            {
                                if (item.doi_tac_sms == ESCSConstants.DOI_TAC_SMS_FPT)
                                {
                                    var fptSMSService = new FPTSMSService();
                                    var fpt_token = new fpt_response_token();
                                    var keyCacheTokenSMS = "TOKEN_SMS:" + ESCSConstants.DOI_TAC_SMS_FPT + ":CLIENTID_" + item.client_id;
                                    string jsonToken = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCacheTokenSMS, RedisCacheMaster.DatabaseIndex);
                                    if (string.IsNullOrEmpty(jsonToken))
                                    {
                                        var request_token = new fpt_request_token(item.client_id, item.secret);
                                        fpt_token = await fptSMSService.GetToken(item.base_url, item.api_auth, request_token);
                                        if (fpt_token.error != null)
                                        {
                                            AddLog(header, ma_doi_tac_nsd, nsd, "SEND_SMS_GET_TOKEN", JsonConvert.SerializeObject(request_token), JsonConvert.SerializeObject(fpt_token));
                                            continue;
                                        }
                                        _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint,
                                                keyCacheTokenSMS, JsonConvert.SerializeObject(fpt_token),
                                                DateTime.Now.AddSeconds(fpt_token.expires_in) - DateTime.Now,
                                                RedisCacheMaster.DatabaseIndex);
                                    }
                                    else
                                    {
                                        fpt_token = JsonConvert.DeserializeObject<fpt_response_token>(jsonToken);
                                    }
                                    var request = new fpt_request_send_sms(fpt_token.access_token, item.brandname, item.sdt_nhan, Utilities.Base64Encode(item.noi_dung));
                                    var resSendSMS = await fptSMSService.SendSMS(item.base_url, item.api_send_sms, request);
                                    AddLog(header, ma_doi_tac_nsd, nsd, "SEND_SMS", JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(resSendSMS));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLog(header, ma_doi_tac_nsd, nsd, "SEND_SMS_EXCEPTION", "", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
            taskSendSMS.Start();

            Task taskSendMCM = new Task(async () =>
            {
                try
                {
                    HeaderRequest header = headerBase.Clone();
                    header.action = ESCSStoredProcedures.PBH_DICH_VU_MCM_LICH_GUI_LKE;
                    Dictionary<string, object> data_info = new Dictionary<string, object>();
                    data_info.AddWithExists("ma_doi_tac_nsd", data_info_base.GetString("ma_doi_tac_nsd"));
                    data_info.AddWithExists("ma_doi_tac", data_info_base.GetString("ma_doi_tac_nsd"));
                    data_info.AddWithExists("ma_chi_nhanh_nsd", data_info_base.GetString("ma_chi_nhanh_nsd"));
                    data_info.AddWithExists("nsd", data_info_base.GetString("nsd"));
                    data_info.AddWithExists("pas", data_info_base.GetString("pas"));
                    BaseResponse<mcm_gui_sms> res = new BaseResponse<mcm_gui_sms>();
                    var outPut = new Dictionary<string, object>();
                    var data = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                    var json = JsonConvert.SerializeObject(data);
                    res.data_info = JsonConvert.DeserializeObject<mcm_gui_sms>(json);
                    if (res.data_info == null || res.data_info.lich_gui == null || res.data_info.lich_gui.Count() <= 0)
                        return;
                    foreach (var item in res.data_info.lich_gui)
                    {
                        try
                        {
                            mcm_lich_gui lich_gui = item;
                            List<mcm_lich_gui_param> lich_gui_param = res.data_info.lich_gui_param.Where(n => n.ma_doi_tac == lich_gui.ma_doi_tac && n.bt == lich_gui.bt).ToList();

                            var chanels = lich_gui.channel.ToLower().Split(",").ToList();
                            if (chanels.Count >= 2)
                            {
                                mcm_request rq = MCMService.GetRequest(header.partner_code, res.data_info.dich_vu, lich_gui, lich_gui_param);
                                var jsonRequestSMSMCM = JsonConvert.SerializeObject(rq);
                                var resSMSMCM = await MCMService.SendMultipleChanel(rq);

                                header.action = ESCSStoredProcedures.PBH_DICH_VU_MCM_LICH_GUI_UPDATE;
                                data_info.AddWithExists("bt", item.bt);
                                data_info.AddWithExists("rq_send", JsonConvert.SerializeObject(rq));
                                data_info.AddWithExists("res_send", JsonConvert.SerializeObject(resSMSMCM));
                                data_info.AddWithExists("smsid", resSMSMCM.SMSID);
                                data_info.AddWithExists("partner_code", header.partner_code);
                                var dataRes = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                            }
                            if (chanels.Count == 1)
                            {
                                var chanel = chanels.FirstOrDefault().ToUpper();
                                if (chanel == "SMS")
                                {
                                    mcm_request_sms_single rq = MCMService.GetRequestSMS(header.partner_code, res.data_info.dich_vu, lich_gui, lich_gui_param);
                                    var jsonRequestSMSMCM = JsonConvert.SerializeObject(rq);
                                    var resSMSMCM = await MCMService.SendSMS(rq);
                                    header.action = ESCSStoredProcedures.PBH_DICH_VU_MCM_LICH_GUI_UPDATE;
                                    data_info.AddWithExists("bt", item.bt);
                                    data_info.AddWithExists("rq_send", JsonConvert.SerializeObject(rq));
                                    data_info.AddWithExists("res_send", JsonConvert.SerializeObject(resSMSMCM));
                                    data_info.AddWithExists("smsid", resSMSMCM.SMSID);
                                    data_info.AddWithExists("partner_code", header.partner_code);
                                    var dataRes = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLog(header, data_info_base.GetString("ma_doi_tac_nsd"), data_info_base.GetString("nsd"), "MCM_ERROR", "", ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
            taskSendMCM.Start();
        }
        /// <summary>
        /// Gửi Email tách luồng
        /// </summary>
        /// <param name="action"></param>
        /// <param name="headerBase"></param>
        /// <param name="data_info_base"></param>
        private void FileAction(ActionConnection action, HeaderRequest headerBase, IDictionary<string, object> data_info_base)
        {
            if (action == null || action.is_file != "C")
                return;
            List<string> arrCreateFile = new List<string>();
            List<string> arrCreateFileSign = new List<string>();
            List<string> arrRemoveFile = new List<string>();

            var pm = data_info_base.GetString("pm") != "" ? data_info_base.GetString("pm") : "GD";
            var create_file = data_info_base.GetString("create_file");
            var create_file_sign = data_info_base.GetString("create_file_sign");
            var remove_file = data_info_base.GetString("remove_file");
            var so_id_dt = data_info_base.GetString("so_id_dt");
            if (string.IsNullOrEmpty(so_id_dt))
                so_id_dt = "0";
            if (!string.IsNullOrEmpty(create_file))
                arrCreateFile = create_file.Split(",").Where(n => !string.IsNullOrEmpty(n)).Select(n => n.Trim()).ToList();
            if (!string.IsNullOrEmpty(create_file_sign))
                arrCreateFileSign = create_file_sign.Split(",").Where(n => !string.IsNullOrEmpty(n)).Select(n => n.Trim()).ToList();
            if (!string.IsNullOrEmpty(remove_file))
                arrRemoveFile = remove_file.Split(",").Where(n => !string.IsNullOrEmpty(n)).Select(n => n.Trim()).ToList();
            LogFileAction fileAction = new LogFileAction();

            var ma_doi_tac_nsd = data_info_base.GetString("ma_doi_tac_nsd");
            var ma_chi_nhanh_nsd = data_info_base.GetString("ma_chi_nhanh_nsd");
            var nsd = data_info_base.GetString("nsd");
            var pas = data_info_base.GetString("pas");

            fileAction.ma_doi_tac = data_info_base.GetString("ma_doi_tac_nsd");
            fileAction.so_id = data_info_base.GetString("so_id");
            if (string.IsNullOrEmpty(fileAction.ma_doi_tac) || string.IsNullOrEmpty(fileAction.so_id))
            {
                fileAction.message = "Thiếu thông tin trình tạo file (ma_doi_tac, so_id)";
                _logFileAction.AddLogAsync(fileAction);
                return;
            }
            Task taskFile = new Task(async () =>
            {
                foreach (var item in arrRemoveFile)
                {
                    try
                    {
                        HeaderRequest header = headerBase.Clone();
                        header.action = ESCSStoredProcedures.PBH_FILE_OPENID_LKE_CT;
                        data_info_base.AddWithExists("ma_file", item);
                        var file = await _dynamicService.ExcuteSingleAsync<bh_file>(data_info_base, header);
                        if (file != null)
                        {
                            var sourceFile = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                            if (System.IO.File.Exists(sourceFile))
                            {
                                System.IO.File.Delete(sourceFile);
                                var headerSave = headerBase.Clone();
                                headerSave.action = ESCSStoredProcedures.PBH_FILE_OPENID_XOA;
                                IDictionary<string, object> paramFile = new Dictionary<string, object>();
                                paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                                paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                                paramFile.AddWithExists("nsd", nsd);
                                paramFile.AddWithExists("pas", pas);
                                paramFile.AddWithExists("ma_doi_tac", fileAction.ma_doi_tac);
                                paramFile.AddWithExists("so_id", fileAction.so_id);
                                paramFile.AddWithExists("ma_file", item);
                                var kq = await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                foreach (var item in arrCreateFile)
                {
                    try
                    {
                        HeaderRequest header = headerBase.Clone();
                        header.action = ESCSStoredProcedures.PHT_MAU_IN_LKE_CT;
                        data_info_base.AddWithExists("ma", item);
                        var mau_in = await _dynamicService.ExcuteSingleAsync<ht_mau_in>(data_info_base, header);
                        if (mau_in != null)
                        {
                            HeaderRequest headerData = headerBase.Clone();
                            headerData.action = mau_in.ma_action_api;
                            DataSet ds = await _dynamicService.GetMultipleToDataSetAsync(data_info_base, headerData);
                            byte[] arrByte = null;
                            arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, mau_in.url_file);
                            var now = DateTime.Now;
                            string fileNameOutPut = "file_" + Guid.NewGuid().ToString("N") + ".pdf";
                            var month = now.Month.ToString();
                            var day = now.Day.ToString();
                            if (month.Length < 2)
                                month = "0" + month;
                            if (day.Length < 2)
                                day = "0" + day;

                            string pathFileOutput = Path.Combine(fileAction.ma_doi_tac, "TAI_LIEU", now.Year.ToString(), month, day);
                            string output = Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput, fileNameOutPut).ChuanHoaDuongDanRemote();
                            var path = Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput).ChuanHoaDuongDanRemote();
                            Directory.CreateDirectory(path);
                            System.IO.File.WriteAllBytes(output, arrByte);

                            var headerSave = headerBase.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LUU_KY_SO;
                            IDictionary<string, object> paramFile = new Dictionary<string, object>();
                            paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                            paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                            paramFile.AddWithExists("nsd", nsd);
                            paramFile.AddWithExists("pas", pas);
                            paramFile.AddWithExists("ma_doi_tac", fileAction.ma_doi_tac);
                            paramFile.AddWithExists("so_id", fileAction.so_id);
                            paramFile.AddWithExists("so_id_dt", so_id_dt);
                            paramFile.AddWithExists("pm", pm);
                            paramFile.AddWithExists("ma_file", item);
                            paramFile.AddWithExists("ten_file", mau_in.ten);
                            paramFile.AddWithExists("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                            paramFile.AddWithExists("trang_thai", "1");
                            paramFile.AddWithExists("x", "0");
                            paramFile.AddWithExists("y", "0");
                            var kq = await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                        }

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            var headerSave = headerBase.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LOG_NH;
                            IDictionary<string, object> paramFile = new Dictionary<string, object>();
                            paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                            paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                            paramFile.AddWithExists("nsd", nsd);
                            paramFile.AddWithExists("pas", pas);
                            paramFile.AddWithExists("so_id", fileAction.so_id);
                            paramFile.AddWithExists("ma_file", item);
                            paramFile.AddWithExists("loi", ex.Message);
                            await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                        }
                        catch { }
                    }
                }
                foreach (var item in arrCreateFileSign)
                {
                    try
                    {
                        HeaderRequest header = headerBase.Clone();
                        header.action = ESCSStoredProcedures.PHT_MAU_IN_LKE_CT;
                        data_info_base.AddWithExists("ma", item);
                        var mau_in = await _dynamicService.ExcuteSingleAsync<ht_mau_in>(data_info_base, header);
                        if (mau_in != null)
                        {
                            HeaderRequest headerData = headerBase.Clone();
                            headerData.action = mau_in.ma_action_api;
                            DataSet ds = await _dynamicService.GetMultipleToDataSetAsync(data_info_base, headerData);

                            byte[] arrByte = null;

                            if (!SignatureFileConfig.Enable)
                                arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, mau_in.url_file);

                            if (SignatureFileConfig.Enable && !SignatureFileConfig.Online)
                            {
                                string urlImgWatermark = Path.Combine(AppSettings.PathFolderNotDeleteFull, fileAction.ma_doi_tac, "LOGO", "signature.png").ChuanHoaDuongDanRemote();
                                var image = Utilities.DrawLetter("Ký bởi: " + AppSettings.SignatureName + "\nKý ngày: " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss"), urlImgWatermark);
                                arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, mau_in.url_file, urlImgWatermark, image);
                            }
                            if (SignatureFileConfig.Enable && SignatureFileConfig.Online)
                            {
                                arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, mau_in.url_file);
                                //Gọi api ký file ở đây
                                if (SignatureFileConfig.Partner == CoreApiConfigContants.OPES)
                                {
                                    if (mau_in.trang == null || mau_in.x == null || mau_in.width == null || mau_in.height == null ||
                                        mau_in.trang < 0 || mau_in.x < 0 || mau_in.y < 0 || mau_in.width <= 0 || mau_in.height <= 0
                                    )
                                        continue;
                                    arrByte = await ApiGatewayOPES.KySoFile(ma_doi_tac_nsd, arrByte, mau_in.trang, mau_in.x, mau_in.y, mau_in.width, mau_in.height, mau_in.signer);
                                }

                                ////var position = Utilities.GetPositionSignature(AppSettings.PathFolderNotDeleteFull, mau_in.url_file);
                                //arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, mau_in.url_file);
                                //var position = Utilities.GetPositionSignature(arrByte);
                                ////Gọi api ký file ở đây
                                //if (SignatureFileConfig.Partner == CoreApiConfigContants.OPES)
                                //{
                                //    if (mau_in.trang == null || mau_in.x == null || mau_in.width == null || mau_in.height == null ||
                                //        mau_in.trang < 0 || mau_in.x < 0 || mau_in.y < 0 || mau_in.width <= 0 || mau_in.height <= 0)
                                //        continue;
                                //    if (position.X == null || position.Y ==null|| (position.X == 0 && position.Y == 0))
                                //    {
                                //        position.X = mau_in.x;
                                //        position.Y = mau_in.y;
                                //    }
                                //    if (position.pageCount == null || position.pageCount==0)
                                //    {
                                //        position.pageCount = mau_in.trang;
                                //    }
                                //    arrByte = await ApiGatewayOPES.KySoFile(ma_doi_tac_nsd, arrByte, position.pageCount, position.X, position.Y, mau_in.width, mau_in.height, mau_in.signer);
                                //}
                            }

                            var now = DateTime.Now;
                            string fileNameOutPut = "file_ky_so_" + Guid.NewGuid().ToString("N") + ".pdf";
                            string pathFileOutput = Path.Combine(fileAction.ma_doi_tac, "FILE_KY_SO", now.Year.ToString(), now.Month.ToString(), now.Day.ToString());

                            string output = Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput, fileNameOutPut).ChuanHoaDuongDanRemote();
                            Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput).ChuanHoaDuongDanRemote());
                            System.IO.File.WriteAllBytes(output, arrByte);

                            var headerSave = headerBase.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LUU_KY_SO;
                            IDictionary<string, object> paramFile = new Dictionary<string, object>();
                            paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                            paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                            paramFile.AddWithExists("nsd", nsd);
                            paramFile.AddWithExists("pas", pas);
                            paramFile.AddWithExists("ma_doi_tac", fileAction.ma_doi_tac);
                            paramFile.AddWithExists("so_id", fileAction.so_id);
                            paramFile.AddWithExists("so_id_dt", so_id_dt);
                            paramFile.AddWithExists("pm", pm);
                            paramFile.AddWithExists("ma_file", item);
                            paramFile.AddWithExists("ten_file", mau_in.ten);
                            paramFile.AddWithExists("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                            paramFile.AddWithExists("trang_thai", "1");
                            paramFile.AddWithExists("x", "0");
                            paramFile.AddWithExists("y", "0");
                            var kq = await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            var headerSave = headerBase.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LOG_NH;
                            IDictionary<string, object> paramFile = new Dictionary<string, object>();
                            paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                            paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                            paramFile.AddWithExists("nsd", nsd);
                            paramFile.AddWithExists("pas", pas);
                            paramFile.AddWithExists("so_id", fileAction.so_id);
                            paramFile.AddWithExists("ma_file", item);
                            paramFile.AddWithExists("loi", ex.Message);
                            await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                        }
                        catch { }
                    }
                }
            });
            taskFile.Start();
        }
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="header"></param>
        /// <param name="ma_doi_tac"></param>
        /// <param name="nsd"></param>
        /// <param name="ma_api"></param>
        /// <param name="data_request"></param>
        /// <param name="data_response"></param>
        /// <returns></returns>
        private void AddLog(HeaderRequest header, string ma_doi_tac, string nsd, string ma_api, string data_request, string data_response)
        {
            try
            {
                Task task = new Task(async () =>
                {
                    if (ESCSServiceConfig.UseService)
                        await ESCSService.AddLogTichHop(ma_doi_tac, nsd, ma_api, data_request, data_response);
                    else
                    {
                        #region Code cũ
                        IDictionary<string, object> dataRequest = new Dictionary<string, object>();
                        dataRequest.AddWithExists("ma_doi_tac", ma_doi_tac);
                        dataRequest.AddWithExists("nsd", nsd);
                        dataRequest.AddWithExists("ma_api", ma_api);
                        dataRequest.AddWithExists("data_request", data_request);
                        dataRequest.AddWithExists("data_response", data_response);
                        var headerGateway = header.Clone();
                        headerGateway.action = ESCSStoredProcedures.PBH_TICH_HOP_API_LOG_NH;
                        try { await _dynamicService.ExcuteDynamicNewAsync(dataRequest, headerGateway); } catch { };
                        #endregion
                    }
                });
                task.Start();
            }
            catch (Exception ex) { }
        }
        private async Task<bool> ReFileAction(HeaderRequest headerBase, IDictionary<string, object> data_info_base)
        {
            try
            {
                List<string> arrCreateFile = new List<string>();
                List<string> arrCreateFileSign = new List<string>();
                List<string> arrRemoveFile = new List<string>();

                var pm = data_info_base.GetString("pm") != "" ? data_info_base.GetString("pm") : "GD";
                var create_file = data_info_base.GetString("create_file");
                var create_file_sign = data_info_base.GetString("create_file_sign");
                var remove_file = data_info_base.GetString("remove_file");
                if (!string.IsNullOrEmpty(create_file))
                    arrCreateFile = create_file.Split(",").Where(n => !string.IsNullOrEmpty(n)).Select(n => n.Trim()).ToList();
                if (!string.IsNullOrEmpty(create_file_sign))
                    arrCreateFileSign = create_file_sign.Split(",").Where(n => !string.IsNullOrEmpty(n)).Select(n => n.Trim()).ToList();
                if (!string.IsNullOrEmpty(remove_file))
                    arrRemoveFile = remove_file.Split(",").Where(n => !string.IsNullOrEmpty(n)).Select(n => n.Trim()).ToList();
                LogFileAction fileAction = new LogFileAction();

                var ma_doi_tac_nsd = data_info_base.GetString("ma_doi_tac_nsd");
                var ma_chi_nhanh_nsd = data_info_base.GetString("ma_chi_nhanh_nsd");
                var nsd = data_info_base.GetString("nsd");
                var pas = data_info_base.GetString("pas");

                fileAction.ma_doi_tac = data_info_base.GetString("ma_doi_tac");
                fileAction.so_id = data_info_base.GetString("so_id");
                if (string.IsNullOrEmpty(fileAction.ma_doi_tac) || string.IsNullOrEmpty(fileAction.so_id))
                {
                    fileAction.message = "Thiếu thông tin trình tạo file (ma_doi_tac, so_id)";
                    return false;
                }
                foreach (var item in arrRemoveFile)
                {
                    HeaderRequest header = headerBase.Clone();
                    header.action = ESCSStoredProcedures.PBH_FILE_OPENID_LKE_CT;
                    data_info_base.AddWithExists("ma_file", item);
                    var file = await _dynamicService.ExcuteSingleAsync<bh_file>(data_info_base, header);
                    if (file != null)
                    {
                        var sourceFile = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                        if (System.IO.File.Exists(sourceFile))
                        {
                            System.IO.File.Delete(sourceFile);
                            var headerSave = headerBase.Clone();
                            headerSave.action = ESCSStoredProcedures.PBH_FILE_OPENID_XOA;
                            IDictionary<string, object> paramFile = new Dictionary<string, object>();
                            paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                            paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                            paramFile.AddWithExists("nsd", nsd);
                            paramFile.AddWithExists("pas", pas);
                            paramFile.AddWithExists("ma_doi_tac", fileAction.ma_doi_tac);
                            paramFile.AddWithExists("so_id", fileAction.so_id);
                            paramFile.AddWithExists("ma_file", item);
                            var kq = await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                        }
                    }
                }
                foreach (var item in arrCreateFile)
                {
                    HeaderRequest header = headerBase.Clone();
                    header.action = ESCSStoredProcedures.PHT_MAU_IN_LKE_CT;
                    data_info_base.AddWithExists("ma", item);
                    var mau_in = await _dynamicService.ExcuteSingleAsync<ht_mau_in>(data_info_base, header);
                    if (mau_in != null)
                    {
                        HeaderRequest headerData = headerBase.Clone();
                        headerData.action = mau_in.ma_action_api;
                        DataSet ds = await _dynamicService.GetMultipleToDataSetAsync(data_info_base, headerData);
                        byte[] arrByte = null;
                        arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, mau_in.url_file);
                        var now = DateTime.Now;
                        string fileNameOutPut = "file_" + Guid.NewGuid().ToString("N") + ".pdf";
                        var month = now.Month.ToString();
                        var day = now.Day.ToString();
                        if (month.Length < 2)
                            month = "0" + month;
                        if (day.Length < 2)
                            day = "0" + day;
                        string pathFileOutput = Path.Combine(fileAction.ma_doi_tac, "TAI_LIEU", now.Year.ToString(), month, day);
                        string output = Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput, fileNameOutPut).ChuanHoaDuongDanRemote();
                        Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput).ChuanHoaDuongDanRemote());
                        System.IO.File.WriteAllBytes(output, arrByte);

                        var headerSave = headerBase.Clone();
                        headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LUU_KY_SO;
                        IDictionary<string, object> paramFile = new Dictionary<string, object>();
                        paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                        paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                        paramFile.AddWithExists("nsd", nsd);
                        paramFile.AddWithExists("pas", pas);
                        paramFile.AddWithExists("ma_doi_tac", fileAction.ma_doi_tac);
                        paramFile.AddWithExists("so_id", fileAction.so_id);
                        paramFile.AddWithExists("so_id_dt", "0");
                        paramFile.AddWithExists("pm", pm);
                        paramFile.AddWithExists("ma_file", item);
                        paramFile.AddWithExists("ten_file", mau_in.ten);
                        paramFile.AddWithExists("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                        paramFile.AddWithExists("trang_thai", "1");
                        paramFile.AddWithExists("x", "0");
                        paramFile.AddWithExists("y", "0");
                        var kq = await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                    }
                }
                foreach (var item in arrCreateFileSign)
                {
                    HeaderRequest header = headerBase.Clone();
                    header.action = ESCSStoredProcedures.PHT_MAU_IN_LKE_CT;
                    data_info_base.AddWithExists("ma", item);
                    var mau_in = await _dynamicService.ExcuteSingleAsync<ht_mau_in>(data_info_base, header);
                    if (mau_in != null)
                    {
                        HeaderRequest headerData = headerBase.Clone();
                        headerData.action = mau_in.ma_action_api;
                        DataSet ds = await _dynamicService.GetMultipleToDataSetAsync(data_info_base, headerData);
                        string urlImgWatermark = Path.Combine(AppSettings.PathFolderNotDeleteFull, fileAction.ma_doi_tac, "LOGO", "signature.png").ChuanHoaDuongDanRemote();
                        var image = Utilities.DrawLetter("Ký bởi: " + AppSettings.SignatureName + "\nKý ngày: " + data_info_base.GetString("ngay_ky"), urlImgWatermark);
                        byte[] arrByte = null;
                        arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, mau_in.url_file, urlImgWatermark, image);
                        var now = DateTime.Now;
                        string fileNameOutPut = "file_ky_so_" + Guid.NewGuid().ToString("N") + ".pdf";
                        string pathFileOutput = Path.Combine(fileAction.ma_doi_tac, "FILE_KY_SO", now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
                        string output = Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput, fileNameOutPut).ChuanHoaDuongDanRemote();
                        Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput).ChuanHoaDuongDanRemote());
                        System.IO.File.WriteAllBytes(output, arrByte);
                        var headerSave = headerBase.Clone();
                        headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LUU_KY_SO;
                        IDictionary<string, object> paramFile = new Dictionary<string, object>();
                        paramFile.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                        paramFile.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                        paramFile.AddWithExists("nsd", nsd);
                        paramFile.AddWithExists("pas", pas);
                        paramFile.AddWithExists("ma_doi_tac", fileAction.ma_doi_tac);
                        paramFile.AddWithExists("so_id", fileAction.so_id);
                        paramFile.AddWithExists("so_id_dt", "0");
                        paramFile.AddWithExists("pm", pm);
                        paramFile.AddWithExists("ma_file", item);
                        paramFile.AddWithExists("ten_file", mau_in.ten);
                        paramFile.AddWithExists("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                        paramFile.AddWithExists("trang_thai", "1");
                        paramFile.AddWithExists("x", "0");
                        paramFile.AddWithExists("y", "0");
                        var kq = await _dynamicService.ExcuteDynamicNewAsync(paramFile, headerSave);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}