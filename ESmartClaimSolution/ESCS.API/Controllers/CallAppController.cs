using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.CallApp;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.API.Hubs;
using ESCS.MODEL.OpenID.ModelView;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ESCS.MODEL.ESCS;
using ESCS.COMMON.Common;
using ESCS.COMMON.ESCSServices;

/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    [Route("api/esmartclaim")]
    [ApiController]
    public class CallAppController : BaseController
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly ILogMongoService<LogCallApp> _logCallApp;
        private readonly IOpenIdCallApp _openIdCallApp;

        /// <summary>
        /// CallAppController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public CallAppController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider,
            IUserConnectionManager userConnectionManager,
            ILogMongoService<LogContent> logContent,
            ILogMongoService<LogCallApp> logCallApp,
            IOpenIdCallApp openIdCallApp)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _userConnectionManager = userConnectionManager;
            _logContent = logContent;
            _logCallApp = logCallApp;
            _openIdCallApp = openIdCallApp;
        }
        [Route("authen-call")]
        [HttpPost]
        public async Task<IActionResult> Call()
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
            try
            {
                #region Lấy thông tin action
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
                #region Thực hiện xác thực và call video
                var ma_doi_tac = data_info["ma_doi_tac_nsd"];
                var nsd = data_info["nsd"];
                var pas = data_info["pas"];
                //xem có truyền lên user_id, nếu không truyền thì là kết nối mới, nếu có truyền thì là yêu cầu refresh_token
                string user_id_old = null;
                if (data_info.ContainsKey("user_id"))
                {
                    user_id_old = data_info["user_id"].ToString();
                }
                var time_live = new DateTimeOffset(DateTime.UtcNow.AddSeconds(CallAppConfiguration.TimeLiveTokenSeconds)).ToUnixTimeSeconds();

                BaseResponse<ht_nsd_call_id> response = new BaseResponse<ht_nsd_call_id>();
                var outPut = new Dictionary<string, object>();
                response.data_info = await _dynamicService.ExcuteSingleAsync<ht_nsd_call_id>(data_info, header, outValue =>
                {
                    response.out_value = outValue;
                });

                if (response.state_info.status== STATUS_NOTOK || response.data_info==null)
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Không tìm thấy thông tin call id của bạn.";
                    return Ok(res);
                }
                //Giải mã
                var decript_sid = Utilities.DecryptByKey(response.data_info.sid, AppSettings.KeyEryptData);
                response.data_info.sid = decript_sid ?? response.data_info.sid;
                var decript_secret = Utilities.DecryptByKey(response.data_info.secret, AppSettings.KeyEryptData);
                response.data_info.secret = decript_secret ?? response.data_info.secret;

                var user_id = "video_"+response.data_info.call_id;
                string access_token = _openIdCallApp.GetSignatureVerify(response.data_info, user_id, time_live);

                var from = response.data_info.sdt;
                BaseResponse<dynamic> res_success = new BaseResponse<dynamic>();
                res_success.state_info.status = STATUS_OK;
                res_success.data_info = new { access_token, user_id, time_live, from };
                res_success.state_info.message_body = "Authen thành công";
                res_success.state_info.message_code = "000";
                return Ok(res_success);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }
        [Route("answer-url")]
        [HttpGet]
        public IActionResult AnswerUrl()
        {
            StringValues from, to, videocall;
            Request.Query.TryGetValue("from", out from);
            Request.Query.TryGetValue("to", out to);
            Request.Query.TryGetValue("videocall", out videocall);
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(videocall))
            {
                throw new Exception("Thiếu thông tin tham số");
            }
            //from type = internal  và to type = internal  ==> app to app
            //from type = internal  và to type = external==> app to phone
            bool isVideo = bool.Parse(videocall);//video - app-to-app
            bool isUserId = to.ToString().StartsWith("video_");
            bool appToApp = isVideo || isUserId;
            List<AnswerResponse> res = new List<AnswerResponse>()
            {
                new AnswerResponse(from,to, appToApp)
            };
            _logCallApp.AddLogAsync(new LogCallApp(res));
            return Ok(res);
        }
        [Route("event-url")]
        [HttpGet]
        public IActionResult EventUrl()
        {
            return Ok();
        }
        [Route("check-connect-call")]
        [HttpPost]
        public async Task<IActionResult> CheckConnect()
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
            try
            {
                #region Lấy thông tin action
                var action = _dynamicService.GetConnection(header);
                if (action == null)
                {
                    BaseResponse<dynamic> res_err = new BaseResponse<dynamic>();
                    res_err.state_info.status = STATUS_NOTOK;
                    res_err.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res_err.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                    return Ok(res_err);
                }
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();
                #endregion
                if (action.action_type == "EXCUTE_DB")
                {
                    BaseResponse<ht_nsd_call_id> res_exc = new BaseResponse<ht_nsd_call_id>();
                    var outPut = new Dictionary<string, object>();
                    res_exc.data_info = await _dynamicService.ExcuteSingleAsync<ht_nsd_call_id>(data_info, header, outValue =>
                    {
                        res_exc.out_value = outValue;
                    });
                    if (res_exc.state_info.status == STATUS_NOTOK || res_exc.data_info == null)
                    {
                        BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                        res.state_info.message_body = "Không tìm thấy thông tin call id của bạn.";
                        return Ok(res);
                    }
                    //Giải mã
                    var decript_sid = Utilities.DecryptByKey(res_exc.data_info.sid, AppSettings.KeyEryptData);
                    res_exc.data_info.sid = decript_sid ?? res_exc.data_info.sid;
                    var decript_secret = Utilities.DecryptByKey(res_exc.data_info.secret, AppSettings.KeyEryptData);
                    res_exc.data_info.secret = decript_secret ?? res_exc.data_info.secret;


                    #region Thực hiện xác thực và call video
                    var ma_doi_tac = data_info["ma_doi_tac_nsd"].ToString();
                    var ma_chi_nhanh = data_info["ma_chi_nhanh_nsd"].ToString();
                    var nsd = data_info["nsd"].ToString();
                    var pas = data_info["pas"].ToString();
                    var user_id = data_info["user_id"].ToString();
                    if (!string.IsNullOrEmpty(user_id) && !user_id.StartsWith("video_"))
                        user_id = "video_"+user_id;

                    var time_live = new DateTimeOffset(DateTime.UtcNow.AddSeconds(CallAppConfiguration.TimeLiveTokenSeconds)).ToUnixTimeSeconds();
                    string token_stringee = _openIdCallApp.GetTokenRestApi(res_exc.data_info, time_live);
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(CallAppConfiguration.BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("X-STRINGEE-AUTH", token_stringee);
                        HttpResponseMessage response = await client.GetAsync("/v1/users?userId=" + user_id);
                        string restext = await response.Content.ReadAsStringAsync();

                        string contentLog = @"[CHECK_CONNECT_CALL] DOMAIN: " + CallAppConfiguration.BaseUrl + "\n";
                        contentLog += "[CHECK_CONNECT_CALL] URL: " + "/v1/users?userId=" + user_id + "\n";
                        contentLog += "[CHECK_CONNECT_CALL] METHOD: GET" + "\n";
                        contentLog += "[CHECK_CONNECT_CALL] TOKEN: " + token_stringee + "\n";
                        contentLog += "[CHECK_CONNECT_CALL] RESPONSE: " + restext + "\n";
                        AddLog(header, ma_doi_tac, nsd, "CHECK_CONNECT_CALL", user_id, contentLog);

                        if (response.IsSuccessStatusCode)
                        {
                            ResponseConnection connect = JsonConvert.DeserializeObject<ResponseConnection>(restext);
                            BaseResponse<ResponseConnection> res = new BaseResponse<ResponseConnection>();
                            res.state_info.status = STATUS_OK;
                            res.data_info = connect;
                            res.state_info.message_code = "CALL_APP_CHECK_CONNECT_000";
                            res.state_info.message_body = "Thành công";
                            return Ok(res);
                        }
                        else
                        {
                            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                            res.state_info.status = STATUS_NOTOK;
                            res.state_info.message_code = "CALL_APP_ERROR_STRINGEE_SERVER";
                            res.state_info.message_body = "Không kết nối được đến server Stringee";
                            return Ok(res);
                        }

                    }
                    #endregion
                }
                else
                {
                    throw new Exception("Không xác định loại Action Type");
                }
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

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
    }
}
