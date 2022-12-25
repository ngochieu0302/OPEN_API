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

/// <summary>
/// Có kết nối server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    /// <summary>
    /// MailController
    /// </summary>
    [Route("api/p/esmartclaim")]
    [ApiController]
    public class MailController : BaseController
    {
        private readonly IMailService _mailService;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private TemplateServiceConfiguration config;
        public static IRazorEngineService _serviceRazor = null;
        ILogMongoService<LogContent> _logContentService;
        ILogMongoService<LogSendMail> _logSendMailService;
        /// <summary>
        /// MailController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public MailController(
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
            if (_serviceRazor == null)
                _serviceRazor = RazorEngineService.Create(config);
        }
        /// <summary>
        /// SendMail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("send-mail")]
        [HttpPost]
        public async Task<IActionResult> SendMail()
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
                if (action.action_type == "SENDMAIL")
                {
                    #region Thực thi lấy dữ liệu
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    #endregion
                    #region Compile template 
                    var server = res.data_info["server"];
                    var mau_email = res.data_info["mau_email"];
                    string duong_dan = mau_email.url.ToString();
                    string key_compile_template = mau_email.ma_doi_tac.ToString() + mau_email.ma.ToString();

                    var networkCredential = new NetworkCredentialItem()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        IpRemote = action.ip_remote,
                        BaseFolderName = action.base_folder,
                        UserName = action.user_remote,
                        Password = action.pas_remote,
                        FullPath = action.is_local == null || action.is_local <= 0 ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                        IsLocal = action.is_local == null || action.is_local <= 0 ? false : true
                    };
                    List<LinkedResource> linkSource = new List<LinkedResource>();
                    if (mau_email.url_banner != null && mau_email.url_banner.ToString() != "")
                    {
                        string pathFile = Path.Combine(networkCredential.FullPath, mau_email.url_banner.ToString());
                        if (!networkCredential.IsLocal)
                        {
                            NetworkCredential credentials = new NetworkCredential(Utilities.Decrypt(networkCredential.UserName), Utilities.Decrypt(networkCredential.Password));
                            using (new ConnectToSharedFolder(networkCredential.FullPath, credentials))
                            {
                                if (System.IO.File.Exists(pathFile))
                                {
                                    Byte[] bitmapData = Convert.FromBase64String(Utilities.ConvertFileToBase64String(pathFile));
                                    System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
                                    LinkedResource linkSourceBannber = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg);
                                    linkSourceBannber.ContentId = "MailBanner";
                                    linkSource.Add(linkSourceBannber);
                                }
                            }
                        }
                        else
                        {
                            if (System.IO.File.Exists(pathFile))
                            {
                                Byte[] bitmapData = Convert.FromBase64String(Utilities.ConvertFileToBase64String(pathFile));
                                System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
                                LinkedResource linkSourceBannber = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg);
                                linkSourceBannber.ContentId = "MailBanner";
                                linkSource.Add(linkSourceBannber);
                            }
                        }
                    }

                    DynamicViewBag dynamicViewBag = new DynamicViewBag();
                    dynamicViewBag.AddValue("Data", res);
                    if (!networkCredential.IsLocal)
                    {
                        dynamicViewBag.AddValue("BaseUrl", "file:" + networkCredential.FullPath);
                    }
                    else
                    {
                        dynamicViewBag.AddValue("BaseUrl", networkCredential.FullPath);
                    }

                    string template = "";
                    if (_serviceRazor.IsTemplateCached(key_compile_template, null))
                    {
                        template = _serviceRazor.Run(key_compile_template, null, null, dynamicViewBag);
                    }
                    else
                    {
                        string pathFile = Path.Combine(networkCredential.FullPath, duong_dan);
                        string htmlTemplate = string.Empty;
                        if (!networkCredential.IsLocal)
                        {
                            NetworkCredential credentials = new NetworkCredential(Utilities.Decrypt(networkCredential.UserName), Utilities.Decrypt(networkCredential.Password));
                            using (new ConnectToSharedFolder(networkCredential.FullPath, credentials))//Nếu có người đang mở thư mục này trên máy khác ko phải máy chủ, việc kết nối sẽ không thành công
                            {
                                if (!System.IO.File.Exists(pathFile))
                                {
                                    BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                                    resError.state_info.status = STATUS_NOTOK;
                                    resError.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                                    resError.state_info.message_body = "Không tồn tại file";
                                    return Ok(resError);
                                }
                                htmlTemplate = System.IO.File.ReadAllText(pathFile);
                            }
                        }
                        else
                        {
                            if (!System.IO.File.Exists(pathFile))
                            {
                                BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                                resError.state_info.status = STATUS_NOTOK;
                                resError.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                                resError.state_info.message_body = "Không tồn tại file";
                                return Ok(resError);
                            }
                            htmlTemplate = System.IO.File.ReadAllText(pathFile);
                        }
                        template = _serviceRazor.RunCompile(htmlTemplate, key_compile_template, null, null, dynamicViewBag);
                    }
                    #endregion
                    #region Attach file đính kèm nếu có
                    ArrayList arrFiles = new ArrayList();
                    #endregion
                    #region Thực thi gửi email
                    EmailHandler emailHandler = new EmailHandler();
                    EmailConfig config = new EmailConfig();
                    config.smtp_server = server.smtp_server.ToString();
                    config.smtp_port = Convert.ToInt32(server.smtp_port.ToString());
                    config.send_from = server.smtp_tai_khoan.ToString();
                    config.pass_mail = server.smtp_mat_khau.ToString();
                    config.display_name = server.ten_hthi.ToString();
                    var bcc = "";
                    if (res.data_info.ContainsKey("mail_bcc"))
                    {
                        var ds_bcc = res.data_info["mail_bcc"];
                        if (ds_bcc != null)
                        {
                            int index = 0;
                            foreach (var item in ds_bcc)
                            {
                                if (index == 0)
                                    bcc = item.mail_bcc;
                                else
                                    bcc += "," + item.mail_bcc;
                                index++;
                            }
                            config.bcc_email = bcc;
                        }
                    }
                    string nguoi_nhan = "";
                    var ds_nguoi_nhan = res.data_info["mail_nhan"];
                    if (ds_nguoi_nhan != null)
                    {
                        int index = 0;
                        foreach (var item in ds_nguoi_nhan)
                        {
                            if (index == 0)
                                nguoi_nhan = item.mail_nhan;
                            else
                                nguoi_nhan += "," + item.mail_nhan;
                            index++;
                        }
                    }
                    Task task = new Task(() =>
                    {

                        try
                        {
                            if (string.IsNullOrEmpty(nguoi_nhan))
                                return;
                            var service = Engine.Razor;
                            var outValue = res.out_value as Dictionary<string, object>;
                            var send = emailHandler.SendMessageWithAttachment(config, nguoi_nhan, outValue["tieu_de"].ToString(), template, arrFiles, linkSource);
                        }
                        catch (Exception ex)
                        {
                            _logRequestService.AddLogAsync(new LogException("SENDMAIL", ex.Message));
                        }
                    });
                    task.Start();
                    #endregion
                    BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
                    resSuccess.state_info.status = STATUS_OK;
                    resSuccess.state_info.message_code = "SUCCESS";
                    resSuccess.state_info.message_body = "Gửi email thành công";
                    return Ok(resSuccess);

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
        /// <summary>
        /// ForwardMail
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [Route("forward-mail")]
        [HttpPost]
        public IActionResult ForwardMail(RequestModel<MailOpenIdConfig> rq)
        {
            Task task = new Task(() =>
            {

                try
                {
                    var mailConfig = rq.data_info;
                    
                    EmailHandler emailHandler = new EmailHandler();
                    EmailConfig config = new EmailConfig();
                    config.smtp_server = mailConfig.server.smtp_server;
                    config.smtp_port = mailConfig.server.smtp_port;
                    config.send_from = mailConfig.from.username;
                    config.pass_mail = mailConfig.from.password;
                    config.display_name = mailConfig.from.alias;
                    config.to = mailConfig.to;
                    config.cc = mailConfig.cc;
                    config.bcc = mailConfig.bcc;
                    List<System.Net.Mail.Attachment> attachment = new List<Attachment>();
                    if (mailConfig.attach != null && mailConfig.attach.Count > 0)
                    {
                        foreach (var file in mailConfig.attach)
                        {
                            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
                            contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
                            var bytes = Convert.FromBase64String(file.base64);
                            Stream stream = new MemoryStream(bytes);
                            var temp = new System.Net.Mail.Attachment(stream, contentType);
                            temp.Name = file.file_name;
                            attachment.Add(temp);
                        }
                    }
                    if (mailConfig.to == null || mailConfig.to.Count <= 0)
                        return;
                    var service = Engine.Razor;
                    var send = emailHandler.SendMessageWithAttachmentConfig(config, mailConfig.title, mailConfig.body, attachment);
                    _logSendMailService.AddLogAsync(new LogSendMail(mailConfig.title, mailConfig.body, config, send));
                }
                catch (Exception ex)
                {
                    _logRequestService.AddLogAsync(new LogException("SENDMAIL", ex.Message));
                }
            });
            task.Start();
            BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
            resSuccess.state_info.status = STATUS_OK;
            resSuccess.state_info.message_code = "SUCCESS";
            resSuccess.state_info.message_body = "Gửi email thành công";
            return Ok(resSuccess);
        }

        [Route("mic-send-mail")]
        [HttpPost]
        public async Task<IActionResult> MICSendMail()
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
            #region Thực thi gửi email
            var pas_tmp = data_info.GetString("pas");
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var dataRequest = ApiGateway.RequestApiSendEmail(data_info);
            string json = await ApiGateway.CallApiSendEmail(ma_doi_tac_nsd, dataRequest);
            AddLog(header, ma_doi_tac_nsd, nsd, "MIC_SEND_EMAIL", JsonConvert.SerializeObject(dataRequest), json);
            return Ok(JObject.Parse(json));
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
