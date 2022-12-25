using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Http;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.API.Hubs;
using ESCS.MODEL.OpenID.ModelView;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using ESCS.COMMON.Request.ApiGateway.MIC;
using ESCS.COMMON.ESCSServices;
using ESCS.MODEL.ESCS;
using ESCS.COMMON.PDFLibrary;

/// <summary>
/// Có kết nối server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    /// <summary>
    /// MailController
    /// </summary>
    [Route("api/common")]
    [ApiController]
    public class CommonController : BaseController
    {
        private readonly IMailService _mailService;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private TemplateServiceConfiguration config;
        public static IRazorEngineService _service = null;
        ILogMongoService<LogContent> _logContentService;
        ILogMongoService<LogSendMail> _logSendMailService;
        /// <summary>
        /// MailController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public CommonController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider,
            IMailService mailService,
            IWebHostEnvironment webHostEnvironment,
            ILogMongoService<LogSendMail> logSendMailService,
            ILogMongoService<LogContent> logContentService)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _mailService = mailService;
            _logContentService = logContentService;
            _webHostEnvironment = webHostEnvironment;
            _logSendMailService = logSendMailService;
            config = new TemplateServiceConfiguration();
            config.CachingProvider = new RazorEngine.Templating.DefaultCachingProvider();
            if (_service == null)
                _service = RazorEngineService.Create(config);
        }
        /// <summary>
        /// Get template email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("get-template-email")]
        [HttpPost]
        public async Task<IActionResult> GetTemplateEmail()
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
                BaseResponse<ThongTinEmail<ThongBaoGiamDinh>> res = new BaseResponse<ThongTinEmail<ThongBaoGiamDinh>>();
                var action = _dynamicService.GetConnection(header);
                if (action == null || action.actionapicode != ESCSStoredProcedures.PHT_MAIL_MAU_GUI_LKE_CT)
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
                var mau_email = await _dynamicService.ExcuteSingleAsync<ht_email_mau_gui>(data_info, header);
                if (mau_email == null || string.IsNullOrEmpty(mau_email.action))
                    throw new Exception("Chưa gán hành động lấy dữ liệu");
                DynamicViewBag dynamicViewBag = new DynamicViewBag();
                var headerClone = header.Clone();
                headerClone.action = mau_email.action;
                var resData = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerClone);
                res.data_info = JsonConvert.DeserializeObject<ThongTinEmail<ThongBaoGiamDinh>>(JsonConvert.SerializeObject(resData));
                dynamicViewBag.AddValue("Data", res);
                res.data_info.key = Utilities.Encrypt(JsonConvert.SerializeObject(res.data_info));

                string path = Path.Combine(AppSettings.PathFolderNotDeleteFull, mau_email.url).ChuanHoaDuongDanRemote();
                if (AppSettings.FolderSharedUsed && path.StartsWith(@"\") && !path.StartsWith(@"\\"))
                    path = @"\" + path;

                if (res.data_info.file != null && res.data_info.file.Count > 0)
                {
                    foreach (var item in res.data_info.file)
                    {
                        try
                        {
                            IDictionary<string, object> data_info_file = new Dictionary<string, object>();
                            data_info_file.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                            data_info_file.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                            data_info_file.AddWithExists("nsd", data_info.GetString("nsd"));
                            data_info_file.AddWithExists("pas", data_info.GetString("pas"));
                            data_info_file.AddWithExists("ma_doi_tac", item.ma_doi_tac);
                            data_info_file.AddWithExists("so_id", item.so_id==null?"0": item.so_id.ToString());
                            data_info_file.AddWithExists("bt", item.bt == null ? "0" : item.bt.ToString());
                            var headerCloneFile = header.Clone();
                            headerCloneFile.action = ESCSStoredProcedures.PHT_BH_FILE_TAI_FILE;
                            var resFile = await _dynamicService.ExcuteDynamicNewAsync(data_info_file, headerCloneFile, outValue => { });
                            if (resFile == null)
                                continue;
                            string jsonRes = JsonConvert.SerializeObject(resFile, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                            bh_file file = JsonConvert.DeserializeObject<bh_file>(jsonRes);
                            string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                            if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                                fileName = @"\" + fileName;
                            file.extension = Path.GetExtension(fileName);
                            item.file_base_64 = Utilities.ConvertFileToBase64String(fileName);
                        }
                        catch
                        { }
                    }
                }

                if (System.IO.File.Exists(path))
                {
                    string name = mau_email.ma_doi_tac + mau_email.ma;
                    string template = "";
                    if (_service.IsTemplateCached(name, null))
                    {
                        try { template = _service.Run(name, null, null, dynamicViewBag); } catch { throw new Exception("Có lỗi xảy ra trong quá trình gán dữ liệu vào mẫu"); }
                    }
                    else
                    {
                        string htmlTemplate = System.IO.File.ReadAllText(path);
                        try { template = _service.RunCompile(htmlTemplate, name, null, null, dynamicViewBag); } catch (Exception ex) { throw ex; }
                    }
                    res.data_info.template = template;
                }
                else
                {
                    throw new Exception("Không tìm thấy đường dẫn file mẫu: " + path);
                }
                return Ok(res);
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }
        [Route("get-config-pdf")]
        public IActionResult GetConfigTemplatePDF()
        {
            return Ok(PDFUtils.GetConfigTemplatePDF());
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
