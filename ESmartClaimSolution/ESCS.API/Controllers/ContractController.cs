using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Http;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.COMMON.SignaturePDF;
using ESCS.API.Hubs;
using ESCS.MODEL.ESCS;
using ESCS.MODEL.OpenID.ModelView;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SelectPdf;
using ClosedXML.Excel;
using System.Data;
using System.Text.RegularExpressions;
using ESCS.COMMON.Hubs;
using System.Threading;
using ESCS.COMMON.OCR;
using System.Drawing;
using ESCS.COMMON.AI;
using System.Text;
using System.Web;
using System.Drawing.Imaging;
using Microsoft.Extensions.Logging;
using ESCS.COMMON.Excels;
using ESCS.API.Attributes;
using ESCS.COMMON.ESCSServices;
using ESCS.COMMON.QRCodeManager;
using ESCS.COMMON.Request.ApiGateway.OPES;
/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    [Route("api/contract")]
    [ApiController]
    [ESCSAuth]
    public class ContractController : BaseController
    {
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private TemplateServiceConfiguration config;
        public static IRazorEngineService _service = null;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogger<FilesController> _logger;

        /// <summary>
        /// MailController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public ContractController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider,
            IMailService mailService,
            IWebHostEnvironment webHostEnvironment,
            ILogMongoService<LogContent> logContent,
            ILogMongoService<LogNhanDienAI> logNhanDienAI,
            IUserConnectionManager userConnectionManager,
            ILogger<FilesController> logger)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _logger = logger;
            _userConnectionManager = userConnectionManager;
            _logContent = logContent;
            _mailService = mailService;
            _webHostEnvironment = webHostEnvironment;
            config = new TemplateServiceConfiguration();
            config.CachingProvider = new RazorEngine.Templating.DefaultCachingProvider();
            if (_service == null)
                _service = RazorEngineService.Create(config);

        }

        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("add-contract")]
        [HttpPost]
        public async Task<IActionResult> AddContract()
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
            if (data_info == null)
                throw new Exception("Không xác định dữ liệu truyền lên");
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");

            if (string.IsNullOrEmpty(ma_doi_tac_nsd))
                throw new Exception("Không xác định mã đối tác");

            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
                if (action.actionapicode != ESCSStoredProcedures.PTICH_HOP_HOP_DONG_XE_OPES)
                    throw new Exception("Hành động không hợp lệ (ActionCode)");
            #endregion
            #region Thực thi type excute db
            var outPut = new Dictionary<string, object>();
            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; });
            return Ok(res);
            #endregion
        }

        /// <summary>
        /// Upload hợp đồng
        /// </summary>
        /// <returns></returns>
        [Route("upload-hdong")]
        [HttpPost]
        public async Task<IActionResult> UploadHdong()
        {
            try
            {
                #region Lấy thông tin header và kiểm tra token
                HeaderRequest header = Request.GetHeader();
                Dictionary<string, bool> prefix = new Dictionary<string, bool>();
                openid_access_token token = null;
                string keyCachePartner = string.Empty;
                var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner, true, false);
                if (!string.IsNullOrEmpty(vetifyTokenMessage))
                {
                    BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                    resError.state_info.status = STATUS_NOTOK;
                    resError.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                    resError.state_info.message_body = vetifyTokenMessage;
                    return Ok(resError);
                }
                header.envcode = token.evncode;
                #endregion
                #region Lấy thông tin action
                var action = _dynamicService.GetConnection(header);
                if (action == null)
                    throw new Exception("Hành động không hợp lệ hoặc không được phân quyền sử dụng.");
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();
                #endregion
                #region Lấy thông tin cấu hình upload
                if (!Utilities.IsMultipartContentType(Request.ContentType))
                    return BadRequest();
                IFormFileCollection files;
                var rqData = Request.GetFormDataRequest(out files);
                if (files == null || files.Count <= 0)
                    throw new Exception("Không xác định được file uploads");
                var extVideo = new string[] { ".jpg", ".png", ".jpeg", ".gif" };
                string extensionItem = Path.GetExtension(files[0].FileName).ToLower();
                if (!extVideo.Contains(extensionItem))
                    throw new Exception("File không hợp lệ (.jpg, .png, .jpeg, .gif)");
                string str = rqData.data;
                Regex[] regexes = new Regex[] { new Regex("^.*files.*$", RegexOptions.IgnoreCase) };
                str = Utilities.RemoveSensitiveProperties(str, regexes);
                var obj = JsonConvert.DeserializeObject<BaseRequest>(str);
                var ma_doi_tac_nsd = obj.data_info.GetValueOrDefault("ma_doi_tac_nsd");
                var so_id_hd = obj.data_info.GetValueOrDefault("so_id_hd");
                var so_id_dt = obj.data_info.GetValueOrDefault("so_id_dt");
                #endregion
                #region Upload file
                string typePath = Path.Combine(ma_doi_tac_nsd, "HOP_DONG", so_id_hd);
                var path = Path.Combine(AppSettings.PathFolderNotDeleteFull, typePath);
                if (AppSettings.FolderSharedUsed && path.StartsWith(@"\") && !path.StartsWith(@"\\"))
                    path = @"\" + path;
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                var name = Guid.NewGuid().ToString("N") + extensionItem;
                var duong_dan = Path.Combine(typePath, name);

                var fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, duong_dan);
                using (var stream = new FileStream(fileName, FileMode.Create))
                    files[0].CopyTo(stream);
                #endregion
                #region Thực thi type excute db
                Dictionary<string, string> data = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(obj.data_info));
                data.AddWithExists("duong_dan", duong_dan);
                BaseRequest rq = new BaseRequest(data);
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.data_info = await _dynamicService.ExcuteDynamicAsync(rq, header);
                #endregion
                return Ok(res);
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }
        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("create-qrcode")]
        [HttpPost]
        public async Task<IActionResult> Create()
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
            var outPut = new Dictionary<string, object>();
            if (action.actionapicode == ESCSStoredProcedures.PBH_HD_XE_GCN_HD_NH || action.actionapicode == ESCSStoredProcedures.PBH_HD_QRCODE_LKE_CT)
            {
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; outPut = outValue; });
                try
                {
                    var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                    var so_id_hd = data_info.GetString("so_id");
                    var nv = data_info.GetString("nv");
                    if (string.IsNullOrEmpty(so_id_hd))
                        so_id_hd = outPut.GetString("so_id");
                    if (string.IsNullOrEmpty(nv))
                        nv = outPut.GetString("nv");
                    var timelive = DateTime.Now.AddMonths(1).ToString("yyyyMMddHHmmss");
                    if (!string.IsNullOrEmpty(ma_doi_tac_nsd) && !string.IsNullOrEmpty(so_id_hd))
                    {
                        var filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE");
                        if (!Directory.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote()))
                            Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());

                        filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE", "HOPDONGXE" + ma_doi_tac_nsd + so_id_hd + ".png");
                        if (!System.IO.File.Exists(Path.Combine(ma_doi_tac_nsd, filePath)))
                        {
                            var apikey = ma_doi_tac_nsd + "|" + nv + "|" + so_id_hd + "|" + "HOP_DONG_XE" + "|" + timelive;
                            var textQRCode = AppSettings.QRCodeLinkDanhGiaRuiRo + "?hash=" + HttpUtility.UrlEncode(Utilities.EncryptByKey(apikey, AppSettings.KeyEryptData));
                            var headerClone = header.Clone();
                            headerClone.action = ESCSStoredProcedures.PBH_HD_QRCODE_NH;
                            QRCodeUtils.GenerateQRCode(textQRCode, Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());
                            var data = new Dictionary<string, object>();
                            data.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                            data.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                            data.AddWithExists("nsd", data_info.GetString("nsd"));
                            data.AddWithExists("pas", data_info.GetString("pas"));
                            data.AddWithExists("so_id", so_id_hd);
                            data.AddWithExists("nv", nv);
                            data.AddWithExists("loai", "HOP_DONG_XE");
                            data.AddWithExists("url_file", filePath);
                            data.AddWithExists("url_link", HttpUtility.UrlEncode(Utilities.EncryptByKey(apikey, AppSettings.KeyEryptData)));
                            await _dynamicService.ExcuteDynamicNewAsync(data, headerClone);
                        }
                    }
                }
                catch { }
                return Ok(res);
            }
            res.state_info.status = STATUS_NOTOK;
            res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
            res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
            return Ok(res);
            #endregion
        }
        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("read-qrcode")]
        [HttpPost]
        public IActionResult ReadQRCodeContract()
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
            var tokenKey = HttpUtility.UrlDecode(data_info.GetString("api_key"));
            if (string.IsNullOrEmpty(tokenKey))
                throw new Exception("Không xác định được thông tin token key");
            var decript = Utilities.DecryptByKey(tokenKey, AppSettings.KeyEryptData);
            if (!string.IsNullOrEmpty(decript))
            {
                var arr = decript.Split("|");
                if (arr == null && arr.Count() != 5)
                    throw new Exception("Không xác định được thông tin mã QRCode");
                var qrcode = new
                {
                    ma_doi_tac = arr[0],
                    nv = arr[1],
                    so_id = arr[2],
                    loai = arr[3],
                    timelive = arr[4]
                };
                long curentDate = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (curentDate > Int64.Parse(qrcode.timelive))
                    throw new Exception("QRCode hết hiệu lực sủ dụng");
                res.state_info.status = STATUS_OK;
                res.data_info = qrcode;
                return Ok(res);
            }
            else
            {
                res.state_info.status = STATUS_NOTOK;
                res.data_info = null;
                return Ok(res);
            }
        }
        /// <summary>
        /// Lấy thông tin QRCode
        /// </summary>
        /// <returns></returns>
        [Route("get-qrcode")]
        [HttpPost]
        public async Task<IActionResult> GetQRCode()
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
            if (action.actionapicode == ESCSStoredProcedures.PBH_HD_QRCODE_LKE_CT)
            {
                bh_hd_qrcode qrcode = await _dynamicService.ExcuteSingleAsync<bh_hd_qrcode>(data_info, header);
                if (qrcode == null || string.IsNullOrEmpty(qrcode.url_file) || !System.IO.File.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, qrcode.url_file).ChuanHoaDuongDanRemote()))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Không xác định được file.";
                    return Ok(res);
                }
                var path = Path.Combine(AppSettings.PathFolderNotDeleteFull, qrcode.url_file).ChuanHoaDuongDanRemote();
                qrcode.file_base64 = Utilities.ConvertFileToBase64String(path);
                qrcode.url_link = AppSettings.QRCodeLinkDanhGiaRuiRo + "?hash=" + qrcode.url_link;
                res.state_info.status = STATUS_OK;
                res.data_info = qrcode;
                return Ok(res);
            }
            res.state_info.status = STATUS_NOTOK;
            res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
            res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
            return Ok(res);
            #endregion
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
    }
}