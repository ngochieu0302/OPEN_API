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
using Newtonsoft.Json;
using ESCS.COMMON.ESCSServices;

/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    [Route("api/esmartclaim")]
    [ApiController]
    public class NotifyController : BaseController
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogMongoService<LogSendNotify> _logNotify;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// ServiceController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public NotifyController(
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
        /// <summary>
        /// Kiểm tra kết nối chat
        /// </summary>
        /// <returns></returns>
        [Route("check-connect")]
        [HttpPost]
        public IActionResult CheckConnect(BaseRequest<kiem_tra_ket_noi> req)
        {
            BaseResponse<kiem_tra_ket_noi> res = new BaseResponse<kiem_tra_ket_noi>();
            if (req==null || 
                req.data_info==null || 
                string.IsNullOrEmpty(req.data_info.ma_doi_tac_nsd)||
                string.IsNullOrEmpty(req.data_info.ma_chi_nhanh_nsd) ||
                string.IsNullOrEmpty(req.data_info.nsd) ||
                string.IsNullOrEmpty(req.data_info.pas) ||
                string.IsNullOrEmpty(req.data_info.ma_doi_tac) ||
                req.data_info.so_id ==null ||
                req.data_info.ds_ket_noi == null ||
                req.data_info.ds_ket_noi.Count <=0)
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = "500";
                res.state_info.message_body = "Thiếu thông tin kiếm tra kết nối";
                return Ok(res);
            }
            kiem_tra_ket_noi connects = req.data_info;
            foreach (var conn in connects.ds_ket_noi)
            {
                string key = req.data_info.ma_doi_tac_nsd + "/" + conn.ma_gdv;
                var connect_tmp = _userConnectionManager.GetUserConnections(key);
                if (connect_tmp!=null && connect_tmp.Count>0)
                    conn.connected = 1;
            }
            res.data_info = connects;
            return Ok(res);
        }
        [Route("chat")]
        [HttpPost]
        public async Task ChatMessage(MessageChat message)
        {
            string key = "HUBCONNECTION:" + message.ma_dtac_nhan + ":" + message.nsd_nhan;
            List<string> dsConnectionUser = _cacheServer.Get<List<string>>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, key, RedisCacheMaster.DatabaseIndex);
            if (dsConnectionUser != null)
            {
                foreach (var connId in dsConnectionUser)
                {
                    await _partnerNotifyHub.Clients.Client(connId).SendAsync("receiveMessage", message);
                }
            }
        }

        /// <summary>
        /// Tạo connect
        /// </summary>
        /// <returns></returns>
        [Route("add-connect")]
        [HttpPost]
        public async Task<IActionResult> AddConnect()
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
            if (action.actionapicode != ESCSStoredProcedures.PHT_THONG_BAO_KET_NOI_NH)
                throw new Exception("Hành động không hợp lệ");
            var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            try
            {
                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; });
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();

                if (config != null && config.Partner == CoreApiConfigContants.OPES)
                {
                    var resNotify = await ApiGatewayOPES.AddConnect(ma_doi_tac_nsd, data_info);
                }
                AddLog(header, "CTYBHABC", "admin@escs.vn", "ADD_CONNECT", JsonConvert.SerializeObject(data_info), JsonConvert.SerializeObject(res));
            }
            catch(Exception ex)
            {
                AddLog(header, "CTYBHABC", "admin@escs.vn", "ERROR_ADD_CONNECT", JsonConvert.SerializeObject(data_info), ex.Message);
            }
            return Ok(res);
            #endregion
        }

        /// <summary>
        /// Ngắt connect
        /// </summary>
        /// <returns></returns>
        [Route("remove-connect")]
        [HttpPost]
        public async Task<IActionResult> RemoveConnect()
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
            if (action.actionapicode != ESCSStoredProcedures.PHT_THONG_BAO_NGAT_KN)
                throw new Exception("Hành động không hợp lệ");

            var outPut = new Dictionary<string, object>();
            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; });
            var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();

            if (config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                var resNotify = await ApiGatewayOPES.RemoveConnect(ma_doi_tac_nsd, data_info);
            }
           
            return Ok(res);
            #endregion
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
