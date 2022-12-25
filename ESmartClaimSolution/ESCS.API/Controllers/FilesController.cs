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
using ESCS.COMMON.PDFLibrary;
/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    [Route("api/esmartclaim")]
    [ApiController]
    [ESCSAuth]
    public class FilesController : BaseController
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
        public FilesController(
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
        /// Gen file PDF
        /// </summary>
        /// <returns></returns>
        [Route("gen-pdf")]
        [HttpPost]
        public async Task<IActionResult> GeneratePDF()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                if (action.action_type == "FILE")
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    string name = data_info["ma_doi_tac_nsd"].ToString() + data_info["ma_mau_in"].ToString();
                    string duong_dan = data_info["url_file"].ToString();
                    if (AppSettings.Environment == "Development")
                    {
                        action.is_local = 1;
                        action.ip_remote = AppSettings.PathFolderNotDelete;
                    }
                    var networkCredential = new NetworkCredentialItem()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        IpRemote = action.ip_remote,
                        BaseFolderName = action.base_folder,
                        UserName = action.user_remote,
                        Password = action.pas_remote,
                        FullPath = (action.is_local == null || action.is_local <= 0) ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                        IsLocal = (action.is_local == null || action.is_local <= 0) ? false : true
                    };
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
                    if (_service.IsTemplateCached(name, null))
                    {
                        try { template = _service.Run(name, null, null, dynamicViewBag); } catch (Exception ex) { throw new Exception("Lỗi cú pháp khi đổ dữ liệu: " + ex.Message); }
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
                        try { template = _service.RunCompile(htmlTemplate, name, null, null, dynamicViewBag); } catch (Exception ex) { throw new Exception("Lỗi cú pháp khi đổ dữ liệu: " + ex.Message); }
                    }
                    string fileName = "file_" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".pdf";
                    byte[] arrByte = null;
                    #region SelectPdf
                    HtmlToPdf converter = new HtmlToPdf();
                    #region Thêm mới
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.MarginLeft = 30;
                    converter.Options.MarginRight = 30;
                    converter.Options.MarginTop = 40;
                    converter.Options.MarginBottom = 30;
                    #endregion
                    //converter.Options.DisplayFooter = true;
                    //converter.Footer.DisplayOnFirstPage = true;
                    //converter.Footer.DisplayOnOddPages = true;
                    //converter.Footer.DisplayOnEvenPages = true;
                    //converter.Footer.Height = 20;
                    //PdfTextSection page_num = new PdfTextSection(5, 5, "Trang: {page_number}/{total_pages}  ", new System.Drawing.Font("Arial", 8));
                    //page_num.HorizontalAlign = PdfTextHorizontalAlign.Right;
                    //converter.Footer.Add(page_num);
                    SelectPdf.PdfDocument doc = converter.ConvertHtmlString(template);
                    arrByte = doc.Save();
                    #endregion
                    #region Syncfusion
                    //HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
                    //WebKitConverterSettings settings = new WebKitConverterSettings();
                    //string baseUrl = _webHostEnvironment.ContentRootPath;
                    //settings.WebKitPath = @"\QtBinariesDotNetCore\";
                    ////Set WebKit path
                    ////settings.WebKitPath = Path.Combine(_webHostEnvironment.ContentRootPath, "QtBinariesWindows");
                    //settings.Margin = new Syncfusion.Pdf.Graphics.PdfMargins() { Top = 40, Bottom = 30, Left = 30, Right = 30 };
                    ////Assign WebKit settings to HTML converter
                    //htmlConverter.ConverterSettings = settings;
                    ////Convert URL to PDF
                    //Syncfusion.Pdf.PdfDocument document = htmlConverter.Convert(template, baseUrl);
                    ////Saving the PDF to the MemoryStream
                    //MemoryStream streamOut = new MemoryStream();
                    //document.Save(streamOut);
                    //arrByte = streamOut.ToArray();
                    #endregion
                    #region itext7
                    //var workStream = new MemoryStream();
                    //using (var pdfWriter = new PdfWriter(workStream))
                    //{
                    //    pdfWriter.SetCloseStream(false);
                    //    using (var document = HtmlConverter.ConvertToDocument(template, pdfWriter))
                    //    {
                    //        document.SetMargins(40, 30, 30, 30);
                    //        arrByte = workStream.ToArray();
                    //        workStream.Write(arrByte, 0, arrByte.Length);
                    //        workStream.Position = 0;
                    //    }
                    //}
                    #endregion
                    return File(arrByte, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
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
        /// Upload file lên server
        /// PHT_BH_FILE_LUU - Web   
        /// </summary>
        /// <returns></returns>
        [Route("upload-file")]
        [HttpPost]
        public async Task<IActionResult> UploadFile()
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
                {
                    BaseResponse<dynamic> resErr001 = new BaseResponse<dynamic>();
                    resErr001.state_info.status = STATUS_NOTOK;
                    resErr001.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    resErr001.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                    return Ok(resErr001);
                }
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();
                if (action.action_type != "FILE")
                    throw new Exception("Không xác định loại Action Type");
                #endregion
                #region Lấy thông tin cấu hình upload
                if (!Utilities.IsMultipartContentType(Request.ContentType))
                    return BadRequest();
                IFormFileCollection files;
                var rqData = Request.GetFormDataRequest(out files);
                string str = rqData.data;
                List<FileInfoData> listFileData = new List<FileInfoData>();
                var jobject = JObject.Parse(str);
                if (jobject["data_info"]["files"] != null)
                {
                    listFileData = jobject["data_info"]["files"].ToObject<List<FileInfoData>>();
                }
                if (listFileData == null)
                    listFileData = new List<FileInfoData>();
                Regex[] regexes = new Regex[]
                {
                        new Regex("^.*files.*$", RegexOptions.IgnoreCase)
                };
                str = Utilities.RemoveSensitiveProperties(str, regexes);
                var obj = JsonConvert.DeserializeObject<BaseRequest>(str);
                List<StatusUploadFile> status = new List<StatusUploadFile>();

                OptionSaveFile config = new OptionSaveFile()
                {
                    type = action.type_file,
                    ma_doi_tac = obj.data_info["ma_doi_tac_nsd"],
                    so_id = obj.data_info["so_id"],
                    max_length = action.max_content_length == null ? (int?)null : (int)action.max_content_length,
                    max_width = action.max_width == null ? (int?)null : (int)action.max_width,
                    is_duplicate_mini_file = action.is_duplicate_mini == null || action.is_duplicate_mini <= 0 ? false : true,
                    config_mini_file = new ConfigDuplicateMiniFile
                    {
                        width = action.max_width_file_mini == null ? (int?)null : (int)action.max_width_file_mini,
                        max_length = action.max_content_mini == null ? (int?)null : (int)action.max_content_mini,
                        prefix = action.prefix_mini
                    }
                };
                #endregion
                #region Upload file
                if (files != null && files.Count > 0)
                {
                    string[] ext = action.extensions_file.Replace(" ", "").Split(',');
                    status = Utilities.UploadFiles(files, AppSettings.PathFolderNotDeleteFull, action.type_file, config, listFileData, ext);
                }
                #endregion
                #region Thực thi type excute db
                var lstFile = new List<bh_file>();
                Dictionary<string, string> myUnderlyingObject = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(obj.data_info));
                int index = 0;
                var status_success = status.Where(n => n.status_upload == StatusUploadFileConstant.SUCCESS);
                foreach (var itemFile in status_success)
                {
                    itemFile.stt = itemFile.stt == null ? 0 : itemFile.stt;
                    itemFile.stt_hang_muc = itemFile.stt_hang_muc == null ? 0 : itemFile.stt_hang_muc;

                    myUnderlyingObject.Add("files[" + index + "][ma_file]", itemFile.nhom_anh);
                    myUnderlyingObject.Add("files[" + index + "][ten_file]", itemFile.file_name_new);
                    myUnderlyingObject.Add("files[" + index + "][duong_dan]", itemFile.path_file);
                    myUnderlyingObject.Add("files[" + index + "][trang_thai]", "1");
                    myUnderlyingObject.Add("files[" + index + "][x]", itemFile.x.ToString());
                    myUnderlyingObject.Add("files[" + index + "][y]", itemFile.y.ToString());
                    myUnderlyingObject.Add("files[" + index + "][gid]", itemFile.gid);
                    myUnderlyingObject.Add("files[" + index + "][stt]", itemFile.stt.ToString());

                    myUnderlyingObject.Add("files[" + index + "][loai]", itemFile.loai);
                    myUnderlyingObject.Add("files[" + index + "][lh_nv]", itemFile.lh_nv);
                    myUnderlyingObject.Add("files[" + index + "][hang_muc]", itemFile.hang_muc);
                    myUnderlyingObject.Add("files[" + index + "][muc_do]", itemFile.muc_do);
                    myUnderlyingObject.Add("files[" + index + "][thay_the_sc]", itemFile.thay_the_sc);
                    myUnderlyingObject.Add("files[" + index + "][chinh_hang]", itemFile.chinh_hang);
                    myUnderlyingObject.Add("files[" + index + "][thu_hoi]", itemFile.thu_hoi);
                    myUnderlyingObject.Add("files[" + index + "][vu_tt]", itemFile.vu_tt == null ? "0" : itemFile.vu_tt.ToString());
                    myUnderlyingObject.Add("files[" + index + "][tien_tu_dong]", itemFile.tien_tu_dong == null ? "0" : itemFile.vu_tt.ToString());
                    myUnderlyingObject.Add("files[" + index + "][tien_gd]", itemFile.tien_gd == null ? "0" : itemFile.tien_gd.ToString());
                    myUnderlyingObject.Add("files[" + index + "][ghi_chu]", itemFile.ghi_chu);
                    myUnderlyingObject.Add("files[" + index + "][stt_hang_muc]", itemFile.stt_hang_muc.ToString());
                    index++;
                }
                BaseRequest rq = new BaseRequest(myUnderlyingObject);
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                prefix.Add("files", true);
                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicAsync(rq, header, prefix, outValue =>
                {
                    res.out_value = outValue;
                });
                #endregion
                #region Nhận diện ảnh tổn thất Computervision
                if (OCRApiConfig.Enable == true)
                {
                    Task threadNhanDien = new Task(async () =>
                    {
                        HeaderRequest headerAI = header.Clone();
                        var ma_doi_tac_nsd = obj.data_info["ma_doi_tac_nsd"];
                        BaseResponse<dynamic> baseResponse = new BaseResponse<dynamic>();
                        var paramRequest = new Dictionary<string, object>();
                        paramRequest.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd.ToString());
                        paramRequest.AddWithExists("ma_chi_nhanh_nsd", myUnderlyingObject.GetString("ma_chi_nhanh_nsd"));
                        paramRequest.AddWithExists("nsd", myUnderlyingObject.GetString("nsd"));
                        paramRequest.AddWithExists("pas", myUnderlyingObject.GetString("pas"));
                        paramRequest.AddWithExists("ma_doi_tac", ma_doi_tac_nsd.ToString());
                        var jsonHangMuc = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, "HANG_MUC_AI", RedisCacheMaster.DatabaseIndex);
                        if (string.IsNullOrEmpty(jsonHangMuc))
                        {
                            headerAI.action = ESCSStoredProcedures.PBH_HT_MA_XE_HANG_MUC_AI;
                            baseResponse.data_info = await _dynamicService.ExcuteDynamicNewAsync(paramRequest, headerAI);
                            if (baseResponse.data_info != null)
                            {
                                jsonHangMuc = JsonConvert.SerializeObject(baseResponse.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                                if (!string.IsNullOrEmpty(jsonHangMuc))
                                {
                                    List<ht_ma_xe_hang_muc> arrHangMucAI = JsonConvert.DeserializeObject<List<ht_ma_xe_hang_muc>>(jsonHangMuc);
                                    _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, "HANG_MUC_AI", arrHangMucAI, DateTime.Now.AddYears(1) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                                }
                            }
                        }
                        foreach (var item in status_success)
                        {
                            headerAI.action = ESCSStoredProcedures.PBH_FILE_AI_NH;
                            Dictionary<string, string> paramSave = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(obj.data_info));
                            paramSave.AddWithExists("ma_file", item.nhom_anh);
                            paramSave.AddWithExists("gid", item.gid);
                            try
                            {
                                List<ht_ma_xe_hang_muc> arrHangMuc = JsonConvert.DeserializeObject<List<ht_ma_xe_hang_muc>>(jsonHangMuc);
                                var arr = arrHangMuc.Where(n => n.ma == item.nhom_anh);
                                if (arr.Count() > 0)
                                {
                                    item.path_file = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.path_file).ChuanHoaDuongDanRemote();
                                    var result = await OCRService.AINhanDienAnhTonThat<cvs_response>(OCRApiConfig.BaseUrl, OCRApiConfig.ApiKey, OCRApiConfig.SecretKey, Utilities.ConvertFileToByteArray(item.path_file));
                                    var base64 = result.image;
                                    result.image = string.IsNullOrEmpty(base64) ? "Không có dữ liệu base64" : "Có dữ liệu base64";
                                    paramSave.AddWithExists("ai_kq", JsonConvert.SerializeObject(result));
                                    paramSave.AddWithExists("duong_dan", "");
                                    paramSave = paramSave.Where(kv => !kv.Key.Contains("damage[")).ToDictionary(kv => kv.Key, kv => kv.Value);
                                    if (result.errorCode == AI_CONSTANT.ERRORCODE && result.errorMessage.ToUpper() == AI_CONSTANT.ERRORMESSAGE && result.data != null && result.data.Count > 0)
                                    {
                                        var now = DateTime.Now;
                                        string fileNameOutPut = now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + ".jpg";
                                        var month = now.Month.ToString();
                                        var day = now.Day.ToString();
                                        if (month.Length < 2)
                                            month = "0" + month;
                                        if (day.Length < 2)
                                            day = "0" + day;
                                        string pathFileOutput = Path.Combine(ma_doi_tac_nsd, "TAI_LIEU", now.Year.ToString(), month, day);
                                        if (!Directory.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput).ChuanHoaDuongDanRemote()))
                                            Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput).ChuanHoaDuongDanRemote());
                                        string output = Path.Combine(AppSettings.PathFolderNotDeleteFull, pathFileOutput, fileNameOutPut).ChuanHoaDuongDanRemote();
                                        byte[] imageBytes = Convert.FromBase64String(base64);
                                        System.IO.File.WriteAllBytes(output, imageBytes);
                                        paramSave["duong_dan"] = Path.Combine(pathFileOutput, fileNameOutPut);
                                        var index_damage = 0;
                                        foreach (var damage in result.data)
                                        {
                                            paramSave.Add("damage[" + index_damage + "][type]", damage.damage_type);
                                            paramSave.Add("damage[" + index_damage + "][parts]", "");
                                            paramSave.Add("damage[" + index_damage + "][box]", string.Join(",", damage.damage_box));
                                            paramSave.Add("damage[" + index_damage + "][score]", damage.damage_score);
                                            index_damage++;
                                        }
                                    }
                                    var data_dic = paramSave.ToDictionary(n => n.Key, n => n.Value == null ? "" : n.Value.ToString());
                                    BaseRequest rq = new BaseRequest(data_dic);
                                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                                    Dictionary<string, bool> prefix_tmp = new Dictionary<string, bool>();
                                    prefix_tmp.Add("damage", true);
                                    await _dynamicService.ExcuteDynamicAsync(rq, headerAI, prefix_tmp);
                                }
                            }
                            catch { }
                        }
                    });
                    threadNhanDien.Start();
                };
                #endregion
                #region Kết quả trả về
                if (status != null && status.Count > 0)
                {
                    foreach (var item in status)
                        item.file = null;
                }
                BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
                resSuccess.state_info.status = STATUS_OK;
                resSuccess.state_info.message_code = STATUS_OK;
                resSuccess.data_info = status;
                resSuccess.state_info.message_body = "Upload thành công";
                resSuccess.out_value = res.out_value;
                return Ok(resSuccess);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }
        /// <summary>
        /// Upload file lên server
        /// PHT_BH_FILE_VIDEO_NH - Web   
        /// </summary>
        /// <returns></returns>

        [Route("upload-video")]
        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> UploadVideo()
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
                {
                    BaseResponse<dynamic> resErr001 = new BaseResponse<dynamic>();
                    resErr001.state_info.status = STATUS_NOTOK;
                    resErr001.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    resErr001.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                    return Ok(resErr001);
                }
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
                var extVideo = new string[] { ".mp4", ".mov" };
                string extensionItem = Path.GetExtension(files[0].FileName).ToLower();
                if (!extVideo.Contains(extensionItem))
                    throw new Exception("File không hợp lệ (.mp4, .mov)");

                string str = rqData.data;
                Regex[] regexes = new Regex[] { new Regex("^.*files.*$", RegexOptions.IgnoreCase) };
                str = Utilities.RemoveSensitiveProperties(str, regexes);
                var obj = JsonConvert.DeserializeObject<BaseRequest>(str);
                var ma_doi_tac_nsd = obj.data_info.GetValueOrDefault("ma_doi_tac_nsd");
                if (string.IsNullOrEmpty(ma_doi_tac_nsd))
                {
                    ma_doi_tac_nsd = obj.data_info["ma_doi_tac_nsd"];
                }
                var ten = obj.data_info.GetValueOrDefault("ten");
                if (string.IsNullOrEmpty(ten))
                    ten = "Video chưa xác định tên";
                #endregion
                #region Upload file
                var dateNow = DateTime.Now;
                var year = dateNow.Year.ToString();
                var month = dateNow.Month.ToString();
                var day = dateNow.Day.ToString();
                if (month.Length < 2)
                    month = "0" + month;
                if (day.Length < 2)
                    day = "0" + day;
                string typePath = Path.Combine(ma_doi_tac_nsd, "VIDEO", year, month, day);
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
                data.AddWithExists("ten", ten);
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
        /// Upload file lên server
        /// PHT_BH_FILE_LUU - Web   
        /// </summary>
        /// <returns></returns>
        [Route("attach-file-email")]
        [HttpPost]
        public async Task<IActionResult> AttachFileEmail()
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
                {
                    BaseResponse<dynamic> resErr001 = new BaseResponse<dynamic>();
                    resErr001.state_info.status = STATUS_NOTOK;
                    resErr001.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    resErr001.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                    return Ok(resErr001);
                }
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();
                if (action.action_type != "FILE")
                    throw new Exception("Không xác định loại Action Type");
                #endregion
                #region Lấy thông tin cấu hình upload
                if (!Utilities.IsMultipartContentType(Request.ContentType))
                {
                    return BadRequest();
                }
                IFormFileCollection files;
                var rqData = Request.GetFormDataRequest(out files);
                string str = rqData.data;
                List<FileInfoData> listFileData = new List<FileInfoData>();
                var jobject = JObject.Parse(str);
                if (jobject["data_info"]["files"] != null)
                {
                    listFileData = jobject["data_info"]["files"].ToObject<List<FileInfoData>>();
                }
                if (listFileData == null)
                    listFileData = new List<FileInfoData>();

                Regex[] regexes = new Regex[] { new Regex("^.*files.*$", RegexOptions.IgnoreCase) };
                str = Utilities.RemoveSensitiveProperties(str, regexes);
                var obj = JsonConvert.DeserializeObject<BaseRequest>(str);
                List<StatusUploadFile> status = new List<StatusUploadFile>();
                OptionSaveFile config = new OptionSaveFile()
                {
                    type = action.type_file,
                    ma_doi_tac = obj.data_info["ma_doi_tac_nsd"],
                    so_id = obj.data_info["so_id"],
                    max_length = action.max_content_length == null ? (int?)null : (int)action.max_content_length,
                    max_width = action.max_width == null ? (int?)null : (int)action.max_width,
                    is_duplicate_mini_file = action.is_duplicate_mini == null || action.is_duplicate_mini <= 0 ? false : true,
                    config_mini_file = new ConfigDuplicateMiniFile
                    {
                        width = action.max_width_file_mini == null ? (int?)null : (int)action.max_width_file_mini,
                        max_length = action.max_content_mini == null ? (int?)null : (int)action.max_content_mini,
                        prefix = action.prefix_mini
                    }
                };
                #endregion
                #region Upload file
                if (files != null && files.Count > 0)
                {
                    string[] ext = action.extensions_file.Replace(" ", "").Split(',');
                    status = Utilities.UploadFiles(files, AppSettings.PathFolderNotDeleteFull, "FILE_DINH_KEM", config, listFileData, ext);
                }
                #endregion
                var lstFile = new List<bh_file>();
                Dictionary<string, string> myUnderlyingObject = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(obj.data_info));
                int index = 0;
                var status_success = status.Where(n => n.status_upload == StatusUploadFileConstant.SUCCESS);
                foreach (var itemFile in status_success)
                {
                    myUnderlyingObject.Add("files[" + index + "][url_file]", itemFile.path_file);
                    myUnderlyingObject.Add("files[" + index + "][ten_file]", itemFile.file_name);
                    index++;
                }
                BaseRequest rq = new BaseRequest(myUnderlyingObject);
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                prefix.Add("files", true);
                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicAsync(rq, header, prefix, outValue =>
                {
                    res.out_value = outValue;
                });

                if (status != null && status.Count > 0)
                {
                    foreach (var item in status)
                        item.file = null;
                }

                BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
                resSuccess.state_info.status = status.Where(n => n.status_upload != StatusUploadFileConstant.SUCCESS).Count() > 0 ? STATUS_NOTOK : STATUS_OK;
                resSuccess.state_info.message_code = "";
                resSuccess.data_info = status;
                resSuccess.state_info.message_body = "";
                resSuccess.out_value = res.out_value;

                return Ok(resSuccess);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// /// <summary>
        /// Đọc OCR giấy tờ xe
        /// </summary>
        /// <returns></returns>
        [Route("ocr-image")]
        [HttpPost]
        public async Task<IActionResult> OCRImage()
        {
            try
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
                if (action == null || action.actionapicode != ESCSStoredProcedures.PHT_BH_FILE_DOC_OCR)
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                    return Ok(res);
                }
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();
                var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                var ma_chi_nhanh_nsd = data_info.GetString("ma_chi_nhanh_nsd");
                var nsd = data_info.GetString("nsd");
                var pas = data_info.GetString("pas");
                var so_id = data_info.GetString("so_id");
                var ma_gara = data_info.GetString("gara");
                if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(so_id))
                    throw new Exception("Không xác định được thông tin hồ sơ");
                #endregion
                #region Kiểm tra cache danh mục bộ mã map hạng mục OCR, nếu chưa có thì lấy cache ra
                HeaderRequest requestDB = header.Clone();
                BaseResponse<dynamic> baseResponse = new BaseResponse<dynamic>();
                var paramRequest = new Dictionary<string, object>();
                paramRequest.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd.ToString());
                var jsonHangMucOCR = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, "DICH_VU_OCR", RedisCacheMaster.DatabaseIndex);
                if (string.IsNullOrEmpty(jsonHangMucOCR))
                {
                    requestDB.action = ESCSStoredProcedures.PHT_DICH_VU_OCR_CACHE;
                    baseResponse.data_info = await _dynamicService.ExcuteDynamicNewAsync(paramRequest, requestDB);
                    if (baseResponse.data_info != null)
                    {
                        jsonHangMucOCR = JsonConvert.SerializeObject(baseResponse.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                        if (!string.IsNullOrEmpty(jsonHangMucOCR))
                        {
                            List<ht_dich_vu_ocr> arrHangMucOCR = JsonConvert.DeserializeObject<List<ht_dich_vu_ocr>>(jsonHangMucOCR);
                            _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, "DICH_VU_OCR", arrHangMucOCR, DateTime.Now.AddYears(1) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                        }
                    }
                }
                #endregion
                #region Đọc OCR
                HeaderRequest headerOCR = header.Clone();
                var dsFiles = await _dynamicService.ExcuteListAsync<ht_lich_doc_ocr>(data_info, header);
                if (dsFiles != null && dsFiles.Count() > 0)
                {
                    foreach (var item in dsFiles)
                    {
                        string response = string.Empty;
                        string pathFileOCR = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan_ocr).ChuanHoaDuongDanRemote();
                        if (pathFileOCR.Contains("error"))
                            continue;
                        if (System.IO.File.Exists(pathFileOCR))
                        {
                            try
                            {
                                response = System.IO.File.ReadAllText(pathFileOCR);
                                if (item.hang_muc == OCRConstant.DANG_KIEM_XE)
                                {
                                    try
                                    {
                                        var resultDangKiem = JsonConvert.DeserializeObject<ocr_dang_kiem_xe>(response);
                                        if (resultDangKiem != null && resultDangKiem.errorCode != null && resultDangKiem.errorCode == 0)
                                        {
                                            headerOCR.action = ESCSStoredProcedures.PHT_OCR_DANG_KIEM_OTO_NH;
                                            var resultOCR = resultDangKiem.GetData();
                                            if (resultOCR != null)
                                            {
                                                resultOCR.ma_doi_tac_nsd = ma_doi_tac_nsd;
                                                resultOCR.ma_chi_nhanh_nsd = ma_chi_nhanh_nsd;
                                                resultOCR.nsd = nsd;
                                                resultOCR.pas = pas;
                                                resultOCR.so_id = item.so_id;
                                                resultOCR.so_id_doi_tuong = item.so_id_doi_tuong;
                                                resultOCR.ung_dung = "WEB";
                                                IDictionary<string, object> data = new Dictionary<string, object>();
                                                data = resultOCR.ModelToDictionary();
                                                await _dynamicService.ExcuteDynamicNewAsync(data, headerOCR, outPutSend => { });
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Có lỗi trong quá trình lưu thông tin OCR đăng kiểm xe:" + ex.Message);
                                    }
                                }
                                if (item.hang_muc == OCRConstant.DANG_KY_XE)
                                {
                                    try
                                    {
                                        var resultDangKy = JsonConvert.DeserializeObject<ocr_dang_ky_xe>(response);
                                        if (resultDangKy != null && resultDangKy.errorCode != null && resultDangKy.errorCode == 0)
                                        {
                                            headerOCR.action = ESCSStoredProcedures.PHT_OCR_DANG_KY_OTO_NH;
                                            var resultOCR = resultDangKy.GetData();
                                            if (resultOCR != null && resultOCR.loai_vb == "mat_sau_dang_ky_xe")
                                            {
                                                resultOCR.ma_doi_tac_nsd = ma_doi_tac_nsd;
                                                resultOCR.ma_chi_nhanh_nsd = ma_chi_nhanh_nsd;
                                                resultOCR.nsd = nsd;
                                                resultOCR.pas = pas;
                                                resultOCR.so_id = item.so_id;
                                                resultOCR.so_id_doi_tuong = item.so_id_doi_tuong;
                                                resultOCR.ung_dung = "WEB";
                                                IDictionary<string, object> data = new Dictionary<string, object>();
                                                data = resultOCR.ModelToDictionary();
                                                await _dynamicService.ExcuteDynamicNewAsync(data, headerOCR, outPutSend => { });
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Có lỗi trong quá trình lưu thông tin OCR đăng ký xe:" + ex.Message);
                                    }
                                }
                                if (item.hang_muc == OCRConstant.BANG_LAI_XE)
                                {
                                    try
                                    {
                                        var resultBangLai = JsonConvert.DeserializeObject<ocr_bang_lai_xe>(response);
                                        if (resultBangLai != null && resultBangLai.errorCode != null && resultBangLai.errorCode == 0)
                                        {
                                            headerOCR.action = ESCSStoredProcedures.PHT_OCR_BANG_LAI_OTO_NH;
                                            var resultOCR = resultBangLai.GetData();
                                            if (resultOCR != null)
                                            {
                                                resultOCR.ma_doi_tac_nsd = ma_doi_tac_nsd;
                                                resultOCR.ma_chi_nhanh_nsd = ma_chi_nhanh_nsd;
                                                resultOCR.nsd = nsd;
                                                resultOCR.pas = pas;
                                                resultOCR.so_id = item.so_id;
                                                resultOCR.so_id_doi_tuong = item.so_id_doi_tuong;
                                                resultOCR.ung_dung = "WEB";
                                                IDictionary<string, object> data = new Dictionary<string, object>();
                                                data = resultOCR.ModelToDictionary();
                                                await _dynamicService.ExcuteDynamicNewAsync(data, headerOCR, outPutSend => { });
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Có lỗi trong quá trình lưu thông tin OCR bằng lái xe:" + ex.Message);
                                    }
                                }
                                if (item.hang_muc == OCRConstant.HOA_DON_GIAY || item.hang_muc == OCRConstant.HOA_DON_DIEN_TU)
                                {
                                    try
                                    {
                                        HeaderRequest requestSave = header.Clone();
                                        requestSave.action = ESCSStoredProcedures.PHT_OCR_GIAY_TO_NH;
                                        Dictionary<string, bool> prefixHoaDon = new Dictionary<string, bool>();
                                        Dictionary<string, string> dataSave = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(data_info));
                                        dataSave.AddWithExists("bt", item.bt.ToString());
                                        dataSave.AddWithExists("nhom", item.nhom);
                                        List<ht_dich_vu_ocr> arrHangMucOCR = JsonConvert.DeserializeObject<List<ht_dich_vu_ocr>>(jsonHangMucOCR);
                                        int index = 0;
                                        var dataOCR = arrHangMucOCR.Where(n => n.hang_muc == item.ma_file);
                                        List<EAVModel> result = new List<EAVModel>();
                                        foreach (var key in dataOCR)
                                        {
                                            result.AddRange(EAVCommon.GetValueByKey(response, key.ma, key.loai, key.stt));
                                        }
                                        foreach (var itemHoaDon in result)
                                        {
                                            if (string.IsNullOrEmpty(itemHoaDon.gia_tri))
                                            {
                                                itemHoaDon.gia_tri = "";
                                            }
                                            dataSave.Add("arr[" + index + "][ma]", itemHoaDon.ma.ToString());
                                            dataSave.Add("arr[" + index + "][loai]", itemHoaDon.loai.ToString());
                                            dataSave.Add("arr[" + index + "][gia_tri]", itemHoaDon.gia_tri.ToString());
                                            dataSave.Add("arr[" + index + "][stt]", itemHoaDon.stt.ToString());
                                            index++;
                                        }
                                        BaseRequest rqSave = new BaseRequest(dataSave);
                                        BaseResponse<dynamic> resSave = new BaseResponse<dynamic>();
                                        prefixHoaDon.Add("arr", true);
                                        resSave.data_info = await _dynamicService.ExcuteDynamicAsync(rqSave, requestSave, prefixHoaDon, outValue =>
                                        {
                                            resSave.out_value = outValue;
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Có lỗi trong quá trình lưu thông tin OCR hóa đơn:" + ex.Message);
                                    }
                                }
                                //Chờ kế hoạch nâng cấp lại OCR cho MIC
                                if (item.hang_muc == OCRConstant.BAO_GIA_GARA && ma_doi_tac_nsd == "MIC")
                                {
                                    try
                                    {
                                        var resultBaoGia = JsonConvert.DeserializeObject<ocr_bao_gia_gara>(response);
                                        Dictionary<string, bool> prefixOCR = new Dictionary<string, bool>();
                                        if (resultBaoGia != null && resultBaoGia.errorCode == "0" && resultBaoGia.errorMessage.ToUpper() == "SUCCESS" && resultBaoGia.data.Count > 0)
                                        {
                                            var dataInfo = resultBaoGia.data.FirstOrDefault().info;
                                            if (dataInfo != null && dataInfo.table.Count > 0)
                                            {
                                                List<ht_ocr_bao_gia_gara> arrBaoGia = new List<ht_ocr_bao_gia_gara>();
                                                HeaderRequest requestSave = header.Clone();
                                                requestSave.action = ESCSStoredProcedures.PBH_BT_XE_HS_BAO_GIA_OCR_MAPPING;
                                                Dictionary<string, string> dataSave = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(data_info));
                                                dataSave.AddWithExists("bt_anh", item.bt.ToString());
                                                int index = 0;
                                                foreach (var lstItem in dataInfo.table)
                                                {
                                                    string ten_hang_muc_temp = lstItem.description.FormatString();
                                                    string don_gia_temp = lstItem.unit_price.FormatStringToInterger();
                                                    string so_luong_temp = lstItem.quantity.FormatStringToInterger();
                                                    string thanh_tien_temp = lstItem.amount_total.FormatStringToInterger();
                                                    string giam_gia_temp = lstItem.discount.Replace(",", "").Replace(".", "").Replace(" ", "");
                                                    if (string.IsNullOrEmpty(lstItem.quantity) && string.IsNullOrEmpty(lstItem.unit_price) && string.IsNullOrEmpty(lstItem.discount) && string.IsNullOrEmpty(lstItem.percent_discount) && string.IsNullOrEmpty(lstItem.amount_total) && string.IsNullOrEmpty(lstItem.tax))
                                                        continue;
                                                    decimal so_luong = 0, don_gia = 0, pt_giam_gia = 0, giam_gia = 0, thanh_tien = 0, tl_thue = 0;
                                                    var ktra_so_luong = Decimal.TryParse(so_luong_temp, out so_luong);
                                                    if (!ktra_so_luong)
                                                        so_luong = 0;
                                                    var ktra_don_gia = Decimal.TryParse(don_gia_temp, out don_gia);
                                                    if (!ktra_don_gia)
                                                        don_gia = 0;
                                                    var ktra_pt_giam_gia = Decimal.TryParse(lstItem.percent_discount, out pt_giam_gia);
                                                    if (!ktra_pt_giam_gia)
                                                        pt_giam_gia = 0;
                                                    var ktra_giam_gia = Decimal.TryParse(giam_gia_temp, out giam_gia);
                                                    if (!ktra_giam_gia)
                                                        giam_gia = 0;
                                                    var ktra_thanh_tien = Decimal.TryParse(thanh_tien_temp, out thanh_tien);
                                                    if (!ktra_thanh_tien)
                                                        thanh_tien = 0;
                                                    var ktra_tl_thue = Decimal.TryParse(lstItem.tax, out tl_thue);
                                                    if (!ktra_tl_thue)
                                                        tl_thue = 0;
                                                    arrBaoGia.Add(new ht_ocr_bao_gia_gara() { ngay_bao_gia = dataInfo.quotation_date, ma_gara = ma_gara, ten_hang_muc = ten_hang_muc_temp, so_luong = so_luong, don_gia = don_gia, giam_gia = giam_gia, pt_giam_gia = pt_giam_gia, thanh_tien = thanh_tien, tl_thue = tl_thue });
                                                }
                                                foreach (var itemBaoGia in arrBaoGia)
                                                {
                                                    dataSave.Add("arr[" + index + "][ngay_bao_gia]", itemBaoGia.ngay_bao_gia);
                                                    dataSave.Add("arr[" + index + "][ten_hang_muc]", itemBaoGia.ten_hang_muc);
                                                    dataSave.Add("arr[" + index + "][so_luong]", itemBaoGia.so_luong.ToString());
                                                    dataSave.Add("arr[" + index + "][don_gia]", itemBaoGia.don_gia.ToString());
                                                    dataSave.Add("arr[" + index + "][pt_giam_gia]", itemBaoGia.pt_giam_gia.ToString());
                                                    dataSave.Add("arr[" + index + "][giam_gia]", itemBaoGia.giam_gia.ToString());
                                                    dataSave.Add("arr[" + index + "][thanh_tien]", itemBaoGia.thanh_tien.ToString());
                                                    dataSave.Add("arr[" + index + "][tl_thue]", itemBaoGia.tl_thue.ToString());
                                                    index++;
                                                }
                                                BaseRequest rqSave = new BaseRequest(dataSave);
                                                BaseResponse<dynamic> resSave = new BaseResponse<dynamic>();
                                                prefixOCR.Add("arr", true);
                                                resSave.data_info = await _dynamicService.ExcuteDynamicAsync(rqSave, requestSave, prefixOCR, outValue =>
                                                {
                                                    resSave.out_value = outValue;
                                                });
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Có lỗi trong quá trình đọc OCR báo giá gara " + ex.Message);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Có lỗi trong quá trình đọc dữ liệu OCR" + ex.Message);
                            }
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy đường đẫn OCR");
                        }
                    }
                }
                else
                {
                    throw new Exception("Chưa có dữ liệu OCR giấy tờ");
                }
                BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
                resSuccess.state_info.status = STATUS_OK;
                resSuccess.state_info.message_code = STATUS_OK;
                resSuccess.state_info.message_body = "Đọc OCR thành công";
                return Ok(resSuccess);
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// /// <summary>
        /// Đọc OCR báo giá gara
        /// </summary>
        /// <returns></returns>
        [Route("price-quotation")]
        [HttpPost]
        public async Task<IActionResult> OCRPriceQuotation()
        {
            try
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
                if (action == null || action.actionapicode != ESCSStoredProcedures.PHT_BH_FILE_DOC_BAO_GIA_OCR)
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                    return Ok(res);
                }
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();

                var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                var ma_chi_nhanh_nsd = data_info.GetString("ma_chi_nhanh_nsd");
                var nsd = data_info.GetString("nsd");
                var pas = data_info.GetString("pas");
                var so_id = data_info.GetString("so_id");
                var ma_gara = data_info.GetString("gara");
                if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(ma_chi_nhanh_nsd) || so_id == null)
                    throw new Exception("Không xác định được thông tin hồ sơ bồi thường");
                if (string.IsNullOrEmpty(ma_gara))
                    throw new Exception("Không xác định được gara báo giá");
                #endregion
                #region Kiểm tra cache danh mục bộ mã map hạng mục OCR, nếu chưa có thì lấy cache ra
                HeaderRequest requestDB = header.Clone();
                BaseResponse<dynamic> baseResponse = new BaseResponse<dynamic>();
                var paramRequest = new Dictionary<string, object>();
                paramRequest.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd.ToString());
                paramRequest.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd.ToString());
                paramRequest.AddWithExists("nsd", nsd.ToString());
                paramRequest.AddWithExists("pas", pas.ToString());
                paramRequest.AddWithExists("ma_doi_tac", ma_doi_tac_nsd.ToString());
                var jsonHangMucMap = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, "HANG_MUC_MAPPING_OCR", RedisCacheMaster.DatabaseIndex);
                var jsonHangMuc = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, "HANG_MUC_AI", RedisCacheMaster.DatabaseIndex);
                if (string.IsNullOrEmpty(jsonHangMucMap))
                {
                    requestDB.action = ESCSStoredProcedures.PBH_BT_XE_HS_BAO_GIA_HANG_MUC_OCR_CACHE;
                    baseResponse.data_info = await _dynamicService.ExcuteDynamicNewAsync(paramRequest, requestDB);
                    if (baseResponse.data_info != null)
                    {
                        jsonHangMucMap = JsonConvert.SerializeObject(baseResponse.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                        if (!string.IsNullOrEmpty(jsonHangMucMap))
                        {
                            List<bh_bt_xe_hs_bao_gia_hang_muc_ocr> arrHangMucMap = JsonConvert.DeserializeObject<List<bh_bt_xe_hs_bao_gia_hang_muc_ocr>>(jsonHangMucMap);
                            _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, "HANG_MUC_MAPPING_OCR", arrHangMucMap, DateTime.Now.AddYears(1) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                        }
                    }
                }
                if (string.IsNullOrEmpty(jsonHangMuc))
                {
                    requestDB.action = ESCSStoredProcedures.PBH_HT_MA_XE_HANG_MUC_AI;
                    baseResponse.data_info = await _dynamicService.ExcuteDynamicNewAsync(paramRequest, requestDB);
                    if (baseResponse.data_info != null)
                    {
                        jsonHangMuc = JsonConvert.SerializeObject(baseResponse.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                        if (!string.IsNullOrEmpty(jsonHangMuc))
                        {
                            List<ht_ma_xe_hang_muc> arrHangMucAI = JsonConvert.DeserializeObject<List<ht_ma_xe_hang_muc>>(jsonHangMuc);
                            _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, "HANG_MUC_AI", arrHangMucAI, DateTime.Now.AddYears(1) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                        }
                    }
                }
                #endregion
                #region Đọc OCR
                var dataLichDoc = await _dynamicService.ExcuteListAsync<ht_lich_doc_bao_gia_ocr>(data_info, header);
                if (dataLichDoc != null && dataLichDoc.Count() > 0)
                {
                    var lichDoc = dataLichDoc.OrderBy(item => item.stt)
                     .GroupBy(item => item.duong_dan_ocr)
                     .Select(gr => new { Key = gr.Key, Value = gr.ToList() })
                     .ToList();
                    if (lichDoc != null && lichDoc.Count() > 0)
                    {
                        foreach (var itemLichDoc in lichDoc)
                        {
                            IEnumerable<ht_lich_doc_bao_gia_ocr> lichDocOCR = itemLichDoc.Value;
                            var result = lichDocOCR.FirstOrDefault();
                            string response = string.Empty;
                            string pathFileOCR = Path.Combine(AppSettings.PathFolderNotDeleteFull, itemLichDoc.Key).ChuanHoaDuongDanRemote();
                            if (pathFileOCR.Contains("error"))
                                continue;
                            if (System.IO.File.Exists(pathFileOCR) && !string.IsNullOrEmpty(pathFileOCR))
                            {
                                try
                                {
                                    List<bh_bt_xe_hs_bao_gia_hang_muc_ocr> arrHangMucMap = JsonConvert.DeserializeObject<List<bh_bt_xe_hs_bao_gia_hang_muc_ocr>>(jsonHangMucMap);
                                    List<ht_ma_xe_hang_muc> arrHangMuc = JsonConvert.DeserializeObject<List<ht_ma_xe_hang_muc>>(jsonHangMuc);
                                    response = System.IO.File.ReadAllText(pathFileOCR);
                                    var resultBaoGia = JsonConvert.DeserializeObject<ocr_bao_gia_gara>(response);
                                    Dictionary<string, bool> prefixOCR = new Dictionary<string, bool>();
                                    if (resultBaoGia != null && resultBaoGia.errorCode == "0" && resultBaoGia.errorMessage.ToUpper() == "SUCCESS" && resultBaoGia.data.Count > 0)
                                    {
                                        var dataInfo = resultBaoGia.data.FirstOrDefault().info;
                                        if (dataInfo != null && dataInfo.table.Count > 0)
                                        {
                                            List<ht_ocr_bao_gia_gara> arrBaoGia = new List<ht_ocr_bao_gia_gara>();
                                            HeaderRequest requestSave = header.Clone();
                                            requestSave.action = ESCSStoredProcedures.PBH_BT_XE_HS_BAO_GIA_OCR_MAPPING;
                                            Dictionary<string, string> dataSave = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(data_info));
                                            dataSave.AddWithExists("bt_anh", result.bt.ToString());
                                            int index = 0;
                                            double? tl_khop = 0.0;
                                            foreach (var lstItem in dataInfo.table)
                                            {
                                                string ma_hang_muc_he_thong = "", ten_hang_muc_he_thong = "", loai_tien = "";
                                                string ten_hang_muc_map = lstItem.description.BAFBODAU();
                                                string ten_hang_muc_temp = lstItem.description.FormatString();
                                                string don_gia_temp = lstItem.unit_price.FormatStringToInterger();
                                                string so_luong_temp = lstItem.quantity.FormatStringToInterger();
                                                string thanh_tien_temp = lstItem.amount_total.FormatStringToInterger();
                                                string giam_gia_temp = lstItem.discount.Replace(",", "").Replace(".", "").Replace(" ", "");
                                                var objMaping = arrHangMucMap.Where(n => n.ten_hang_muc == ten_hang_muc_map).FirstOrDefault();
                                                if (objMaping != null)
                                                {
                                                    ma_hang_muc_he_thong = objMaping.ma_hang_muc;
                                                    ten_hang_muc_he_thong = objMaping.ten;
                                                    loai_tien = objMaping.loai_tien;
                                                    tl_khop = Convert.ToDouble(objMaping.tl_khop);
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(ma_hang_muc_he_thong))
                                                    {
                                                        double? ti_le_max = 0;
                                                        foreach (var itemMap in arrHangMuc)
                                                        {
                                                            if (!string.IsNullOrEmpty(itemMap.ten_alias))
                                                            {
                                                                string[] arrTenAlias = itemMap.ten_alias.Split(",");
                                                                foreach (var item in arrTenAlias)
                                                                {
                                                                    tl_khop = Math.Round(TextCompare.CalculateSimilarity(ten_hang_muc_map, item), 2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                tl_khop = Math.Round(TextCompare.CalculateSimilarity(ten_hang_muc_map, itemMap.ten_tim), 2);
                                                            }
                                                            if (tl_khop > ti_le_max)
                                                            {
                                                                ma_hang_muc_he_thong = itemMap.ma;
                                                                ten_hang_muc_he_thong = itemMap.ten;
                                                                ti_le_max = tl_khop;
                                                                loai_tien = TextCompare.GetCurrency(ten_hang_muc_map);
                                                            }
                                                        }
                                                    }
                                                }
                                                if (string.IsNullOrEmpty(lstItem.quantity) && string.IsNullOrEmpty(lstItem.unit_price) && string.IsNullOrEmpty(lstItem.discount) && string.IsNullOrEmpty(lstItem.percent_discount) && string.IsNullOrEmpty(lstItem.amount_total) && string.IsNullOrEmpty(lstItem.tax))
                                                    continue;
                                                decimal so_luong = 0, don_gia = 0, pt_giam_gia = 0, giam_gia = 0, thanh_tien = 0, tl_thue = 0;
                                                var ktra_so_luong = Decimal.TryParse(so_luong_temp, out so_luong);
                                                if (!ktra_so_luong)
                                                    so_luong = 0;
                                                var ktra_don_gia = Decimal.TryParse(don_gia_temp, out don_gia);
                                                if (!ktra_don_gia)
                                                    don_gia = 0;
                                                var ktra_pt_giam_gia = Decimal.TryParse(lstItem.percent_discount, out pt_giam_gia);
                                                if (!ktra_pt_giam_gia)
                                                    pt_giam_gia = 0;
                                                var ktra_giam_gia = Decimal.TryParse(giam_gia_temp, out giam_gia);
                                                if (!ktra_giam_gia)
                                                    giam_gia = 0;
                                                var ktra_thanh_tien = Decimal.TryParse(thanh_tien_temp, out thanh_tien);
                                                if (!ktra_thanh_tien)
                                                    thanh_tien = 0;
                                                var ktra_tl_thue = Decimal.TryParse(lstItem.tax, out tl_thue);
                                                if (!ktra_tl_thue)
                                                    tl_thue = 0;
                                                arrBaoGia.Add(new ht_ocr_bao_gia_gara() { ngay_bao_gia = dataInfo.quotation_date, ma_gara = ma_gara, ten_hang_muc = ten_hang_muc_temp, ma_hang_muc_he_thong = ma_hang_muc_he_thong, ten_hang_muc_he_thong = ten_hang_muc_he_thong, so_luong = so_luong, don_gia = don_gia, giam_gia = giam_gia, pt_giam_gia = pt_giam_gia, thanh_tien = thanh_tien, tl_thue = tl_thue, tl_khop = Convert.ToDecimal(tl_khop), loai_tien = loai_tien });
                                            }
                                            foreach (var itemBaoGia in arrBaoGia)
                                            {
                                                dataSave.Add("arr[" + index + "][ma_hang_muc_ht]", itemBaoGia.ma_hang_muc_he_thong.ToString());
                                                dataSave.Add("arr[" + index + "][ten_hang_muc_ht]", itemBaoGia.ten_hang_muc_he_thong.ToString());
                                                dataSave.Add("arr[" + index + "][ngay_bao_gia]", itemBaoGia.ngay_bao_gia);
                                                dataSave.Add("arr[" + index + "][ten_hang_muc]", itemBaoGia.ten_hang_muc.ToString());
                                                dataSave.Add("arr[" + index + "][so_luong]", itemBaoGia.so_luong.ToString());
                                                dataSave.Add("arr[" + index + "][don_gia]", itemBaoGia.don_gia.ToString());
                                                dataSave.Add("arr[" + index + "][pt_giam_gia]", itemBaoGia.pt_giam_gia.ToString());
                                                dataSave.Add("arr[" + index + "][giam_gia]", itemBaoGia.giam_gia.ToString());
                                                dataSave.Add("arr[" + index + "][thanh_tien]", itemBaoGia.thanh_tien.ToString());
                                                dataSave.Add("arr[" + index + "][tl_thue]", itemBaoGia.tl_thue.ToString());
                                                dataSave.Add("arr[" + index + "][loai_tien]", itemBaoGia.loai_tien.ToString());
                                                dataSave.Add("arr[" + index + "][tl_khop]", itemBaoGia.tl_khop.ToString());
                                                index++;
                                            }
                                            BaseRequest rqSave = new BaseRequest(dataSave);
                                            BaseResponse<dynamic> resSave = new BaseResponse<dynamic>();
                                            prefixOCR.Add("arr", true);
                                            resSave.data_info = await _dynamicService.ExcuteDynamicAsync(rqSave, requestSave, prefixOCR, outValue =>
                                            {
                                                resSave.out_value = outValue;
                                            });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Lỗi lưu báo giá: " + ex.Message);
                                }
                            }
                            else
                            {
                                throw new Exception("Không tìm thấy đường đẫn OCR báo giá!");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Chưa có dữ liệu OCR báo giá gara!");
                    }
                }
                #endregion
                #region Kết quả trả về
                BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
                resSuccess.state_info.status = STATUS_OK;
                resSuccess.state_info.message_code = STATUS_OK;
                resSuccess.state_info.message_body = "Đọc OCR thành công";
                return Ok(resSuccess);
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Nhận diện ảnh tổn thất
        /// </summary>
        /// <returns></returns>
        [Route("detect-image")]
        [HttpPost]
        public async Task<IActionResult> DetectImage()
        {
            try
            {
                //Action lấy danh sách file cần nhận diện
                //PBH_FILE_NHAN_DIEN_AI
                #region Lấy thông tin header và kiểm tra token
                HeaderRequest header = Request.GetHeader();
                string payload = string.Empty;
                var data_info = Request.GetData(out var define_info, out payload);

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
                {
                    BaseResponse<dynamic> resErr001 = new BaseResponse<dynamic>();
                    resErr001.state_info.status = STATUS_NOTOK;
                    resErr001.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    resErr001.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                    return Ok(resErr001);
                }
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();

                if (action.action_type != "FILE")
                    throw new Exception("Không xác định loại Action Type");
                #endregion
                #region Lấy thông tin cấu hình upload
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
                #endregion

                List<StatusUploadFile> status = new List<StatusUploadFile>();
                #region Nhận diện ảnh tổn thất
                try
                {
                    string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                    HeaderRequest headerDetect = header.Clone();
                    headerDetect.action = ESCSStoredProcedures.PBH_DICH_VU_KIEM_TRA;
                    IDictionary<string, object> kiemTraDichVu = new Dictionary<string, object>();
                    kiemTraDichVu.Add("ma_doi_tac_nsd", ma_doi_tac_nsd);
                    kiemTraDichVu.Add("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    kiemTraDichVu.Add("nsd", data_info.GetString("nsd"));
                    kiemTraDichVu.Add("pas", data_info.GetString("pas"));
                    kiemTraDichVu.Add("so_id", data_info.GetString("so_id"));
                    kiemTraDichVu.Add("ma_dich_vu", "DICH_VU_AI");
                    kiemTraDichVu.Add("bt", "");
                    kiemTraDichVu.Add("so_id_doi_tuong", data_info.GetString("so_id_doi_tuong"));

                    var cauHinh = await _dynamicService.ExcuteDynamicNewAsync(kiemTraDichVu, headerDetect);
                    string json = JsonConvert.SerializeObject(cauHinh);
                    bh_dich_vu<bh_dich_vu_ai> cauHinhDV = JsonConvert.DeserializeObject<bh_dich_vu<bh_dich_vu_ai>>(json);
                    var dich_vu = cauHinhDV.dich_vu;
                    if (dich_vu == null || dich_vu.ap_dung == 0)
                        throw new Exception("Dịch vụ chưa được cấu hình hoặc chưa được áp dụng");
                    //Lấy danh sách file cần nhận diện
                    var ds_anh = await _dynamicService.ExcuteListAsync<bh_file>(data_info, header);
                    if (ds_anh != null && ds_anh.Count() > 0)
                    {
                        foreach (var item in ds_anh)
                        {
                            var arr_byte = Utilities.FileToByteArray(Path.Combine(networkCredential.FullPath, item.duong_dan));
                            if (arr_byte == null)
                                continue;
                            kiemTraDichVu.AddWithExists("bt_ai", item.bt.ToString());
                            //var image = Utilities.ResizeImage(Utilities.byteArrayToImage(arr_byte), new Size(550, 550));
                            //var arr = Utilities.ImageToByteArray(image);
                            var result = await OCRService.NhanDienAnhTonThat<ai_response>(dich_vu.base_url, dich_vu.api_key, arr_byte);
                            var base64 = result.base64;
                            result.base64 = string.IsNullOrEmpty(base64) ? "Không có dữ liệu base64" : "Có dữ liệu base64";
                            kiemTraDichVu.AddWithExists("ai_kq", JsonConvert.SerializeObject(result));
                            kiemTraDichVu.AddWithExists("gid", Guid.NewGuid().ToString("N"));
                            kiemTraDichVu.AddWithExists("duong_dan", "");
                            if (result.code == AI_CONSTANT.SUCCESS && result.damage != null && result.damage.Count > 0)
                            {
                                headerDetect.action = ESCSStoredProcedures.PBH_FILE_AI_NH;
                                var now = DateTime.Now;
                                string fileNameOutPut = now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + ".jpg";
                                var month = now.Month.ToString();
                                var day = now.Day.ToString();
                                if (month.Length < 2)
                                    month = "0" + month;
                                if (day.Length < 2)
                                    day = "0" + day;
                                string pathFileOutput = Path.Combine(ma_doi_tac_nsd, "TAI_LIEU", now.Year.ToString(), month, day);
                                string output = Path.Combine(networkCredential.FullPath, pathFileOutput, fileNameOutPut);
                                Utilities.CreateFolderWhenNotExist(networkCredential.FullPath, pathFileOutput, networkCredential);
                                byte[] imageBytes = Convert.FromBase64String(base64);
                                System.IO.File.WriteAllBytes(output, imageBytes);
                                kiemTraDichVu.AddWithExists("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                                var index_damage = 0;
                                foreach (var damage in result.damage)
                                {
                                    kiemTraDichVu.Add("damage[" + index_damage + "][type]", damage.damage_type);
                                    kiemTraDichVu.Add("damage[" + index_damage + "][parts]", damage.parts);
                                    kiemTraDichVu.Add("damage[" + index_damage + "][box]", damage.damage_box);
                                    kiemTraDichVu.Add("damage[" + index_damage + "][score]", damage.damage_score);
                                    index_damage++;
                                }
                                var data_dic = kiemTraDichVu.ToDictionary(n => n.Key, n => n.Value == null ? "" : n.Value.ToString());
                                BaseRequest rq = new BaseRequest(data_dic);
                                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                                Dictionary<string, bool> prefix_tmp = new Dictionary<string, bool>();
                                prefix_tmp.Add("damage", true);
                                await _dynamicService.ExcuteDynamicAsync(rq, headerDetect, prefix_tmp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                #endregion
                #region Kết quả trả về
                BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
                resSuccess.state_info.status = STATUS_OK;
                resSuccess.state_info.message_code = STATUS_OK;
                resSuccess.data_info = status;
                resSuccess.state_info.message_body = "Hoàn thành nhận diện";
                return Ok(resSuccess);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Upload file vào thư mục
        /// </summary>
        /// <returns></returns>
        [Route("upload-file-path")]
        [HttpPost]
        public IActionResult UploadFileToPath()
        {
            try
            {
                BaseRequest<List<file_uploads>> baseRequest = new BaseRequest<List<file_uploads>>();
                string requestData = "";
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    requestData = HttpUtility.UrlDecode(reader.ReadToEnd());
                }
                baseRequest = JsonConvert.DeserializeObject<BaseRequest<List<file_uploads>>>(requestData);
                List<file_uploads> files = baseRequest.data_info;
                List<file_cau_hinh> cauHinh = new List<file_cau_hinh>()
                {
                    new file_cau_hinh(){code="MAU_IN_PDF", extension=".xml", folder = Path.Combine("MAU_IN","PDF")},
                    new file_cau_hinh(){code="MAU_IN_EXCEL", extension=".xls,.xlsx", folder = Path.Combine("MAU_IN","EXCEL")},
                    new file_cau_hinh(){code="TEMPLATE_MAIL", extension=".cshtml", folder = "TEMPLATE_MAIL"},
                    new file_cau_hinh(){code="CHUNG_CHI_KY_SO", extension=".pfx", folder = "CHUNG_CHI_KY_SO"}
                };

                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                if (files == null || files.Count <= 0)
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                    res.state_info.message_body = "Không có dữ liệu upload file";
                    return Ok(res);
                }
                foreach (var file in files)
                {
                    if (string.IsNullOrEmpty(file.ma_doi_tac))
                    {
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                        res.state_info.message_body = "Thiếu thông tin mã đối tác";
                        return Ok(res);
                    }
                    var file_cau_hinh = cauHinh.Where(n => n.code == file.loai).FirstOrDefault();
                    if (string.IsNullOrEmpty(file.loai) || file_cau_hinh == null)
                    {
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                        res.state_info.message_body = "Thiếu hoặc sai thông tin loại file upload";
                        return Ok(res);
                    }
                    if (string.IsNullOrEmpty(file.file_base64))
                    {
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                        res.state_info.message_body = "Không tìm thấy file base64";
                        return Ok(res);
                    }
                    file.file_base64 = file.file_base64.Replace(" ", "+");
                    var ext = Utilities.GetFileExtension(file.file_base64);
                    string fileExt = Path.GetExtension(file.path);
                    if (string.IsNullOrEmpty(ext) || !file_cau_hinh.extension.Split(",").Contains(ext) || ext != fileExt)
                    {
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                        res.state_info.message_body = "Không đúng định dạng file hoặc tên file không khớp định dạng file";
                        return Ok(res);
                    }
                    string fileName = Path.GetFileName(file.path);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                        res.state_info.message_body = "Không tìm thấy tên file";
                        return Ok(res);
                    }
                    string basePath = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.ma_doi_tac, file_cau_hinh.folder).ChuanHoaDuongDanRemote();
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    file.path = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.ma_doi_tac, file_cau_hinh.folder, fileName).ChuanHoaDuongDanRemote();
                }
                #region Lấy thông tin header và kiểm tra token
                HeaderRequest header = Request.GetHeader();
                Dictionary<string, bool> prefix = new Dictionary<string, bool>();
                openid_access_token token = null;
                string keyCachePartner = string.Empty;
                var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner, true, false);
                if (!string.IsNullOrEmpty(vetifyTokenMessage))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                    res.state_info.message_body = vetifyTokenMessage;
                    return Ok(res);
                }
                header.envcode = token.evncode;
                #endregion

                foreach (var file in files)
                {

                    var byteArr = Convert.FromBase64String(file.file_base64);
                    System.IO.File.WriteAllBytes(file.path, Convert.FromBase64String(file.file_base64));
                }
                #region Kết quả trả về
                res.state_info.status = STATUS_OK;
                res.state_info.message_code = STATUS_OK;
                res.data_info = files;
                res.state_info.message_body = "Upload thành công";
                return Ok(res);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Lấy ảnh thumnail
        /// </summary>
        /// <returns></returns>
        [Route("get-file-thumnail")]
        [HttpPost]
        public async Task<IActionResult> GetFilesThumnail()
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
                if (action.action_type == "FILE")
                {
                    #region Thực thi type excute db
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    if (res.data_info == null)
                    {
                        return Ok(res);
                    }
                    string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                    if (string.IsNullOrEmpty(jsonRes))
                    {
                        return Ok(res);
                    }
                    List<bh_file> dsFiles = JsonConvert.DeserializeObject<List<bh_file>>(jsonRes);
                    var extImage = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
                    var extDoc = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };

                    foreach (var item in dsFiles)
                    {
                        if (!string.IsNullOrEmpty(item.duong_dan))
                        {
                            item.extension = Path.GetExtension(item.duong_dan).ToLower();
                            string fileName = Path.GetFileName(item.duong_dan);
                            if (extImage.Contains(item.extension))
                            {
                                string fileNameMini = action.prefix_mini + "_" + fileName;
                                item.duong_dan = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan.Replace(fileName, fileNameMini)).ChuanHoaDuongDanRemote();
                                if (AppSettings.FolderSharedUsed && item.duong_dan.StartsWith(@"\") && !item.duong_dan.StartsWith(@"\\"))
                                    item.duong_dan = @"\" + item.duong_dan;
                                item.duong_dan = Utilities.ConvertFileToBase64String(item.duong_dan);
                                if (string.IsNullOrEmpty(item.duong_dan))
                                    item.duong_dan = Utilities.ConvertFileToBase64String(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "image_default.png"));
                            }
                            if (string.IsNullOrEmpty(item.ma_file))
                                item.ma_file = "";
                        }
                        else
                        {
                            item.duong_dan = Utilities.ConvertFileToBase64String(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "image_default.png"));
                            item.extension = ".jpg";
                        }
                    }
                    res.data_info = dsFiles;
                    return Ok(res);
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

        /// <summary>
        /// Lấy ảnh thumnail api
        /// </summary>
        /// <returns></returns>
        [Route("get-file-thumnail-api")]
        [HttpPost]
        public async Task<IActionResult> GetFilesThumnailApi()
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
                if (action.action_type == "FILE")
                {
                    #region Thực thi type excute db
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    if (res.data_info == null)
                    {
                        return Ok(res);
                    }
                    string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                    if (string.IsNullOrEmpty(jsonRes))
                    {
                        return Ok(res);
                    }
                    List<bh_file> dsFiles = JsonConvert.DeserializeObject<List<bh_file>>(jsonRes);

                    var extImage = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
                    var extDoc = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };

                    foreach (var item in dsFiles)
                    {
                        if (!string.IsNullOrEmpty(item.duong_dan))
                        {
                            item.extension = Path.GetExtension(item.duong_dan).ToLower();
                            string fileName = Path.GetFileName(item.duong_dan);
                            if (extImage.Contains(item.extension))
                            {
                                string fileNameMini = action.prefix_mini + "_" + fileName;
                                item.duong_dan = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan.Replace(fileName, fileNameMini)).ChuanHoaDuongDanRemote();
                                item.duong_dan = Utilities.ConvertFileToBase64String(item.duong_dan);
                            }
                            if (extDoc.Contains(item.extension))
                            {
                                item.duong_dan = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan).ChuanHoaDuongDanRemote();
                                item.duong_dan = Utilities.ConvertFileToBase64String(item.duong_dan);
                            }
                            if (string.IsNullOrEmpty(item.ma_file))
                                item.ma_file = "";
                        }
                    }
                    res.data_info = dsFiles;
                    return Ok(res);
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

        /// <summary>
        /// Lấy ảnh thumnail đánh giá rủi ro
        /// </summary>
        /// <returns></returns>
        [Route("get-thumnail-dgrr")]
        [HttpPost]
        public async Task<IActionResult> GetFilesThumnailDGRR()
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
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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
                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                if (res.data_info == null)
                    return Ok(res);
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    return Ok(res);
                List<bh_file_dgrr> dsFiles = JsonConvert.DeserializeObject<List<bh_file_dgrr>>(jsonRes);
                var extImage = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
                var extDoc = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };

                foreach (var item in dsFiles)
                {
                    if (!string.IsNullOrEmpty(item.duong_dan))
                    {
                        item.extension = Path.GetExtension(item.duong_dan).ToLower();
                        string fileName = Path.GetFileName(item.duong_dan);
                        if (extImage.Contains(item.extension))
                        {
                            string fileNameMini = "thumnail_" + fileName;
                            item.duong_dan = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan.Replace(fileName, fileNameMini)).ChuanHoaDuongDanRemote();
                            if (AppSettings.FolderSharedUsed && item.duong_dan.StartsWith(@"\") && !item.duong_dan.StartsWith(@"\\"))
                                item.duong_dan = @"\" + item.duong_dan;
                            item.duong_dan = Utilities.ConvertFileToBase64String(item.duong_dan);
                            if (string.IsNullOrEmpty(item.duong_dan))
                                item.duong_dan = Utilities.ConvertFileToBase64String(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "image_default.png"));
                        }
                        if (string.IsNullOrEmpty(item.ma_file))
                            item.ma_file = "";
                    }
                    else
                    {
                        item.duong_dan = Utilities.ConvertFileToBase64String(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "image_default.png"));
                        item.extension = ".jpg";
                    }
                }
                res.data_info = dsFiles;
                return Ok(res);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Lấy danh sách file tài liệu yêu cầu bổ sung
        /// </summary>
        /// <returns></returns>
        [Route("get-document")]
        [HttpPost]
        public async Task<IActionResult> GetThumnailDocument()
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
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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
                if (header.action != ESCSStoredProcedures.PMOBILE_DOI_TAC_HO_SO_GIAY_TO_YCBS)
                    throw new Exception("API Không xác định hành động");

                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; });
                if (res.data_info == null)
                    return Ok(res);
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    return Ok(res);

                List<bh_file_tai_lieu_ycbs> dsFiles = JsonConvert.DeserializeObject<List<bh_file_tai_lieu_ycbs>>(jsonRes);
                var extImage = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
                var extDoc = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };

                foreach (var item in dsFiles)
                {
                    item.extension = Path.GetExtension(item.duong_dan).ToLower();
                    string fileName = Path.GetFileName(item.duong_dan);
                    if (extImage.Contains(item.extension))
                    {
                        string fileNameMini = "thumnail_" + fileName;
                        item.duong_dan = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan.Replace(fileName, fileNameMini)).ChuanHoaDuongDanRemote();
                        if (AppSettings.FolderSharedUsed && item.duong_dan.StartsWith(@"\") && !item.duong_dan.StartsWith(@"\\"))
                            item.duong_dan = @"\" + item.duong_dan;
                        item.duong_dan = Utilities.ConvertFileToBase64String(item.duong_dan);
                        if (string.IsNullOrEmpty(item.duong_dan))
                            item.duong_dan = Utilities.ConvertFileToBase64String(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "image_default.png"));
                    }
                    if (string.IsNullOrEmpty(item.ma_file))
                        item.ma_file = "";
                }
                res.data_info = dsFiles;
                return Ok(res);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Xem chi tiết ảnh hồ sơ giấy tờ
        /// </summary>
        /// <returns></returns>
        [Route("get-document-detail")]
        [HttpPost]
        public async Task<IActionResult> GetThumnailDocumentDetail()
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
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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
                if (header.action != ESCSStoredProcedures.PMOBILE_DOI_TAC_HO_SO_GIAY_TO_YCBS_CT)
                    throw new Exception("API Không xác định hành động");

                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                if (res.data_info == null)
                    return Ok(res);
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    return Ok(res);
                bh_file_tai_lieu_ycbs file = JsonConvert.DeserializeObject<bh_file_tai_lieu_ycbs>(jsonRes);

                string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                    fileName = @"\" + fileName;

                file.extension = Path.GetExtension(fileName);
                file.duong_dan = Utilities.ConvertFileToBase64String(fileName);
                res.data_info = file;

                return Ok(res);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Lấy ảnh
        /// </summary>
        /// <returns></returns>
        [Route("get-paging-file")]
        [HttpPost]
        public async Task<IActionResult> GetPagingFile()
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
            try
            {
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
                BaseResponse<Pagination> resPonseBase = new BaseResponse<Pagination>();
                resPonseBase.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                {
                    resPonseBase.out_value = outValue;
                });
                string jsonRes = JsonConvert.SerializeObject(resPonseBase.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                BaseResponse<PaginationGenneric<bh_file>> resPonse = new BaseResponse<PaginationGenneric<bh_file>>();
                resPonse.data_info = JsonConvert.DeserializeObject<PaginationGenneric<bh_file>>(jsonRes);
                if (resPonse.data_info == null || resPonse.data_info.data == null || resPonse.data_info.data.Count() <= 0)
                    return Ok(resPonse);
                var extImage = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
                var extDoc = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };
                foreach (var item in resPonse.data_info.data)
                {
                    item.extension = Path.GetExtension(item.duong_dan).ToLower();
                    string fileName = Path.GetFileName(item.duong_dan);
                    var path = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan).ChuanHoaDuongDanRemote();
                    if (System.IO.File.Exists(path))
                    {
                        item.duong_dan = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan).ChuanHoaDuongDanRemote();
                        item.duong_dan = Utilities.ConvertFileToBase64String(item.duong_dan);
                    }
                    else
                    {
                        item.duong_dan = null;
                    }
                }
                resPonse.data_info.data = resPonse.data_info.data.Where(n => !string.IsNullOrEmpty(n.duong_dan)).ToList();
                return Ok(resPonse);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }
        /// <summary>
        /// Xem ảnh (giao thức FTP)
        /// </summary>
        /// <returns></returns>
        [Route("view-list-file")]
        [HttpPost]
        public async Task<IActionResult> ViewListFile()
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
            try
            {
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
                BaseResponse<Pagination> resPonseBase = new BaseResponse<Pagination>();
                resPonseBase.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                {
                    resPonseBase.out_value = outValue;
                });
                string jsonRes = JsonConvert.SerializeObject(resPonseBase.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                BaseResponse<PaginationGenneric<bh_file>> resPonse = new BaseResponse<PaginationGenneric<bh_file>>();
                resPonse.data_info = JsonConvert.DeserializeObject<PaginationGenneric<bh_file>>(jsonRes);
                if (resPonse.data_info == null || resPonse.data_info.data == null || resPonse.data_info.data.Count() <= 0)
                    return Ok(resPonse);
                var extImage = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
                var extDoc = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };

                foreach (var item in resPonse.data_info.data)
                {
                    item.extension = Path.GetExtension(item.duong_dan).ToLower();
                    string fileName = Path.GetFileName(item.duong_dan);
                    var path = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan).ChuanHoaDuongDanRemote();
                    if (System.IO.File.Exists(path))
                    {
                        item.duong_dan = Path.Combine(AppSettings.PathFolderNotDeleteFull, item.duong_dan).ChuanHoaDuongDanRemote();
                        item.duong_dan = Utilities.ConvertFileToBase64String(item.duong_dan);
                    }
                    else
                    {
                        item.duong_dan = null;
                    }
                }
                resPonse.data_info.data = resPonse.data_info.data.Where(n => !string.IsNullOrEmpty(n.duong_dan)).ToList();
                return Ok(resPonse);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Tải 1 file
        /// </summary>
        /// <returns></returns>
        [Route("get-file")]
        [HttpPost]
        public async Task<IActionResult> GetFile()
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
                if (action.action_type == "FILE")
                {
                    #region Thực thi type excute db
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                    if (res.data_info == null)
                        return Ok(res);
                    string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                    if (string.IsNullOrEmpty(jsonRes))
                        return Ok(res);
                    bh_file file = JsonConvert.DeserializeObject<bh_file>(jsonRes);
                    if (!string.IsNullOrEmpty(file.duong_dan))
                    {
                        string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                        if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                            fileName = @"\" + fileName;

                        file.extension = Path.GetExtension(fileName);
                        file.duong_dan = Utilities.ConvertFileToBase64String(fileName);
                        if (!string.IsNullOrEmpty(file.duong_dan_ai))
                            file.duong_dan_ai = Utilities.ConvertFileToBase64String(Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan_ai).ChuanHoaDuongDanRemote());
                        res.data_info = file;
                    }
                    else
                    {
                        file.duong_dan = Utilities.ConvertFileToBase64String(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "image_default.png"));
                        file.extension = ".jpg";
                    }
                    return Ok(res);
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
        /// <summary>
        /// Tải 1 file theo bt
        /// </summary>
        /// <returns></returns>
        [Route("get-file-bt")]
        [HttpPost]
        public async Task<IActionResult> GetFileBT()
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
            try
            {
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
                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                if (res.data_info == null)
                    return Ok(res);
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    return Ok(res);
                bh_file file = JsonConvert.DeserializeObject<bh_file>(jsonRes);
                if (!string.IsNullOrEmpty(file.duong_dan))
                {
                    string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                    if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                        fileName = @"\" + fileName;

                    file.extension = Path.GetExtension(fileName);
                    file.duong_dan = Utilities.ConvertFileToBase64String(fileName);
                    if (!string.IsNullOrEmpty(file.duong_dan_ai))
                        file.duong_dan_ai = Utilities.ConvertFileToBase64String(Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan_ai).ChuanHoaDuongDanRemote());
                    res.data_info = file;
                }
                else
                {
                    file.duong_dan = Utilities.ConvertFileToBase64String(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "image_default.png"));
                    file.extension = ".jpg";
                }
                return Ok(res);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }
        /// <summary>
        /// Tải 1 file
        /// </summary>
        /// <returns></returns>
        [Route("get-file-base")]
        [HttpPost]
        public async Task<IActionResult> GetFileBase()
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
                if (action.action_type == "FILE")
                {
                    #region Thực thi type excute db
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                    if (res.data_info == null)
                        return Ok(res);
                    string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                    if (string.IsNullOrEmpty(jsonRes))
                        return Ok(res);
                    bh_file file = JsonConvert.DeserializeObject<bh_file>(jsonRes);

                    string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                    file.extension = Path.GetExtension(fileName);
                    file.duong_dan = Utilities.ConvertFileToBase64String(fileName);
                    res.data_info = file;
                    return Ok(res);
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

        /// <summary>
        /// Tải và đóng gói danh sách file
        /// </summary>
        /// <returns></returns>
        [Route("download-zip-file")]
        [HttpPost]
        public async Task<IActionResult> DownloadZipFiles()
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
                #region Check ddos
                if (action.type_ddos == "APPLY")
                {
                    if (string.IsNullOrEmpty(header.ip_remote_ipv4) && string.IsNullOrEmpty(header.ip_remote_ipv6))
                    {
                        BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                        res.state_info.message_body = "Thiếu thông IP truy cập";
                        return Ok(res);
                    }
                    if (
                        action.max_time_ddos == null ||
                        action.max_rq_ddos == null ||
                        action.time_lock_ddos == null ||
                        action.max_time_cache <= 0 ||
                        action.max_rq_ddos <= 0 ||
                        action.time_lock_ddos <= 0
                     )
                    {
                        BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                        res.state_info.message_body = "Thông tin cấu hình DDOS chưa chính xác";
                        return Ok(res);
                    }
                    //Lấy key ddos
                    string keyCacheDDOS = CachePrefixKeyConstants.GetKeyCacheDDos(keyCachePartner, header.action, header.ip_remote_ipv4, header.ip_remote_ipv6);
                    var ddos_log = _cacheServer.Get<int?>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCacheDDOS, RedisCacheMaster.DatabaseIndex);
                    if (ddos_log < 0)
                    {
                        BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_DDOS;
                        res.state_info.message_body = "Hành động này đang tạm thời bị khóa do truy cập nhiều lần trong thời gian ngắn.";
                        return Ok(res);
                    }
                    if (ddos_log == null)
                    {
                        _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCacheDDOS, 1, DateTime.Now.AddSeconds((int)action.max_time_ddos.Value) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                    }
                    else
                    {
                        ddos_log = ddos_log + 1;
                        if (ddos_log.Value <= action.max_rq_ddos)
                        {
                            _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCacheDDOS, ddos_log, DateTime.Now.AddSeconds((int)action.max_time_ddos.Value) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                        }
                        else
                        {
                            _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCacheDDOS, -1, DateTime.Now.AddMinutes((int)action.time_lock_ddos) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                            res.state_info.status = STATUS_NOTOK;
                            res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_DDOS;
                            res.state_info.message_body = "Hành động này đang tạm thời bị khóa do truy cập nhiều lần trong thời gian ngắn.";
                            return Ok(res);
                        }
                    }
                }
                #endregion
                #region Xóa cache nếu action này thực hiện việc xóa cache
                await _dynamicService.ClearCacheActions(header, _cacheServer, action.actions_clear_cache);
                #endregion
                if (action.action_type == "FILE")
                {
                    #region Thực thi type excute db
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    if (res.data_info == null)
                    {
                        return Ok(res);
                    }
                    string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });

                    if (string.IsNullOrEmpty(jsonRes))
                    {
                        return Ok(res);
                    }

                    List<bh_file> files = JsonConvert.DeserializeObject<List<bh_file>>(jsonRes);
                    if (files == null || files.Count < 0)
                    {
                        return BadRequest("Không tìm thấy dữ liệu file");
                    }

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
                    List<string> lstPath = files.Select(n => Path.Combine(networkCredential.FullPath, n.duong_dan)).ToList();
                    byte[] arrByteFile;
                    if (!networkCredential.IsLocal)
                    {
                        NetworkCredential credentials = new NetworkCredential(Utilities.Decrypt(networkCredential.UserName), Utilities.Decrypt(networkCredential.Password));
                        using (new ConnectToSharedFolder(networkCredential.FullPath, credentials))
                        {
                            arrByteFile = Utilities.DowloadZipFile(lstPath);
                        }
                    }
                    else
                    {
                        arrByteFile = Utilities.DowloadZipFile(lstPath);
                    }
                    res.data_info = arrByteFile;
                    return Ok(res);
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

        /// <summary>
        /// Export Excel
        /// </summary>
        /// <returns></returns>
        [Route("export-excel")]
        [HttpPost]
        public async Task<IActionResult> ExportExcel()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                if (action.action_type == "FILE")
                {
                    string name = data_info["ma_doi_tac_nsd"].ToString() + data_info["ma_mau_in"].ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    string duong_dan = data_info["url_file"].ToString();

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
                    string pathFile = Path.Combine(networkCredential.FullPath, duong_dan);
                    string filename_output = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "Temp", name);
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
                            System.IO.File.Copy(pathFile, filename_output, true);
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
                        System.IO.File.Copy(pathFile, filename_output, true);
                    }
                    #region Code xuất excel
                    byte[] arrOutput = null;
                    string fileName = "export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    using (XLWorkbook workbook = new XLWorkbook(filename_output))
                    {
                        var ws = workbook.Worksheet(1);
                        try
                        {
                            await _dynamicService.ExcuteReaderAsync(data_info, header, (reader, cursor) =>
                             {
                                 int count_cursor = cursor.Count();
                                 int index = 0;
                                 do
                                 {
                                     if (index >= count_cursor)
                                         break;
                                     var cursor_name = cursor[index];
                                     var cell_current_cursor = ws.CellsUsed()
                                                                 .Where(n => n.HasFormula && n.Value.ToString().ToLower().StartsWith(cursor_name));
                                     var ds_bien = ws.CellsUsed()
                                                     .Where(n => n.HasFormula && n.Value.ToString().ToLower().StartsWith(cursor_name))
                                                     .Select(n => n.Value.ToString().ToLower().Split('.')[1]);

                                     int index_row_add = 0;
                                     while (reader.Read())
                                     {
                                         if (cursor_name.StartsWith("cur_"))
                                         {
                                             foreach (var b_bien in ds_bien)
                                             {
                                                 if (reader.HasColumn(b_bien))
                                                 {
                                                     var address_cell = cell_current_cursor.Where(n => n.Value.ToString().ToLower() == cursor_name + "." + b_bien).FirstOrDefault().Address;
                                                     ws.Cell(address_cell).SetDataType(XLDataType.Text).SetValue<string>(reader[b_bien] == null ? "" : reader[b_bien].ToString());
                                                 }
                                             }
                                         }
                                         if (cursor_name.StartsWith("curs_"))
                                         {
                                             foreach (var b_bien in ds_bien)
                                             {
                                                 var address_cell = cell_current_cursor.Where(n => n.Value.ToString().ToLower() == cursor_name + "." + b_bien).ToList();
                                                 foreach (var cell in address_cell)
                                                 {
                                                     ws.Cell(cell.Address.RowNumber + index_row_add, cell.Address.ColumnNumber).InsertCellsBelow(1).FirstOrDefault().SetDataType(XLDataType.Text).SetValue<string>(reader[b_bien] == null ? "" : reader[b_bien].ToString());
                                                 }
                                             }
                                             index_row_add++;
                                         }
                                     }
                                     foreach (var b_bien in ds_bien)
                                     {
                                         var cells = ws.CellsUsed()
                                           .Where(n => n.HasFormula && n.Value.ToString().ToLower()
                                           .StartsWith(cursor_name) && n.Value.ToString().ToLower() == cursor_name + "." + b_bien);
                                         foreach (var cell in cells)
                                         {
                                             ws.Cell(cell.Address)?.Delete(XLShiftDeletedCells.ShiftCellsUp);
                                         }
                                     }
                                     index++;
                                 }
                                 while (reader.NextResult() && index < count_cursor);
                             });
                            using (MemoryStream stream = new MemoryStream())
                            {
                                workbook.SaveAs(stream);
                                stream.Seek(0, SeekOrigin.Begin);
                                arrOutput = stream.ToArray();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    System.IO.File.Delete(filename_output);
                    return File(
                                fileContents: arrOutput,
                                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileDownloadName: fileName
                            );
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

        /// <summary>
        /// Xuất excel nguyên bảng trong Database
        /// </summary>
        /// <returns></returns>
        [Route("export-excel-table")]
        [HttpPost]
        public async Task<IActionResult> ExportExcelTable()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                if (action.action_type == "EXCUTE_DB")
                {
                    byte[] arrOutput = null;
                    string fileName = "export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        try
                        {
                            await _dynamicService.ExcuteReaderAsync(data_info, header, (reader, cursor) =>
                             {
                                 int count_cursor = cursor.Count();
                                 int index = 0;
                                 do
                                 {
                                     IXLWorksheet ws = null;
                                     var cursor_name = cursor[index];
                                     if (cursor_name.StartsWith("cur_"))
                                     {
                                         ws = workbook.Worksheets.Add(cursor_name.Substring(4));
                                     }
                                     else if (cursor_name.StartsWith("curs_"))
                                     {
                                         ws = workbook.Worksheets.Add(cursor_name.Substring(5));
                                     }
                                     else
                                     {
                                         ws = workbook.Worksheets.Add(cursor_name);
                                     }
                                     int rowindex = 1;
                                     while (reader.Read())
                                     {
                                         for (int i = 0; i < reader.FieldCount; i++)
                                         {
                                             ws.Cell(rowindex, i + 1).SetDataType(XLDataType.Text).SetValue<string>(reader[i] == null ? "" : reader[i].ToString());
                                             if (rowindex == 1)
                                             {
                                                 ws.Cell(rowindex, i + 1).Comment.AddText("field: " + reader.GetName(i).ToLower());
                                                 ws.Cell(rowindex, i + 1).Comment.AddNewLine();
                                                 ws.Cell(rowindex, i + 1).Comment.AddText("datatype: " + reader.GetDataTypeName(i).ToLower());
                                             }
                                         }
                                         rowindex++;
                                     }
                                     index++;
                                 }
                                 while (reader.NextResult());
                             });
                            using (MemoryStream stream = new MemoryStream())
                            {
                                workbook.SaveAs(stream);
                                stream.Seek(0, SeekOrigin.Begin);
                                arrOutput = stream.ToArray();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    return File(
                                fileContents: arrOutput,
                                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileDownloadName: fileName
                            );
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
        /// Tạo, lưu và ký file
        /// </summary>
        /// <returns></returns>
        [Route("create-signature-file")]
        [HttpPost]
        public async Task<IActionResult> CreateSignatureFile()
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
                if (action.action_type == "FILE")
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    DataSet ds = new DataSet();
                    ds = await _dynamicService.GetMultipleToDataSetAsync(data_info, header);
                    if (AppSettings.Environment == "Development")
                    {
                        action.is_local = 1;
                        action.ip_remote = AppSettings.PathFolderNotDelete;
                    }
                    var networkCredential = new NetworkCredentialItem()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        IpRemote = action.ip_remote,
                        BaseFolderName = action.base_folder,
                        UserName = action.user_remote,
                        Password = action.pas_remote,
                        FullPath = (action.is_local == null || action.is_local <= 0) ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                        IsLocal = (action.is_local == null || action.is_local <= 0) ? false : true
                    };
                    string ten_mau_in = data_info["ten_mau_in"].ToString();
                    byte[] arrByte = null;
                    var fileName = "export-word-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    arrByte = Utilities.ExportToPDF(ds, networkCredential.FullPath, data_info["url_file"].ToString());
                    #region ký số và lưu file
                    var ky_so = res.data_info["ky_so"];
                    string pathFilePFX = ky_so.url_pfx;
                    string fullPathPFX = Path.Combine(networkCredential.FullPath, pathFilePFX);
                    var now = DateTime.Now;
                    string fileNameOutPut = "file_ky_so_" + now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + ".pdf";
                    string pathFileOutput = Path.Combine(ky_so.ma_doi_tac.ToString(), "FILE_KY_SO", now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
                    string output = Path.Combine(networkCredential.FullPath, pathFileOutput, fileNameOutPut);
                    Utilities.CreateFolderWhenNotExist(networkCredential.FullPath, pathFileOutput, networkCredential);

                    SignatureHelper signatureHelper = new SignatureHelper(fullPathPFX, ky_so.mat_khau_pfx.ToString(), output);
                    if (!networkCredential.IsLocal)
                    {
                        NetworkCredential credentials = new NetworkCredential(Utilities.Decrypt(networkCredential.UserName), Utilities.Decrypt(networkCredential.Password));
                        using (new ConnectToSharedFolder(networkCredential.FullPath, credentials))
                        {
                            using (Stream stream = new MemoryStream(arrByte))
                            {
                                signatureHelper.SignPdfByteArray(ky_so.ly_do.ToString(), ky_so.lien_he.ToString(), ky_so.vi_tri.ToString(), stream);
                                arrByte = Utilities.FileToByteArray(output);
                            }
                        }
                    }
                    else
                    {
                        using (Stream stream = new MemoryStream(arrByte))
                        {
                            signatureHelper.SignPdfByteArray(ky_so.ly_do.ToString(), ky_so.lien_he.ToString(), ky_so.vi_tri.ToString(), stream);
                            arrByte = Utilities.FileToByteArray(output);
                        }
                    }

                    #endregion
                    #region Lưu vào bh_file
                    try
                    {
                        Task task = new Task(async () =>
                        {
                            var headerSave = header.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LUU_KY_SO;
                            IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                            dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                            dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                            dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                            dicRequest.AddWithExists("pas", data_info.GetString("pas"));
                            dicRequest.AddWithExists("so_id", data_info.GetString("so_id"));
                            dicRequest.AddWithExists("so_id_dt", data_info.GetString("so_id_dt"));
                            dicRequest.AddWithExists("pm", data_info.GetString("pm"));
                            dicRequest.AddWithExists("ma_file", data_info.GetString("ma_mau_in"));
                            dicRequest.AddWithExists("ten_file", data_info.GetString("ten_mau_in"));
                            dicRequest.AddWithExists("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                            dicRequest.AddWithExists("trang_thai", "1");
                            dicRequest.AddWithExists("x", "0");
                            dicRequest.AddWithExists("y", "0");
                            await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerSave);

                        });
                        task.Start();
                    }
                    catch
                    {

                    }

                    #endregion
                    BaseResponse<file_result> resSuccess = new BaseResponse<file_result>();
                    resSuccess.data_info = new file_result();
                    resSuccess.data_info.file = arrByte;
                    resSuccess.data_info.path = Path.Combine(pathFileOutput, fileNameOutPut);
                    resSuccess.state_info.status = STATUS_OK;
                    resSuccess.state_info.message_code = STATUS_OK;
                    resSuccess.state_info.message_body = "Ký số thành công";
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
        /// Tạo và lưu file
        /// </summary>
        /// <returns></returns>
        [Route("create-save-file")]
        [HttpPost]
        public async Task<IActionResult> CreateSaveFile()
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
                if (action.action_type == "FILE")
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    DataSet ds = new DataSet();
                    ds = await _dynamicService.GetMultipleToDataSetAsync(data_info, header);
                    if (AppSettings.Environment == "Development")
                    {
                        action.is_local = 1;
                        action.ip_remote = AppSettings.PathFolderNotDelete;
                    }
                    var networkCredential = new NetworkCredentialItem()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        IpRemote = action.ip_remote,
                        BaseFolderName = action.base_folder,
                        UserName = action.user_remote,
                        Password = action.pas_remote,
                        FullPath = (action.is_local == null || action.is_local <= 0) ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                        IsLocal = (action.is_local == null || action.is_local <= 0) ? false : true
                    };
                    string ten_mau_in = data_info["ten_mau_in"].ToString();
                    string ma_doi_tac = data_info["ma_doi_tac"].ToString();
                    byte[] arrByte = Utilities.ExportToPDF(ds, networkCredential.FullPath, data_info["url_file"].ToString());
                    var now = DateTime.Now;
                    string fileNameOutPut = "file_ky_so_" + now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + ".pdf";
                    string pathFileOutput = Path.Combine(ma_doi_tac, "FILE_KY_SO", now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
                    string output = Path.Combine(networkCredential.FullPath, pathFileOutput, fileNameOutPut);
                    Utilities.CreateFolderWhenNotExist(networkCredential.FullPath, pathFileOutput, networkCredential);
                    System.IO.File.WriteAllBytes(output, arrByte);
                    #region Lưu vào bh_file
                    try
                    {
                        Task task = new Task(async () =>
                        {
                            var headerSave = header.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LUU_KY_SO;
                            var lstFile = new List<bh_file>();
                            Dictionary<string, string> myUnderlyingObject = data_info.ToDictionary(k => k.Key, k => k.Value == null ? "" : k.Value.ToString());
                            if (!myUnderlyingObject.ContainsKey("ma_file"))
                                myUnderlyingObject.Add("ma_file", myUnderlyingObject.ContainsKey("ma_mau_in") ? myUnderlyingObject["ma_mau_in"] : "");
                            if (!myUnderlyingObject.ContainsKey("ten_file"))
                                myUnderlyingObject.Add("ten_file", string.IsNullOrEmpty(ten_mau_in) ? fileNameOutPut : ten_mau_in);
                            if (!myUnderlyingObject.ContainsKey("duong_dan"))
                                myUnderlyingObject.Add("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                            if (!myUnderlyingObject.ContainsKey("trang_thai"))
                                myUnderlyingObject.Add("trang_thai", "1");
                            else
                                myUnderlyingObject["trang_thai"] = "1";

                            if (!myUnderlyingObject.ContainsKey("x"))
                                myUnderlyingObject.Add("x", "0");
                            if (!myUnderlyingObject.ContainsKey("y"))
                                myUnderlyingObject.Add("y", "0");
                            BaseRequest rq = new BaseRequest(myUnderlyingObject);
                            await _dynamicService.ExcuteDynamicAsync(rq, headerSave, null, outValue =>
                            {

                            });
                        });
                        task.Start();
                    }
                    catch
                    {

                    }

                    #endregion
                    BaseResponse<file_result> resSuccess = new BaseResponse<file_result>();
                    resSuccess.data_info = new file_result();
                    resSuccess.data_info.file = arrByte;
                    resSuccess.data_info.path = Path.Combine(pathFileOutput, fileNameOutPut);
                    resSuccess.state_info.status = STATUS_OK;
                    resSuccess.state_info.message_code = STATUS_OK;
                    resSuccess.state_info.message_body = "Ký số thành công";
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
        /// Tạo và không lưu file
        /// </summary>
        /// <returns></returns>
        [Route("create-file")]
        [HttpPost]
        public async Task<IActionResult> CreateFile()
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
                if (action.action_type == "FILE")
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    DataSet ds = new DataSet();
                    ds = await _dynamicService.GetMultipleToDataSetAsync(data_info, header);
                    if (AppSettings.Environment == "Development")
                    {
                        action.is_local = 1;
                        action.ip_remote = AppSettings.PathFolderNotDelete;
                    }
                    var networkCredential = new NetworkCredentialItem()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        IpRemote = action.ip_remote,
                        BaseFolderName = action.base_folder,
                        UserName = action.user_remote,
                        Password = action.pas_remote,
                        FullPath = (action.is_local == null || action.is_local <= 0) ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                        IsLocal = (action.is_local == null || action.is_local <= 0) ? false : true
                    };
                    string ten_mau_in = data_info["ten_mau_in"].ToString();
                    string ma_doi_tac = data_info["ma_doi_tac"].ToString();
                    byte[] arrByte = Utilities.ExportToPDF(ds, networkCredential.FullPath, data_info["url_file"].ToString());
                    var now = DateTime.Now;
                    string fileNameOutPut = "file_ky_so_" + now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N") + ".pdf";
                    string pathFileOutput = Path.Combine(ma_doi_tac, "FILE_KY_SO", now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
                    string output = Path.Combine(networkCredential.FullPath, pathFileOutput, fileNameOutPut);
                    System.IO.File.WriteAllBytes(output, arrByte);
                    #region Lưu vào bh_file
                    try
                    {
                        Task task = new Task(async () =>
                        {
                            var headerSave = header.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_LUU_KY_SO;
                            var lstFile = new List<bh_file>();
                            Dictionary<string, string> myUnderlyingObject = data_info.ToDictionary(k => k.Key, k => k.Value == null ? "" : k.Value.ToString());
                            if (!myUnderlyingObject.ContainsKey("ma_file"))
                                myUnderlyingObject.Add("ma_file", myUnderlyingObject.ContainsKey("ma_mau_in") ? myUnderlyingObject["ma_mau_in"] : "");
                            if (!myUnderlyingObject.ContainsKey("ten_file"))
                                myUnderlyingObject.Add("ten_file", string.IsNullOrEmpty(ten_mau_in) ? fileNameOutPut : ten_mau_in);
                            if (!myUnderlyingObject.ContainsKey("duong_dan"))
                                myUnderlyingObject.Add("duong_dan", Path.Combine(pathFileOutput, fileNameOutPut));
                            if (!myUnderlyingObject.ContainsKey("trang_thai"))
                                myUnderlyingObject.Add("trang_thai", "1");
                            else
                                myUnderlyingObject["trang_thai"] = "1";

                            if (!myUnderlyingObject.ContainsKey("x"))
                                myUnderlyingObject.Add("x", "0");
                            if (!myUnderlyingObject.ContainsKey("y"))
                                myUnderlyingObject.Add("y", "0");
                            BaseRequest rq = new BaseRequest(myUnderlyingObject);
                            await _dynamicService.ExcuteDynamicAsync(rq, headerSave, null, outValue =>
                            {

                            });
                        });
                        task.Start();
                    }
                    catch
                    {

                    }

                    #endregion
                    BaseResponse<file_result> resSuccess = new BaseResponse<file_result>();
                    resSuccess.data_info = new file_result();
                    resSuccess.data_info.file = arrByte;
                    resSuccess.data_info.path = Path.Combine(pathFileOutput, fileNameOutPut);
                    resSuccess.state_info.status = STATUS_OK;
                    resSuccess.state_info.message_code = STATUS_OK;
                    resSuccess.state_info.message_body = "Ký số thành công";
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
        /// Ký file đã có trong thưc mục
        /// </summary>
        /// <returns></returns>
        [Route("signature-file")]
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> SignatureFile()
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
            if (!data_info.ContainsKey("ma_mau_in") ||
                data_info["ma_mau_in"] == null ||
                string.IsNullOrEmpty(data_info["ma_mau_in"].ToString()) ||
                !data_info.ContainsKey("ma_doi_tac") ||
                data_info["ma_doi_tac"] == null ||
                string.IsNullOrEmpty(data_info["ma_doi_tac"].ToString()) ||
                !data_info.ContainsKey("so_id") ||
                data_info["so_id"] == null ||
                string.IsNullOrEmpty(data_info["so_id"].ToString()))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Không xác định file ký";
                return Ok(res);
            }
            header.envcode = token.evncode;
            var ma_doi_tac = data_info["ma_doi_tac"].ToString();
            var so_id = Convert.ToDecimal(data_info["so_id"].ToString());
            var so_id_dt = data_info.ContainsKey("so_id_dt") ? (data_info["so_id_dt"] != null ? Convert.ToDecimal(data_info["so_id_dt"].ToString()) : 0) : 0;
            var ma_mau_in = data_info["ma_mau_in"].ToString();
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
                if (action.action_type == "FILE")
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                    ht_ky_so ky_so = null;
                    try { ky_so = JsonConvert.DeserializeObject<ht_ky_so>(JsonConvert.SerializeObject(res.data_info["ky_so"])); } catch { };
                    if (ky_so == null)
                    {
                        BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                        resError.state_info.status = STATUS_NOTOK;
                        resError.state_info.message_body = "Chưa cấu hình chữ ký số";
                        return Ok(resError);
                    }

                    if (AppSettings.Environment == "Development")
                    {
                        action.is_local = 1;
                        action.ip_remote = AppSettings.PathFolderNotDelete;
                    }
                    var networkCredential = new NetworkCredentialItem()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        IpRemote = action.ip_remote,
                        BaseFolderName = action.base_folder,
                        UserName = action.user_remote,
                        Password = action.pas_remote,
                        FullPath = (action.is_local == null || action.is_local <= 0) ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                        IsLocal = (action.is_local == null || action.is_local <= 0) ? false : true
                    };
                    List<bh_file> dsFile = new List<bh_file>();
                    try
                    {
                        dsFile = JsonConvert.DeserializeObject<List<bh_file>>(JsonConvert.SerializeObject(res.data_info["ds_file_ky"]));
                    }
                    catch { }
                    if (dsFile == null || dsFile.Count() <= 0)
                    {
                        BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                        resError.state_info.status = STATUS_NOTOK;
                        resError.state_info.message_body = "File ký chưa được khởi tạo";
                        return Ok(resError);
                    }
                    var bh_file = dsFile.Where(n => n.ma_doi_tac == ma_doi_tac && n.so_id == so_id && n.so_id_dt == so_id_dt && n.ma_file == ma_mau_in).FirstOrDefault();
                    //Lấy file ra

                    string pathFileCanKy = Path.Combine(networkCredential.FullPath, bh_file.duong_dan);
                    if (!System.IO.File.Exists(pathFileCanKy))
                    {
                        BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                        resError.state_info.status = STATUS_NOTOK;
                        resError.state_info.message_body = "Không tìm thấy đường dẫn file";
                        return Ok(resError);
                    }
                    byte[] arrByte = Utilities.FileToByteArray(pathFileCanKy);

                    #region ký số và lưu file
                    string fullPathPFX = Path.Combine(networkCredential.FullPath, ky_so.url_pfx);
                    var now = DateTime.Now;
                    SignatureHelper signatureHelper = new SignatureHelper(fullPathPFX, ky_so.mat_khau_pfx, pathFileCanKy);
                    if (!networkCredential.IsLocal)
                    {
                        NetworkCredential credentials = new NetworkCredential(Utilities.Decrypt(networkCredential.UserName), Utilities.Decrypt(networkCredential.Password));
                        using (new ConnectToSharedFolder(networkCredential.FullPath, credentials))
                        {
                            using (Stream stream = new MemoryStream(arrByte))
                            {
                                signatureHelper.SignPdfByteArray(ky_so.ly_do, ky_so.lien_he, ky_so.vi_tri, stream);
                            }
                        }
                    }
                    else
                    {
                        using (Stream stream = new MemoryStream(arrByte))
                        {
                            signatureHelper.SignPdfByteArray(ky_so.ly_do, ky_so.lien_he, ky_so.vi_tri, stream);
                        }
                    }
                    arrByte = Utilities.FileToByteArray(fullPathPFX);
                    #endregion

                    BaseResponse<file_result> resSuccess = new BaseResponse<file_result>();
                    resSuccess.data_info = new file_result();
                    resSuccess.data_info.file = arrByte;
                    resSuccess.data_info.path = bh_file.duong_dan;
                    resSuccess.state_info.status = STATUS_OK;
                    resSuccess.state_info.message_code = STATUS_OK;
                    resSuccess.state_info.message_body = "Ký số thành công";
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
        /// Ký file đã có trong thưc mục
        /// </summary>
        /// <returns></returns>
        [Route("remove-file")]
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> RemoveFile()
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
            if (!data_info.ContainsKey("ma_mau_in") ||
                data_info["ma_mau_in"] == null ||
                string.IsNullOrEmpty(data_info["ma_mau_in"].ToString()) ||
                !data_info.ContainsKey("ma_doi_tac") ||
                data_info["ma_doi_tac"] == null ||
                string.IsNullOrEmpty(data_info["ma_doi_tac"].ToString()) ||
                !data_info.ContainsKey("so_id") ||
                data_info["so_id"] == null ||
                string.IsNullOrEmpty(data_info["so_id"].ToString()))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Không xác định file ký";
                return Ok(res);
            }
            header.envcode = token.evncode;
            var ma_doi_tac = data_info["ma_doi_tac"].ToString();
            var so_id = Convert.ToDecimal(data_info["so_id"].ToString());
            var so_id_dt = data_info.ContainsKey("so_id_dt") ? (data_info["so_id_dt"] != null ? Convert.ToDecimal(data_info["so_id_dt"].ToString()) : 0) : 0;
            var ma_mau_in = data_info["ma_mau_in"].ToString();
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
                if (action.action_type == "FILE")
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                    if (AppSettings.Environment == "Development")
                    {
                        action.is_local = 1;
                        action.ip_remote = AppSettings.PathFolderNotDelete;
                    }
                    var networkCredential = new NetworkCredentialItem()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        IpRemote = action.ip_remote,
                        BaseFolderName = action.base_folder,
                        UserName = action.user_remote,
                        Password = action.pas_remote,
                        FullPath = (action.is_local == null || action.is_local <= 0) ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                        IsLocal = (action.is_local == null || action.is_local <= 0) ? false : true
                    };
                    List<bh_file> dsFile = new List<bh_file>();
                    try
                    {
                        dsFile = JsonConvert.DeserializeObject<List<bh_file>>(JsonConvert.SerializeObject(res.data_info["ds_file_ky"]));
                    }
                    catch { }
                    if (dsFile == null || dsFile.Count() <= 0)
                    {
                        BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                        resError.state_info.status = STATUS_NOTOK;
                        resError.state_info.message_body = "File ký chưa được khởi tạo";
                        return Ok(resError);
                    }
                    var bh_file = dsFile.Where(n => n.ma_doi_tac == ma_doi_tac && n.so_id == so_id && n.so_id_dt == so_id_dt && n.ma_file == ma_mau_in).FirstOrDefault();
                    //Lấy file ra
                    string pathFile = Path.Combine(networkCredential.FullPath, bh_file.duong_dan);
                    if (!System.IO.File.Exists(pathFile))
                    {
                        BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                        resError.state_info.status = STATUS_NOTOK;
                        resError.state_info.message_body = "Không tìm thấy đường dẫn file";
                        return Ok(resError);
                    }
                    System.IO.File.Delete(pathFile);
                    #region Lưu vào bh_file
                    try
                    {
                        Task task = new Task(async () =>
                        {
                            var headerSave = header.Clone();
                            headerSave.action = ESCSStoredProcedures.PHT_BH_FILE_XOA_FILE_TRINH;
                            Dictionary<string, string> myUnderlyingObject = new Dictionary<string, string>();
                            myUnderlyingObject.Add("ma_doi_tac_nsd", data_info["ma_doi_tac_nsd"].ToString());
                            myUnderlyingObject.Add("ma_chi_nhanh_nsd", data_info["ma_chi_nhanh_nsd"].ToString());
                            myUnderlyingObject.Add("nsd", data_info["nsd"].ToString());
                            myUnderlyingObject.Add("pas", data_info["pas"].ToString());
                            myUnderlyingObject.Add("ma_doi_tac", ma_doi_tac);
                            myUnderlyingObject.Add("so_id", so_id.ToString());
                            myUnderlyingObject.Add("so_id_dt", so_id_dt.ToString());
                            myUnderlyingObject.Add("ma_file", ma_mau_in);
                            BaseRequest rq = new BaseRequest(myUnderlyingObject);
                            await _dynamicService.ExcuteDynamicAsync(rq, headerSave, null);
                        });
                        task.Start();
                    }
                    catch
                    {

                    }

                    #endregion
                    BaseResponse<file_result> resSuccess = new BaseResponse<file_result>();
                    resSuccess.data_info = new file_result();
                    resSuccess.data_info.file = null;
                    resSuccess.data_info.path = bh_file.duong_dan;
                    resSuccess.state_info.status = STATUS_OK;
                    resSuccess.state_info.message_code = STATUS_OK;
                    resSuccess.state_info.message_body = "Xóa file thành công";
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
        /// Xác thực chữ ký file
        /// </summary>
        /// <returns></returns>
        [Route("verify-signature-file")]
        [HttpPost]
        public async Task<IActionResult> VerifyFile()
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
                if (action.action_type == "FILE")
                {
                    #region Thực thi type excute db
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    var outPut = new Dictionary<string, object>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                    {
                        res.out_value = outValue;
                    });
                    if (res.data_info == null)
                    {
                        return Ok(res);
                    }
                    string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });

                    if (string.IsNullOrEmpty(jsonRes))
                    {
                        return Ok(res);
                    }

                    bh_file file = JsonConvert.DeserializeObject<bh_file>(jsonRes);
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
                    if (!networkCredential.IsLocal)
                    {
                        NetworkCredential credentials = new NetworkCredential(Utilities.Decrypt(networkCredential.UserName), Utilities.Decrypt(networkCredential.Password));
                        using (new ConnectToSharedFolder(networkCredential.FullPath, credentials))
                        {
                            string fileName = Path.Combine(networkCredential.FullPath, file.duong_dan);
                            file.extension = Path.GetExtension(fileName);
                            //Ký file và lưu lại file ký

                        }
                    }
                    else
                    {
                        string fileName = Path.Combine(action.ip_remote, action.base_folder, file.duong_dan);
                        file.extension = Path.GetExtension(fileName);
                        //Ký file và lưu lại file ký


                    }
                    res.data_info = file;
                    return Ok(res);
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

        /// <summary>
        /// Export pdf
        /// </summary>
        /// <returns></returns>
        [Route("export-pdf")]
        [HttpPost]
        public async Task<IActionResult> ExportPdf()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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
                #region Lấy dữ liệu
                DataSet ds = new DataSet();
                ds = await _dynamicService.GetMultipleToDataSetAsync(data_info, header);
                if (ds == null)
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Không có dữ liệu hiển thị mẫu in.";
                    return Ok(res);
                }
                #endregion
                byte[] arrByte = null;
                var fileName = "export-word-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var url_file = data_info.GetString("url_file");
                arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, url_file);
                return File(arrByte, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }
        /// <summary>
        /// Export pdf
        /// </summary>
        /// <returns></returns>
        [Route("in-hoa-don")]
        [HttpPost]
        public async Task<IActionResult> InHoaDon()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                if (action.action_type == "FILE")
                {
                    DataSet ds = new DataSet();
                    ds = await _dynamicService.GetMultipleToDataSetAsync(data_info, header);
                    if (ds == null)
                    {
                        BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                        res.state_info.status = STATUS_NOTOK;
                        res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                        res.state_info.message_body = "Không có dữ liệu hiển thị mẫu in.";
                        return Ok(res);
                    }
                    byte[] arrByte = null;
                    var fileName = "export-word-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    arrByte = Utilities.ExportHoaDon(ds, AppSettings.PathFolderNotDeleteFull, data_info["url_file"].ToString());
                    return File(arrByte, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
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
        /// Export image pdf
        /// </summary>
        /// <returns></returns>
        [Route("export-image-pdf")]
        [HttpPost]
        public async Task<IActionResult> ExportImagePdf()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                DataSet ds = new DataSet();
                ds = await _dynamicService.GetMultipleToDataSetAsync(data_info, header);
                if (ds == null)
                {
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Không có dữ liệu hiển thị mẫu in.";
                    return Ok(res);
                }
                if (AppSettings.Environment == "Development")
                {
                    action.is_local = 1;
                    action.ip_remote = AppSettings.PathFolderNotDelete;
                }
                var networkCredential = new NetworkCredentialItem()
                {
                    Code = Guid.NewGuid().ToString("N"),
                    IpRemote = action.ip_remote,
                    BaseFolderName = action.base_folder,
                    UserName = action.user_remote,
                    Password = action.pas_remote,
                    FullPath = (action.is_local == null || action.is_local <= 0) ? @"\\" + action.ip_remote + @"\" + action.base_folder : Path.Combine(action.ip_remote, action.base_folder),
                    IsLocal = (action.is_local == null || action.is_local <= 0) ? false : true
                };
                byte[] arrByte = null;
                var fileName = "export-word-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                arrByte = Utilities.ExportToPDF(ds, networkCredential.FullPath, data_info["url_file"].ToString());
                return File(arrByte, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Export pdf
        /// </summary>
        /// <returns></returns>
        [Route("export-pdf-base64")]
        [HttpPost]
        public async Task<IActionResult> ExportPdfBase64()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                if (action.action_type == "FILE")
                {
                    DataSet ds = new DataSet();
                    ds = await _dynamicService.GetMultipleToDataSetAsync(data_info, header);
                    if (AppSettings.Environment == "Development")
                    {
                        action.is_local = 1;
                        action.ip_remote = AppSettings.PathFolderNotDelete;
                    }

                    byte[] arrByte = null;
                    var fileName = "export-word-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    arrByte = Utilities.ExportToPDF(ds, AppSettings.PathFolderNotDeleteFull, data_info["url_file"].ToString());
                    var base64String = Convert.ToBase64String(arrByte);
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    res.state_info.status = STATUS_OK;
                    res.state_info.message_code = "200";
                    res.state_info.message_body = "Thành công";
                    res.data_info = new
                    {
                        file_name = fileName,
                        base64_string = base64String
                    };
                    return Ok(res);
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
        /// Export Excel
        /// </summary>
        /// <returns></returns>
        [Route("export-excel-v2")]
        [HttpPost]
        public async Task<IActionResult> ExportExcelV2()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                #region Code xuất excel
                byte[] arrOutput = null;
                string fileName = "export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Data");

                    try
                    {
                        DataSet ds = await _dynamicService.GetDataSetAsync(data_info, header);
                        //Chèn tiêu đề
                        List<CauHinhTieuDe> lst = Utilities.ConvertDataTable<CauHinhTieuDe>(ds.Tables["cur_tieu_de"]);
                        lst = lst.OrderBy(n => n.stt_cot).ToList();
                        int cot = 1;
                        foreach (var item in lst)
                        {
                            ws.Cell(1, cot).Value = item.ten;
                            ws.Cell(1, cot).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                            ws.Cell(1, cot).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(1, cot).Comment.AddText("field: " + item.feild).AddNewLine()
                                                   .AddText("type: " + item.kieu_dl).AddNewLine()
                                                   .AddText("reqired: " + item.required).AddNewLine()
                                                   .AddText("maxlength: " + item.maxlength).AddNewLine();
                            ws.Cell(1, cot).Style.Font.SetBold();
                            cot++;
                        }
                        if (ds.Tables["curs_data"] != null && ds.Tables["curs_data"].Rows.Count > 0)
                        {
                            ws.Cell(2, 1).Value = ds.Tables["curs_data"].AsEnumerable();
                        }

                        ws.Columns().AdjustToContents();
                        using (MemoryStream stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            arrOutput = stream.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return File(
                            fileContents: arrOutput,
                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileDownloadName: fileName
                        );
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Export Báo cáo
        /// </summary>
        /// <returns></returns>
        [Route("export-bao-cao")]
        [HttpPost]
        public async Task<IActionResult> ExportBaoCao()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            openid_access_token token = null;
            header.check_ip_backlist = false;
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
                #region Lấy thông tin đường dẫn file
                var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                var ma_mau_in = data_info.GetString("ma_mau_in");
                var url_file = data_info.GetString("url_file");

                string name = ma_doi_tac_nsd + ma_mau_in + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string duong_dan = url_file;
                string pathFile = Path.Combine(AppSettings.PathFolderNotDeleteFull, duong_dan).ChuanHoaDuongDanRemote();
                if (!System.IO.File.Exists(pathFile))
                {
                    BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                    resError.state_info.status = STATUS_NOTOK;
                    resError.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    resError.state_info.message_body = "Không tồn tại file";
                    return Ok(resError);
                }
                #endregion
                #region Lấy thông tin data
                byte[] arrOutput = null;
                string fileName = "export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                DataSet ds = await _dynamicService.GetDataSetAsync(data_info, header);
                if (ds == null || ds.Tables == null || ds.Tables.Count <= 0)
                {
                    BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                    resError.state_info.status = STATUS_NOTOK;
                    resError.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    resError.state_info.message_body = "Không có dữ liệu hiển thị";
                    return Ok(resError);
                }
                #endregion
                string[] lstSpecialTemplate = new string[] { "ESCS_EXCEL_DS_CHUYEN_TIEN" };
                //Lấy ra danh sách table có tên tiền tố là cur_
                IEnumerable<string> tableSingle = ds.Tables.OfType<DataTable>().Where(n => n.TableName.StartsWith("cur_")).Select(n => n.TableName);
                IEnumerable<string> tableRepeat = ds.Tables.OfType<DataTable>().Where(n => n.TableName.StartsWith("curs_")).Select(n => n.TableName);
                if (!lstSpecialTemplate.Contains(ma_mau_in))
                {
                    using (XLWorkbook workbook = new XLWorkbook(pathFile))
                    {
                        var ws = workbook.Worksheet(1);
                        try
                        {
                            if (tableSingle != null && tableSingle.Count() > 0)
                            {
                                foreach (var cursor_name in tableSingle)
                                {
                                    var table = ds.Tables[cursor_name];
                                    var cell_current_cursor = ws.CellsUsed().Where(n => n.HasFormula && n.Value.ToString().ToLower().StartsWith(cursor_name));
                                    var ds_bien = ws.CellsUsed()
                                                    .Where(n => n.HasFormula && n.Value.ToString().ToLower().StartsWith(cursor_name))
                                                    .Select(n => n.Value.ToString().ToLower().Split('.')[1]);

                                    if (table.Rows.Count <= 0)
                                    {
                                        foreach (var b_bien in ds_bien)
                                        {
                                            var cells = ws.CellsUsed()
                                              .Where(n => n.HasFormula && n.Value.ToString().ToLower()
                                              .StartsWith(cursor_name) && n.Value.ToString().ToLower() == cursor_name + "." + b_bien);
                                            foreach (var cell in cells)
                                            {
                                                ws.Cell(cell.Address)?.Delete(XLShiftDeletedCells.ShiftCellsUp);
                                            }
                                        }
                                        continue;
                                    }
                                    foreach (var b_bien in ds_bien)
                                    {
                                        if (table.Columns.Contains(b_bien))
                                        {
                                            var address_cell = cell_current_cursor.Where(n => n.Value.ToString().ToLower() == cursor_name + "." + b_bien).FirstOrDefault().Address;
                                            ws.Cell(address_cell).SetDataType(XLDataType.Text).SetValue<string>(table.Rows[0][b_bien] == null ? "" : table.Rows[0][b_bien].ToString());
                                        }
                                    }

                                }
                            }
                            if (tableRepeat != null && tableRepeat.Count() > 0)
                            {
                                foreach (var cursor_name in tableRepeat)
                                {
                                    var cells = ws.CellsUsed()
                                              .Where(n => n.HasFormula && n.Value.ToString().ToLower()
                                              .StartsWith(cursor_name)).FirstOrDefault();
                                    if (ds.Tables[cursor_name].Rows.Count <= 0)
                                    {
                                        ws.Row(cells.Address.RowNumber).Delete();
                                        continue;
                                    }
                                    if (ds.Tables[cursor_name].Rows.Count > 2)
                                    {
                                        ws.Row(cells.Address.RowNumber).InsertRowsBelow(ds.Tables[cursor_name].Rows.Count - 1);
                                    }
                                    ws.Cell(cells.Address.RowNumber, cells.Address.ColumnNumber).InsertData(ds.Tables[cursor_name]);
                                }
                            }
                            using (MemoryStream stream = new MemoryStream())
                            {
                                workbook.SaveAs(stream);
                                stream.Seek(0, SeekOrigin.Begin);
                                arrOutput = stream.ToArray();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                }
                if (ma_mau_in == "ESCS_EXCEL_DS_CHUYEN_TIEN")
                {
                    using (XLWorkbook workbook = new XLWorkbook(pathFile))
                    {
                        var ws = workbook.Worksheet(1);
                        try
                        {
                            #region Biến đơn
                            if (tableSingle != null && tableSingle.Count() > 0)
                            {
                                foreach (var cursor_name in tableSingle)
                                {
                                    var table = ds.Tables[cursor_name];
                                    var cell_current_cursor = ws.CellsUsed().Where(n => n.HasFormula && n.Value.ToString().ToLower().StartsWith(cursor_name));
                                    var ds_bien = ws.CellsUsed()
                                                    .Where(n => n.HasFormula && n.Value.ToString().ToLower().StartsWith(cursor_name))
                                                    .Select(n => n.Value.ToString().ToLower().Split('.')[1]);

                                    if (table.Rows.Count <= 0)
                                    {
                                        foreach (var b_bien in ds_bien)
                                        {
                                            var cells = ws.CellsUsed()
                                              .Where(n => n.HasFormula && n.Value.ToString().ToLower()
                                              .StartsWith(cursor_name) && n.Value.ToString().ToLower() == cursor_name + "." + b_bien);
                                            foreach (var cell in cells)
                                            {
                                                ws.Cell(cell.Address)?.Delete(XLShiftDeletedCells.ShiftCellsUp);
                                            }
                                        }
                                        continue;
                                    }
                                    foreach (var b_bien in ds_bien)
                                    {
                                        if (table.Columns.Contains(b_bien))
                                        {
                                            var address_cell = cell_current_cursor.Where(n => n.Value.ToString().ToLower() == cursor_name + "." + b_bien).FirstOrDefault().Address;
                                            var val = table.Rows[0][b_bien];
                                            if (val == null || !val.IsNumber())
                                                ws.Cell(address_cell).SetValue(val == null ? "" : val.ToString());
                                            else
                                                ws.Cell(address_cell).SetValue(val);
                                        }
                                    }

                                }
                            }
                            #endregion
                            List<MICThanhToan> dsData = Utilities.ConvertDataTable<MICThanhToan>(ds.Tables["curs_ho_so"]);
                            int rowIndex = 5;
                            int rowThanhToan = 1;
                            if (dsData != null && dsData.Count() > 0)
                            {
                                var dsDonVi = dsData.Select(n => n.ten_cnhanh).Distinct();
                                foreach (var ten_dvi in dsDonVi)
                                {
                                    ws.Row(rowIndex).InsertRowsBelow(1);
                                    rowIndex = rowIndex + 1;
                                    var dataRow = dsData.Where(n => n.ten_cnhanh == ten_dvi).ToList();
                                    ws.Row(rowIndex).Style.Font.FontSize = 10;
                                    ws.Row(rowIndex).Style.Fill.BackgroundColor = XLColor.White;
                                    ws.Range("A" + rowIndex + ":N" + rowIndex).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A" + rowIndex + ":N" + rowIndex).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    ws.Row(rowIndex).Style.Alignment.SetWrapText(false);
                                    ws.Cell("A" + rowIndex).Value = ten_dvi;
                                    ws.Cell("A" + rowIndex).Style.Font.Bold = true;
                                    ws.Cell("A" + rowIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                    ws.Range("A" + rowIndex + ":E" + rowIndex).Merge();
                                    ws.Range("J" + rowIndex + ":Q" + rowIndex).Merge();

                                    int rowDonVi = rowIndex;
                                    double soTienChuaVat = 0;
                                    double soTienVAT = 0;
                                    double soTienTheoHoaDon = 0;
                                    double soTienThucTraKH = 0;

                                    if (dataRow != null && dataRow.Count() > 0)
                                    {
                                        foreach (var ttoan in dataRow)
                                        {
                                            ws.Row(rowIndex).InsertRowsBelow(1);
                                            rowIndex = rowIndex + 1;
                                            ws.Row(rowIndex).Unmerge();
                                            ws.Row(rowIndex).Style.Font.Bold = false;
                                            ws.Row(rowIndex).Style.Fill.BackgroundColor = XLColor.White;
                                            ws.Range("A" + rowIndex + ":Q" + rowIndex).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                            ws.Range("A" + rowIndex + ":Q" + rowIndex).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                            ws.Range("A" + rowIndex + ":Q" + rowIndex).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Range("A" + rowIndex + ":Q" + rowIndex).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Row(rowIndex).Style.Alignment.SetWrapText(false);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SOTT).SetValue(rowThanhToan);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SOTT).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_HD).SetValue(ttoan.so_hd);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_HD).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.NGAY_HD).SetValue(ttoan.ngay_hd);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.NGAY_HD).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.NGAY_HD).Style.DateFormat.Format = "dd/MM/yyyy";

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TEN).SetValue(ttoan.ten);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TEN).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_HS).SetValue(ttoan.so_hs);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_HS).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_TIEN_CHUA_VAT).SetValue(ttoan.so_tien_chua_vat);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_TIEN_CHUA_VAT).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_TIEN_CHUA_VAT).Style.NumberFormat.Format = "#,##0";
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.SO_TIEN_CHUA_VAT).DataType = XLDataType.Number;

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.VAT).SetValue(ttoan.vat);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.VAT).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.VAT).Style.NumberFormat.Format = "#,##0";
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.VAT).DataType = XLDataType.Number;

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_THEO_HD).SetValue(ttoan.tien_theo_hd);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_THEO_HD).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_THEO_HD).Style.NumberFormat.Format = "#,##0";
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_THEO_HD).DataType = XLDataType.Number;

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_KH_TRA).SetValue(ttoan.tien_kh_tra);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_KH_TRA).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_KH_TRA).Style.NumberFormat.Format = "#,##0";
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TIEN_KH_TRA).DataType = XLDataType.Number;

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TK_CMT).SetValue(ttoan.tk_cmt);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TK_CMT).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TEN_NGAN_HANG).SetValue(ttoan.ten_ngan_hang);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TEN_NGAN_HANG).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.DIEN_GIAI).SetValue(ttoan.dien_giai);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.TEN).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.DTHOAI_KH).SetValue(ttoan.dthoai_kh);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.DTHOAI_KH).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.GDV).SetValue(ttoan.gdv);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.GDV).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.MAU_HDON).SetValue(ttoan.mau_hdon);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.MAU_HDON).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.KY_HIEU_HDON).SetValue(ttoan.ky_hieu_hdon);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.KY_HIEU_HDON).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.MST).SetValue(ttoan.mst);
                                            ws.Cell(rowIndex, (int)MICThanhToanEnum.MST).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                            soTienChuaVat += (ttoan.so_tien_chua_vat == null ? 0 : ttoan.so_tien_chua_vat.Value);
                                            soTienVAT += (ttoan.vat == null ? 0 : ttoan.vat.Value);
                                            soTienTheoHoaDon += (ttoan.tien_theo_hd == null ? 0 : ttoan.tien_theo_hd.Value);
                                            soTienThucTraKH += (ttoan.tien_kh_tra == null ? 0 : ttoan.tien_kh_tra.Value);

                                            rowThanhToan = rowThanhToan + 1;
                                        }
                                    }

                                    ws.Cell("F" + rowDonVi).SetValue(soTienChuaVat);
                                    ws.Cell("F" + rowDonVi).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell("F" + rowDonVi).Style.NumberFormat.Format = "#,##0";
                                    ws.Cell("F" + rowDonVi).Style.Font.Bold = true;
                                    ws.Cell("F" + rowDonVi).DataType = XLDataType.Number;

                                    ws.Cell("G" + rowDonVi).SetValue(soTienVAT);
                                    ws.Cell("G" + rowDonVi).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell("G" + rowDonVi).Style.NumberFormat.Format = "#,##0";
                                    ws.Cell("G" + rowDonVi).Style.Font.Bold = true;
                                    ws.Cell("G" + rowDonVi).DataType = XLDataType.Number;

                                    ws.Cell("H" + rowDonVi).SetValue(soTienTheoHoaDon);
                                    ws.Cell("H" + rowDonVi).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell("H" + rowDonVi).Style.NumberFormat.Format = "#,##0";
                                    ws.Cell("H" + rowDonVi).Style.Font.Bold = true;
                                    ws.Cell("H" + rowDonVi).DataType = XLDataType.Number;

                                    ws.Cell("I" + rowDonVi).SetValue(soTienTheoHoaDon);
                                    ws.Cell("I" + rowDonVi).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell("I" + rowDonVi).Style.NumberFormat.Format = "#,##0";
                                    ws.Cell("I" + rowDonVi).Style.Font.Bold = true;
                                    ws.Cell("I" + rowDonVi).DataType = XLDataType.Number;
                                }
                            }

                            using (MemoryStream stream = new MemoryStream())
                            {
                                workbook.SaveAs(stream);
                                stream.Seek(0, SeekOrigin.Begin);
                                arrOutput = stream.ToArray();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                return File(
                            fileContents: arrOutput,
                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileDownloadName: fileName
                        );

            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
        }

        /// <summary>
        /// Xoay chiều file ảnh
        /// </summary>
        /// <returns></returns>
        [Route("rotate-image")]
        [HttpPost]
        public async Task<IActionResult> RotateImage()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data_info = Request.GetData(out var define_info, out payload);
            if (!data_info.ContainsKey("degree") && !data_info.ContainsKey("flipx") && !data_info.ContainsKey("flipy"))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                res.state_info.message_body = "Không xác định được tham số";
                return Ok(res);
            }
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
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
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
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                if (res.data_info == null)
                    return Ok(res);
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    return Ok(res);
                bh_file file = JsonConvert.DeserializeObject<bh_file>(jsonRes);
                List<string> ext = new List<string>() { ".jpg", ".jpeg", ".png" };
                string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                    fileName = @"\" + fileName;

                file.extension = Path.GetExtension(fileName);
                if (ext.Contains(file.extension.ToLower()))
                {
                    //Xoay ảnh chính
                    var degree = data_info.ContainsKey("degree") ? float.Parse(data_info["degree"].ToString()) : 0;
                    var flipx = data_info.ContainsKey("flipx") ? int.Parse(data_info["flipx"].ToString()) : 0;
                    var flipy = data_info.ContainsKey("flipy") ? int.Parse(data_info["flipy"].ToString()) : 0;
                    if (degree == 90 || degree == -90 || flipx == 1 || flipx == -1 || flipy == 1 || flipy == -1)
                    {
                        var imageByte = Utilities.FileToByteArray(fileName);
                        var image = Utilities.byteArrayToImage(imageByte);
                        image = Utilities.RotateImage(image, degree, flipx, flipy);
                        System.IO.File.WriteAllBytes(fileName, Utilities.ImageToByteArray(image));
                        //Xoay ảnh thumnail
                        var thumnail = Path.GetFileName(fileName);
                        var curentPath = Path.GetDirectoryName(fileName);
                        string fileNameThumnail = Path.Combine(curentPath, "thumnail_" + thumnail);
                        if (System.IO.File.Exists(fileNameThumnail))
                        {
                            var imageThumnailByte = Utilities.FileToByteArray(fileNameThumnail);
                            var imageThumnail = Utilities.byteArrayToImage(imageThumnailByte);
                            imageThumnail = Utilities.RotateImage(imageThumnail, degree, flipx, flipy);
                            System.IO.File.WriteAllBytes(fileNameThumnail, Utilities.ImageToByteArray(imageThumnail));
                        }
                        //Xoay ảnh AI nếu có
                        if (!string.IsNullOrEmpty(file.duong_dan_ai))
                        {
                            var fileNameAI = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan_ai).ChuanHoaDuongDanRemote();
                            if (AppSettings.FolderSharedUsed && fileNameAI.StartsWith(@"\") && !fileNameAI.StartsWith(@"\\"))
                                fileNameAI = @"\" + fileNameAI;

                            var imageByteAI = Utilities.FileToByteArray(fileNameAI);
                            var imageAI = Utilities.byteArrayToImage(imageByteAI);
                            imageAI = Utilities.RotateImage(imageAI, degree, flipx, flipy);
                            System.IO.File.WriteAllBytes(fileNameAI, Utilities.ImageToByteArray(imageAI));
                        }
                    }
                }
                file.duong_dan = Utilities.ConvertFileToBase64String(fileName);
                if (!string.IsNullOrEmpty(file.duong_dan_ai))
                {
                    var pathFileAI = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan_ai).ChuanHoaDuongDanRemote();
                    if (AppSettings.FolderSharedUsed && pathFileAI.StartsWith(@"\") && !pathFileAI.StartsWith(@"\\"))
                        pathFileAI = @"\" + pathFileAI;
                    file.duong_dan_ai = Utilities.ConvertFileToBase64String(pathFileAI);
                }
                res.data_info = file;
                return Ok(res);
                #endregion
            }
            catch (Exception exception)
            {
                #region Send notify when error
                throw exception;
                #endregion
            }
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
    public class CauHinhTieuDe
    {
        public double? stt_cot { get; set; }
        public string ten { get; set; }
        public string kieu_dl { get; set; }
        public string feild { get; set; }
        public double? maxlength { get; set; }
        public string required { get; set; }
    }
}