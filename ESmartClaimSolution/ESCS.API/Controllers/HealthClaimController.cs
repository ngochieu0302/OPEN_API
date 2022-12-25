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
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using ESCS.COMMON.SMS.MCM;
using ESCS.COMMON.QRCodeManager;
using ESCS.MODEL.HealthClaim;
using ESCS.COMMON.OCR;
using ESCS.COMMON.ESCSServices;
using ESCS.COMMON.Response.ApiGateway.PJICO;
using ESCS.COMMON.Response.ApiGateway;

/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    /// <summary>
    /// Excute stored procedure
    /// </summary>
    [Route("api/health")]
    [ApiController]
    [ESCSAuth]
    public class HealthClaimController : BaseController
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogMongoService<LogSendNotify> _logNotify;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly ILogMongoService<LogSMSFPT> _logSMSFPT;
        private readonly ILogMongoService<LogFileAction> _logFileAction;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// ServiceController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public HealthClaimController(
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
            IWebHostEnvironment webHostEnvironment)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _userConnectionManager = userConnectionManager;
            _logNotify = logNotify;
            _logSMSFPT = logSMSFPT;
            _logContent = logContent;
            _logFileAction = logFileAction;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Tìm kiếm danh sách đơn bảo hiểm
        /// </summary>
        /// <returns></returns>
        [Route("search-policy")]
        [HttpPost]
        public async Task<IActionResult> SearchPolicy()
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PBH_BT_NG_HS_TIM_NDBH)
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
            /*
             *  b_ma_doi_tac varchar2,
                b_ma_chi_nhanh varchar2,
                b_nd_tim varchar2,
                b_gcn varchar2,
                b_so_hd varchar2,
                b_ten_kh nvarchar2,
                b_lhnv nvarchar2,
                b_ngay_sinh varchar2,
                b_d_thoai varchar2,
             */
            #region Thực thi api trước khi excute db
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string ma_chi_nhanh_nsd = data_info.GetString("ma_chi_nhanh_nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();

            //Bảo hiểm quân đội MIC
            if (config != null && config.Partner == ma_doi_tac_nsd && AppSettings.ConnectApiCorePartnerHealth && config.Partner == CoreApiConfigContants.MIC)
            {
                if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    res.state_info.message_body = "Chưa thiết lập cấu hình kết nối.";
                    return Ok(res);
                };

                string nsd = data_info.GetString("nsd");
                var dataRequest = ApiGateway.RequestApiTruyVanNguoi(data_info);
                string json = await ApiGateway.CallApiTruyVanNguoi(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "NG_TIM_KIEM_GCN", JsonConvert.SerializeObject(dataRequest), json);
                try
                {
                    var response = JsonConvert.DeserializeObject<BaseResponse<mic_truy_van_gcn_con_nguoi>>(json);
                    res.state_info.status = response.state_info.status;
                    res.state_info.message_code = response.state_info.message_code;
                    res.state_info.message_body = response.state_info.message_body;
                    response.data_info.ma_doi_tac_nsd = ma_doi_tac_nsd;
                    response.data_info.ma_chi_nhanh_nsd = ma_chi_nhanh_nsd;
                    res.data_info = response.data_info == null ? null : response.data_info.ConvertData();
                    return Ok(res);
                }
                catch (Exception ex)
                {
                    throw new Exception("THÔNG BÁO: " + ex.Message);
                }
            }
            else if (config != null && config.Partner == ma_doi_tac_nsd && AppSettings.ConnectApiCorePartnerHealth
                && (config.Partner == CoreApiConfigContants.DIGINS || config.Partner == CoreApiConfigContants.TMIV))
            {
                if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    res.state_info.message_body = "Chưa thiết lập cấu hình kết nối.";
                    return Ok(res);
                };

                string nsd = data_info.GetString("nsd");
                var dataRequest = ApiGateway.RequestApiTruyVanNguoi(data_info);
                string json = await ApiGateway.CallApiTruyVanNguoi(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "NG_TIM_KIEM_GCN", JsonConvert.SerializeObject(dataRequest), json);
                try
                {
                    var response = JsonConvert.DeserializeObject<BaseResponse<digins_truy_van_gcn_con_nguoi>>(json);
                    res.state_info.status = response.state_info.status;
                    res.state_info.message_code = response.state_info.message_code;
                    res.state_info.message_body = response.state_info.message_body;
                    response.data_info.ma_doi_tac_nsd = ma_doi_tac_nsd;
                    response.data_info.ma_chi_nhanh_nsd = ma_chi_nhanh_nsd;
                    res.data_info = response.data_info == null ? null : response.data_info.ConvertData();
                    return Ok(res);
                }
                catch (Exception ex)
                {
                    throw new Exception("THÔNG BÁO: " + ex.Message);
                }
            }
            else if (config != null && config.Partner == ma_doi_tac_nsd && AppSettings.ConnectApiCorePartnerHealth && (config.Partner == CoreApiConfigContants.PJICO))
            {
                if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    res.state_info.message_body = "Chưa thiết lập cấu hình kết nối.";
                    return Ok(res);
                };

                string nsd = data_info.GetString("nsd");
                var dataRequest = ApiGatewayPJICO.RequestApiTruyVanNguoi(data_info);
                string json = await ApiGatewayPJICO.CallApiTruyVanNguoi(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "NG_TIM_KIEM_GCN", JsonConvert.SerializeObject(dataRequest), json);
                try
                {
                    var dataPjico = JsonConvert.DeserializeObject<PJICOResponse<List<GiayChungNhanPjico>>>(json);
                    if (dataPjico.returnCode != 0)
                        throw new Exception("THÔNG BÁO: " + dataPjico.errMess + "(" + dataPjico.returnCode + ")");
                    //Mapping lại dữ liệu ở đây
                    BaseResponse<DSGiayChungNhanESCS> dataESCS = new BaseResponse<DSGiayChungNhanESCS>();
                    dataESCS.data_info = new DSGiayChungNhanESCS();
                    dataESCS.data_info.hd_nguoi = new List<GiayChungNhanNguoiESCS>();
                    if (dataPjico.data == null || dataPjico.data.Count() <= 0)
                        return Ok(dataESCS);
                    dataESCS.data_info.hd_nguoi = ESCSConverter.ConvertGCNNguoiPjicoToESCS(dataPjico.data);
                    return Ok(dataESCS);
                }
                catch (Exception ex)
                {
                    throw new Exception("THÔNG BÁO: " + ex.Message);
                }
            }
            else
            {
                var outPut = new Dictionary<string, object>();
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                {
                    res.out_value = outValue;
                });
                return Ok(res);
            }
            #endregion
        }
        /// <summary>
        /// Lấy thông tin đơn bảo hiểm
        /// </summary>
        /// <returns></returns>
        [Route("get-policy")]
        [HttpPost]
        public async Task<IActionResult> GetPolicy()
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PTICH_HOP_HOP_DONG_CON_NGUOI_MIC)
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
            if (!AppSettings.ConnectApiCorePartnerHealth)
                return Ok(res);
            #region Thực thi api trước khi excute db
            string pas_tmp = data_info.GetString("pas");
            bool sdbs = true;
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string ma_chi_nhanh = data_info.GetString("ma_chi_nhanh");
            string so_id_hd = data_info.GetString("so_id_hd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            do
            {
                var dataRequest = ApiGateway.RequestApiThongTinGCNNguoi(data_info);
                string json = await ApiGateway.CallApiThongTinGCNNguoi(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "NG_XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);

                if (config != null && config.Partner == ma_doi_tac_nsd && AppSettings.ConnectApiCorePartnerHealth && config.Partner == CoreApiConfigContants.MIC)
                {
                    BaseResponse<mic_xem_gcn> response = new BaseResponse<mic_xem_gcn>();
                    response = JsonConvert.DeserializeObject<BaseResponse<mic_xem_gcn>>(json);
                    if (response.state_info.status != STATUS_OK)
                        return Ok(response);
                    //Import hợp đồng
                    if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                    {
                        //response.state_info.status = STATUS_NOTOK;
                        //response.state_info.message_body = "Không tìm thấy hợp đồng/gcn trên core";
                        //return Ok(response);

                        //Vẫn có trường hợp hợp đồng gốc không có đối tượng vẫn duyệt được
                        //Hợp đồng gốc chỉ là hợp đồng nguyên tắc, sau đó làm sửa đổi bổ sung để thêm đối tượng
                        break;
                    }
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_CON_NGUOI_MIC;
                    await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                    var hd = response.data_info.hd.FirstOrDefault();
                    sdbs = hd.so_id_hdong != hd.so_id_hdong_dau;
                    data_info.AddWithExists("so_id_hd", hd.so_id_hdong_dau);
                }
                else if (config != null && config.Partner == ma_doi_tac_nsd && AppSettings.ConnectApiCorePartnerHealth
                    && (config.Partner == CoreApiConfigContants.DIGINS || config.Partner == CoreApiConfigContants.TMIV))
                {
                    BaseResponse<digins_xem_gcn> response = new BaseResponse<digins_xem_gcn>();
                    response = JsonConvert.DeserializeObject<BaseResponse<digins_xem_gcn>>(json);
                    if (response.state_info.status != STATUS_OK)
                        return Ok(response);
                    //Import hợp đồng
                    if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                    {
                        //response.state_info.status = STATUS_NOTOK;
                        //response.state_info.message_body = "Không tìm thấy hợp đồng/gcn trên core";
                        //return Ok(response);

                        //Vẫn có trường hợp hợp đồng gốc không có đối tượng vẫn duyệt được
                        //Hợp đồng gốc chỉ là hợp đồng nguyên tắc, sau đó làm sửa đổi bổ sung để thêm đối tượng
                        break;
                    }
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_CON_NGUOI_DIGINS;
                    await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                    var hd = response.data_info.hd.FirstOrDefault();
                    sdbs = hd.so_id_hdong != hd.so_id_hdong_dau;
                    data_info.AddWithExists("so_id_hd", hd.so_id_hdong_dau);
                }
            } while (sdbs);
            try
            {
                if (config != null && config.Partner == ma_doi_tac_nsd && AppSettings.ConnectApiCorePartnerHealth && config.Partner == CoreApiConfigContants.MIC)
                {
                    data_info.AddWithExists("ma_chi_nhanh_ql", ma_chi_nhanh);
                    data_info.AddWithExists("so_id_hd", so_id_hd);
                    var dataRequestTTTTPhi = ApiGateway.RequestApiThanhToanPhi(data_info);
                    string json = await ApiGateway.CallApiThanhToanPhi(ma_doi_tac_nsd, dataRequestTTTTPhi);
                    AddLog(header, ma_doi_tac_nsd, nsd, "THANH_TOAN_PHI", JsonConvert.SerializeObject(dataRequestTTTTPhi), json);
                    BaseResponse<KQTinhTrangThanhToanPhi> response = new BaseResponse<KQTinhTrangThanhToanPhi>();
                    response = JsonConvert.DeserializeObject<BaseResponse<KQTinhTrangThanhToanPhi>>(json);
                    if (response.state_info.status == STATUS_OK)
                    {
                        IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                        dicRequest = json.ConvertStringJsonToDictionary();
                        dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                        dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                        dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                        dicRequest.AddWithExists("pas", pas_tmp);
                        dicRequest.AddWithExists("so_id", data_info.GetString("so_id_hd"));
                        dicRequest.AddWithExists("nv", "NG");
                        var headerGateway = header.Clone();
                        headerGateway.action = ESCSStoredProcedures.PBH_HD_KY_THANH_TOAN_MIC_NH;
                        try { await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway); } catch { };
                    }
                }
            }
            catch
            {

            }
            return Ok(res);
            #endregion
        }
        ///// <summary>
        ///// Lấy thông tin đơn bảo hiểm
        ///// </summary>
        ///// <returns></returns>
        [Route("get-old-policy")]
        [HttpPost]
        public async Task<IActionResult> GetOldPolicy()
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PTICH_HOP_HOP_DONG_CON_NGUOI_CU_MIC)
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
            if (!AppSettings.ConnectApiCorePartnerHealth)
                return Ok(res);
            #region Thực thi api trước khi excute db
            string pas_tmp = data_info.GetString("pas");
            bool sdbs = true;
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string ma_chi_nhanh = data_info.GetString("ma_chi_nhanh");
            string so_id_hd = data_info.GetString("so_id_hd");
            string nsd = data_info.GetString("nsd");
            do
            {
                var dataRequest = ApiGateway.RequestApiThongTinGCNNguoi(data_info);
                string json = await ApiGateway.CallApiThongTinGCNNguoi(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "NG_XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);

                BaseResponse<mic_xem_gcn> response = new BaseResponse<mic_xem_gcn>();
                response = JsonConvert.DeserializeObject<BaseResponse<mic_xem_gcn>>(json);
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
                //Import hợp đồng
                if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                {
                    break;
                }

                IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                dicRequest = json.ConvertStringJsonToDictionary();
                dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                dicRequest.AddWithExists("pas", pas_tmp);
                // gcn
                dicRequest.AddWithExists("gcn_ten_ndbh", data_info.GetString("ten_ndbh"));
                dicRequest.AddWithExists("gcn_ngay_sinh", data_info.GetString("ngay_sinh"));
                dicRequest.AddWithExists("gcn_gioi_tinh", data_info.GetString("gioi_tinh"));
                dicRequest.AddWithExists("gcn_ctrinh", data_info.GetString("san_pham"));
                dicRequest.AddWithExists("gcn_so_id_goi", data_info.GetString("goi_bh"));
                dicRequest.AddWithExists("gcn_nhom", data_info.GetString("nhom"));
                // quyen loi
                dicRequest.AddWithExists("dk_nghiep_vu", JArray.Parse(data_info.GetString("arr_lh_nv")));
                dicRequest.AddWithExists("dk_nghiep_vu_ql", JArray.Parse(data_info.GetString("arr_lh_nv_ct")));
                dicRequest.AddWithExists("dk_ten_ql", JArray.Parse(data_info.GetString("arr_ten")));
                dicRequest.AddWithExists("dk_gioi_han_so_ngay", JArray.Parse(data_info.GetString("arr_so_lan_ngay")));
                dicRequest.AddWithExists("dk_gioi_han_tien_ngay", JArray.Parse(data_info.GetString("arr_tien_lan_ngay")));
                dicRequest.AddWithExists("dk_muc_tn", JArray.Parse(data_info.GetString("arr_tien_nam")));
                dicRequest.AddWithExists("dk_ty_le_dong", JArray.Parse(data_info.GetString("arr_dong_bh")));
                dicRequest.AddWithExists("dk_tgian_cho", JArray.Parse(data_info.GetString("arr_so_ngay_cho")));
                dicRequest.AddWithExists("dk_phi", JArray.Parse(data_info.GetString("arr_phi")));
                dicRequest.AddWithExists("dk_so_ngay_sd", JArray.Parse(data_info.GetString("arr_so_lan_ngay_duyet")));
                dicRequest.AddWithExists("dk_tien_sd", JArray.Parse(data_info.GetString("arr_tien_nam_duyet")));
                dicRequest.AddWithExists("dk_quyen_loi", JArray.Parse(data_info.GetString("arr_quyen_loi")));
                dicRequest.AddWithExists("dk_loai", JArray.Parse(data_info.GetString("arr_loai")));
                dicRequest.AddWithExists("dk_loaiq", JArray.Parse(data_info.GetString("arr_loaiq")));

                var headerGateway = header.Clone();
                headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_CON_NGUOI_CU_MIC;
                await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                var hd = response.data_info.hd.FirstOrDefault();
                sdbs = hd.so_id_hdong != hd.so_id_hdong_dau;
                data_info.AddWithExists("so_id_hd", hd.so_id_hdong_dau);
            } while (sdbs);
            try
            {
                data_info.AddWithExists("ma_chi_nhanh_ql", ma_chi_nhanh);
                data_info.AddWithExists("so_id_hd", so_id_hd);
                var dataRequestTTTTPhi = ApiGateway.RequestApiThanhToanPhi(data_info);
                string json = await ApiGateway.CallApiThanhToanPhi(ma_doi_tac_nsd, dataRequestTTTTPhi);
                AddLog(header, ma_doi_tac_nsd, nsd, "THANH_TOAN_PHI", JsonConvert.SerializeObject(dataRequestTTTTPhi), json);
                BaseResponse<KQTinhTrangThanhToanPhi> response = new BaseResponse<KQTinhTrangThanhToanPhi>();
                response = JsonConvert.DeserializeObject<BaseResponse<KQTinhTrangThanhToanPhi>>(json);
                if (response.state_info.status == STATUS_OK)
                {
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    dicRequest.AddWithExists("so_id", data_info.GetString("so_id_hd"));
                    dicRequest.AddWithExists("nv", "NG");
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PBH_HD_KY_THANH_TOAN_MIC_NH;
                    try { await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway); } catch { };
                }
            }
            catch
            {

            }
            return Ok(res);
            #endregion
        }
        /// <summary>
        /// Đẩy dữ liệu bồi thường
        /// </summary>
        /// <returns></returns>
        [Route("insurance")]
        [HttpPost]
        public async Task<IActionResult> Import()
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_CON_NGUOI_MIC)
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
            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                resData.out_value = outValue;
            });
            var jsonData = JsonConvert.SerializeObject(resData.data_info);
            DuLieuBoiThuongMIC duLieuBoiThuong = JsonConvert.DeserializeObject<DuLieuBoiThuongMIC>(jsonData);
            if (duLieuBoiThuong == null || duLieuBoiThuong.hs == null || string.IsNullOrEmpty(duLieuBoiThuong.hs.loai_hop_dong))
            {
                duLieuBoiThuong = new DuLieuBoiThuongMIC();
                duLieuBoiThuong.hs = new HoSoMIC();
                duLieuBoiThuong.hs.loai_hop_dong = "M";
            }
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            string hanh_dong = data_info.GetString("hanh_dong");
            data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));
            BaseResponse<dynamic> response = new BaseResponse<dynamic>();

            string so_hs_out = "";
            if (duLieuBoiThuong.hs.loai_hop_dong == "M")
            {
                DuLieuBoiThuongConNguoi duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoi>(jsonData);
                if (duLieuBT != null && duLieuBT.hs != null)
                {
                    duLieuBT.hs.so_cv_kn = (duLieuBT.hs.so_cv_kn == null || string.IsNullOrEmpty(duLieuBT.hs.so_cv_kn.Trim())) ? "" : duLieuBT.hs.so_cv_kn.Trim();
                    duLieuBT.hs.so_hs = (duLieuBT.hs.so_hs == null || string.IsNullOrEmpty(duLieuBT.hs.so_hs.Trim())) ? "" : duLieuBT.hs.so_hs.Trim();
                }

                string json = await ApiGateway.CallApiChuyenDLBoiThuongConNguoi(ma_doi_tac_nsd, duLieuBT);
                AddLog(header, ma_doi_tac_nsd, nsd, "NG_CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);

                DuLieuBoiThuongConNguoiResponse resDataApi = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoiResponse>(json);

                if (resDataApi.status != STATUS_OK)
                {
                    response.state_info.status = STATUS_NOTOK;
                    response.state_info.message_body = "[THÔNG BÁO TỪ CORE] - " + resDataApi.message_body;
                    response.state_info.message_code = resDataApi.message_code;
                    return Ok(response);
                }
                if (resDataApi.out_value == null || string.IsNullOrEmpty(resDataApi.out_value.so_ho_so))
                {
                    response.state_info.status = STATUS_NOTOK;
                    response.state_info.message_body = "Hệ thống core không trả về số hồ sơ";
                    response.state_info.message_code = resDataApi.message_code;
                    return Ok(response);
                }

                so_hs_out = resDataApi.out_value.so_ho_so;
            }
            if (duLieuBoiThuong.hs.loai_hop_dong == "C")
            {
                DuLieuBoiThuongConNguoi_CU duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoi_CU>(jsonData);
                if (duLieuBT != null && duLieuBT.hs != null)
                {
                    duLieuBT.hs.so_cv_kn = (duLieuBT.hs.so_cv_kn == null || string.IsNullOrEmpty(duLieuBT.hs.so_cv_kn.Trim())) ? "" : duLieuBT.hs.so_cv_kn.Trim();
                    duLieuBT.hs.so_hs = (duLieuBT.hs.so_hs == null || string.IsNullOrEmpty(duLieuBT.hs.so_hs.Trim())) ? "" : duLieuBT.hs.so_hs.Trim();
                }
                DuLieuBoiThuongConNguoi_CU_TICH_HOP tichHop = duLieuBT.GetDuLieuTichHop();
                if (tichHop.nv == null || tichHop.nv.Count() <= 0)
                    tichHop.nv = null;
                if (tichHop.grv == null || tichHop.grv.Count() <= 0)
                    tichHop.grv = null;
                if (tichHop.vph == null || tichHop.vph.Count() <= 0)
                    tichHop.vph = null;
                if (tichHop.lsb == null || tichHop.lsb.Count() <= 0)
                    tichHop.lsb = null;
                if (tichHop.bkhac == null || tichHop.bkhac.Count() <= 0)
                    tichHop.bkhac = null;

                string json = await ApiGateway.CallApiChuyenDLBoiThuongConNguoiCu(ma_doi_tac_nsd, tichHop);
                AddLog(header, ma_doi_tac_nsd, nsd, "NG_CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(tichHop), json);

                var resTichHop = JsonConvert.DeserializeObject<BaseResponse<dynamic, DuLieuBoiThuongConNguoiSoHS>>(json);
                if (resTichHop.state_info.status != STATUS_OK)
                    return Ok(resTichHop);

                so_hs_out = resTichHop.out_value.so_ho_so;
            }


            if (!string.IsNullOrEmpty(so_hs_out) && hanh_dong == "LAY_SO_HS")
            {
                var headerClone = header.Clone();
                headerClone.action = ESCSStoredProcedures.PBH_BT_NG_CAP_NHAT_SO_HS;
                IDictionary<string, object> data_upadte = new Dictionary<string, object>();
                data_upadte.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                data_upadte.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                data_upadte.AddWithExists("nsd", data_info.GetString("nsd"));
                data_upadte.AddWithExists("pas", data_info.GetString("pas"));
                data_upadte.AddWithExists("ma_doi_tac", data_info.GetString("ma_doi_tac_nsd"));
                data_upadte.AddWithExists("so_id", data_info.GetString("so_id"));
                data_upadte.AddWithExists("so_hs", so_hs_out);
                response.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_upadte, headerClone);
                response.out_value = new { so_hs = so_hs_out };
                return Ok(response);
            }
            return Ok(response);
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
            if (header.action != ESCSStoredProcedures.PBH_BT_NG_HS_DONG_HS)
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
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { resData.out_value = outValue; });
            var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();

            if (AppSettings.ConnectApiCorePartnerHealth && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                var headerTichHop = header.Clone();
                headerTichHop.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_CON_NGUOI_MIC;
                data_info.AddWithExists("ma_doi_tac", data_info.GetString("ma_doi_tac_nsd"));
                data_info.AddWithExists("hanh_dong", "CHUYEN_DU_LIEU_BT");

                BaseResponse<dynamic> resDataTichHop = new BaseResponse<dynamic>();
                try
                {
                    resDataTichHop.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop, outValue =>
                    {
                        resDataTichHop.out_value = outValue;
                    });
                }
                catch (Exception ex)
                {
                    headerTichHop.action = ESCSStoredProcedures.PBH_BT_NG_HS_HUY_DONG_HS;
                    await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop);
                    throw ex;
                }

                var jsonData = JsonConvert.SerializeObject(resDataTichHop.data_info);
                DuLieuBoiThuongMIC duLieuBoiThuong = JsonConvert.DeserializeObject<DuLieuBoiThuongMIC>(jsonData);
                if (duLieuBoiThuong == null || duLieuBoiThuong.hs == null || string.IsNullOrEmpty(duLieuBoiThuong.hs.loai_hop_dong))
                {
                    duLieuBoiThuong = new DuLieuBoiThuongMIC();
                    duLieuBoiThuong.hs = new HoSoMIC();
                    duLieuBoiThuong.hs.loai_hop_dong = "M";
                }
                string hanh_dong = data_info.GetString("hanh_dong");
                data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));
                BaseResponse<dynamic> response = new BaseResponse<dynamic>();

                if (duLieuBoiThuong.hs.loai_hop_dong == "M")
                {
                    DuLieuBoiThuongConNguoi duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoi>(jsonData);
                    if (duLieuBT != null && duLieuBT.hs != null)
                    {
                        duLieuBT.hs.so_cv_kn = (duLieuBT.hs.so_cv_kn == null || string.IsNullOrEmpty(duLieuBT.hs.so_cv_kn.Trim())) ? "" : duLieuBT.hs.so_cv_kn.Trim();
                        duLieuBT.hs.so_hs = (duLieuBT.hs.so_hs == null || string.IsNullOrEmpty(duLieuBT.hs.so_hs.Trim())) ? "" : duLieuBT.hs.so_hs.Trim();
                    }

                    string json = await ApiGateway.CallApiChuyenDLBoiThuongConNguoi(ma_doi_tac_nsd, duLieuBT);
                    AddLog(headerTichHop, ma_doi_tac_nsd, nsd, "NG_CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                    DuLieuBoiThuongConNguoiResponse resDataApi = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoiResponse>(json);

                    if (resDataApi.status != STATUS_OK)
                    {
                        headerTichHop.action = ESCSStoredProcedures.PBH_BT_NG_HS_HUY_DONG_HS;
                        await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop);

                        response.state_info.status = STATUS_NOTOK;
                        response.state_info.message_body = "[THÔNG BÁO TỪ CORE] - " + resDataApi.message_body;
                        response.state_info.message_code = resDataApi.message_code;
                        return Ok(response);
                    }
                }
                if (duLieuBoiThuong.hs.loai_hop_dong == "C")
                {
                    DuLieuBoiThuongConNguoi_CU duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoi_CU>(jsonData);
                    if (duLieuBT != null && duLieuBT.hs != null)
                    {
                        duLieuBT.hs.so_cv_kn = (duLieuBT.hs.so_cv_kn == null || string.IsNullOrEmpty(duLieuBT.hs.so_cv_kn.Trim())) ? "" : duLieuBT.hs.so_cv_kn.Trim();
                        duLieuBT.hs.so_hs = (duLieuBT.hs.so_hs == null || string.IsNullOrEmpty(duLieuBT.hs.so_hs.Trim())) ? "" : duLieuBT.hs.so_hs.Trim();
                    }
                    DuLieuBoiThuongConNguoi_CU_TICH_HOP tichHop = duLieuBT.GetDuLieuTichHop();
                    if (tichHop.nv == null || tichHop.nv.Count() <= 0)
                        tichHop.nv = null;
                    if (tichHop.grv == null || tichHop.grv.Count() <= 0)
                        tichHop.grv = null;
                    if (tichHop.vph == null || tichHop.vph.Count() <= 0)
                        tichHop.vph = null;
                    if (tichHop.lsb == null || tichHop.lsb.Count() <= 0)
                        tichHop.lsb = null;
                    if (tichHop.bkhac == null || tichHop.bkhac.Count() <= 0)
                        tichHop.bkhac = null;

                    string json = await ApiGateway.CallApiChuyenDLBoiThuongConNguoiCu(ma_doi_tac_nsd, tichHop);
                    AddLog(headerTichHop, ma_doi_tac_nsd, nsd, "NG_CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(tichHop), json);
                    var resTichHop = JsonConvert.DeserializeObject<BaseResponse<dynamic, DuLieuBoiThuongConNguoiSoHS>>(json);

                    if (resTichHop.state_info.status != STATUS_OK)
                    {
                        headerTichHop.action = ESCSStoredProcedures.PBH_BT_NG_HS_HUY_DONG_HS;
                        await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop);

                        resTichHop.state_info.message_body = "[THÔNG BÁO TỪ CORE] - " + resTichHop.state_info.message_body;
                        return Ok(resTichHop);
                    }
                }
            }

            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Mở lại hồ sơ
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
            if (header.action != ESCSStoredProcedures.PBH_BT_NG_HS_HUY_DONG_HS)
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
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { resData.out_value = outValue; });
            var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();

            if (AppSettings.ConnectApiCorePartnerHealth && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                var headerTichHop = header.Clone();
                headerTichHop.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_CON_NGUOI_MIC;
                data_info.AddWithExists("ma_doi_tac", data_info.GetString("ma_doi_tac_nsd"));
                data_info.AddWithExists("hanh_dong", "CHUYEN_DU_LIEU_BT");

                BaseResponse<dynamic> resDataTichHop = new BaseResponse<dynamic>();
                try
                {
                    resDataTichHop.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop, outValue =>
                    {
                        resDataTichHop.out_value = outValue;
                    });
                }
                catch (Exception ex)
                {
                    headerTichHop.action = ESCSStoredProcedures.PBH_BT_NG_HS_DONG_HS;
                    await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop);
                    throw ex;
                }
                var jsonData = JsonConvert.SerializeObject(resDataTichHop.data_info);
                DuLieuBoiThuongMIC duLieuBoiThuong = JsonConvert.DeserializeObject<DuLieuBoiThuongMIC>(jsonData);
                if (duLieuBoiThuong == null || duLieuBoiThuong.hs == null || string.IsNullOrEmpty(duLieuBoiThuong.hs.loai_hop_dong))
                {
                    duLieuBoiThuong = new DuLieuBoiThuongMIC();
                    duLieuBoiThuong.hs = new HoSoMIC();
                    duLieuBoiThuong.hs.loai_hop_dong = "M";
                }
                string hanh_dong = data_info.GetString("hanh_dong");
                data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));
                BaseResponse<dynamic> response = new BaseResponse<dynamic>();

                if (duLieuBoiThuong.hs.loai_hop_dong == "M")
                {
                    DuLieuBoiThuongConNguoi duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoi>(jsonData);
                    if (duLieuBT != null && duLieuBT.hs != null)
                    {
                        duLieuBT.hs.so_cv_kn = (duLieuBT.hs.so_cv_kn == null || string.IsNullOrEmpty(duLieuBT.hs.so_cv_kn.Trim())) ? "" : duLieuBT.hs.so_cv_kn.Trim();
                        duLieuBT.hs.so_hs = (duLieuBT.hs.so_hs == null || string.IsNullOrEmpty(duLieuBT.hs.so_hs.Trim())) ? "" : duLieuBT.hs.so_hs.Trim();
                    }

                    string json = await ApiGateway.CallApiChuyenDLBoiThuongConNguoi(ma_doi_tac_nsd, duLieuBT);
                    AddLog(headerTichHop, ma_doi_tac_nsd, nsd, "NG_CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                    DuLieuBoiThuongConNguoiResponse resDataApi = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoiResponse>(json);

                    if (resDataApi.status != STATUS_OK)
                    {
                        headerTichHop.action = ESCSStoredProcedures.PBH_BT_NG_HS_DONG_HS;
                        await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop);

                        response.state_info.status = STATUS_NOTOK;
                        response.state_info.message_body = "[THÔNG BÁO TỪ CORE] - " + resDataApi.message_body;
                        response.state_info.message_code = resDataApi.message_code;
                        return Ok(response);
                    }
                }
                if (duLieuBoiThuong.hs.loai_hop_dong == "C")
                {
                    DuLieuBoiThuongConNguoi_CU duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongConNguoi_CU>(jsonData);
                    if (duLieuBT != null && duLieuBT.hs != null)
                    {
                        duLieuBT.hs.so_cv_kn = (duLieuBT.hs.so_cv_kn == null || string.IsNullOrEmpty(duLieuBT.hs.so_cv_kn.Trim())) ? "" : duLieuBT.hs.so_cv_kn.Trim();
                        duLieuBT.hs.so_hs = (duLieuBT.hs.so_hs == null || string.IsNullOrEmpty(duLieuBT.hs.so_hs.Trim())) ? "" : duLieuBT.hs.so_hs.Trim();
                    }
                    DuLieuBoiThuongConNguoi_CU_TICH_HOP tichHop = duLieuBT.GetDuLieuTichHop();
                    if (tichHop.nv == null || tichHop.nv.Count() <= 0)
                        tichHop.nv = null;
                    if (tichHop.grv == null || tichHop.grv.Count() <= 0)
                        tichHop.grv = null;
                    if (tichHop.vph == null || tichHop.vph.Count() <= 0)
                        tichHop.vph = null;
                    if (tichHop.lsb == null || tichHop.lsb.Count() <= 0)
                        tichHop.lsb = null;
                    if (tichHop.bkhac == null || tichHop.bkhac.Count() <= 0)
                        tichHop.bkhac = null;

                    string json = await ApiGateway.CallApiChuyenDLBoiThuongConNguoiCu(ma_doi_tac_nsd, tichHop);
                    AddLog(headerTichHop, ma_doi_tac_nsd, nsd, "NG_CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(tichHop), json);
                    var resTichHop = JsonConvert.DeserializeObject<BaseResponse<dynamic, DuLieuBoiThuongConNguoiSoHS>>(json);

                    if (resTichHop.state_info.status != STATUS_OK)
                    {
                        headerTichHop.action = ESCSStoredProcedures.PBH_BT_NG_HS_DONG_HS;
                        await _dynamicService.ExcuteDynamicNewAsync(data_info, headerTichHop);

                        resTichHop.state_info.message_body = "[THÔNG BÁO TỪ CORE] - " + resTichHop.state_info.message_body;
                        return Ok(resTichHop);
                    }
                }
            }

            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(resData);
            #endregion
        }
        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("create")]
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
            if (action.actionapicode == ESCSStoredProcedures.PBH_BT_NG_HS_BAO_LANH_NH ||
                action.actionapicode == ESCSStoredProcedures.PBH_BT_NG_HS_TIEP_NHAN_NH ||
                action.actionapicode == ESCSStoredProcedures.PMOBILE_KH_BT_NG_HS_TIEP_NHAN_NH ||
                action.actionapicode == ESCSStoredProcedures.P_MOBILE_BH_BT_NG_HS_BAO_LANH_NH)
            {
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; outPut = outValue; });
                try
                {
                    var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                    var so_id = outPut.GetString("so_id");
                    if (!string.IsNullOrEmpty(ma_doi_tac_nsd) && !string.IsNullOrEmpty(so_id))
                    {
                        // sử dụng ở web
                        var filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE");
                        if (!Directory.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote()))
                            Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());

                        filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE", "NG" + ma_doi_tac_nsd + so_id + ".png");
                        if (!System.IO.File.Exists(Path.Combine(ma_doi_tac_nsd, filePath)))
                        {
                            var textQRCode = Utilities.EncryptByKey("ma_doi_tac=" + ma_doi_tac_nsd + "&nv=NG&so_id=" + so_id + "&loai=HOSO", AppSettings.KeyEryptData);
                            QRCodeUtils.GenerateQRCode(textQRCode, Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());
                            var headerClone = header.Clone();
                            headerClone.action = ESCSStoredProcedures.PBH_BT_HS_QRCODE_NH;
                            var data = new Dictionary<string, object>();
                            data.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                            data.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                            data.AddWithExists("nsd", data_info.GetString("nsd"));
                            data.AddWithExists("pas", data_info.GetString("pas"));
                            data.AddWithExists("so_id", so_id);
                            data.AddWithExists("nv", "NG");
                            data.AddWithExists("loai", "HOSO");
                            data.AddWithExists("nguon", "");
                            data.AddWithExists("url_file", filePath);
                            await _dynamicService.ExcuteDynamicNewAsync(data, headerClone);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                SendNotify(action, header, data_info);
                SendSMS(action, header, data_info);
                return Ok(res);
            }

            res.state_info.status = STATUS_NOTOK;
            res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
            res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
            return Ok(res);
            #endregion
        }
        /// /// <summary>
        /// Đọc OCR giấy tờ con người
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
                try
                {
                    var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                    var type = string.Empty;
                    #region Kiểm tra cache danh mục bộ mã map hạng mục AI, nếu chưa có thì lấy cache ra
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
                    #region Lấy danh sách lịch đọc OCR
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
                            if (System.IO.File.Exists(pathFileOCR) && pathFileOCR.Contains(item.ma_doi_tac) && !string.IsNullOrEmpty(item.hang_muc))
                            {
                                List<ht_dich_vu_ocr> arrHangMucOCR = JsonConvert.DeserializeObject<List<ht_dich_vu_ocr>>(jsonHangMucOCR);
                                Dictionary<string, bool> prefixOCR = new Dictionary<string, bool>();
                                HeaderRequest requestSave = header.Clone();
                                try
                                {
                                    requestSave.action = ESCSStoredProcedures.PHT_OCR_GIAY_TO_NH;
                                    Dictionary<string, string> dataSave = Utilities.ConvertStringJsonToDictionary(JsonConvert.SerializeObject(data_info));
                                    response = System.IO.File.ReadAllText(pathFileOCR);                            
                                    if (item.hang_muc == "BANG_KE_CHI_TIET_HDON")
                                    {
                                        try
                                        {
                                            requestSave.action = ESCSStoredProcedures.PHT_OCR_BANG_KE_CHI_TIET_NH;
                                            var resultBangKe = JsonConvert.DeserializeObject<ocr_bang_ke_chi_tiet>(response);
                                            List<ht_ocr_bang_ke_chi_tiet> arrBangKe = new List<ht_ocr_bang_ke_chi_tiet>();
                                            if (resultBangKe.errorCode == "0" && resultBangKe.errorMessage.ToUpper() == "SUCCESS" && resultBangKe.data.Count() > 0)
                                            {
                                                string thanh_tien_str = ""; string so_luong_str = ""; string don_gia_str = "";
                                                decimal thanh_tien = 0; decimal so_luong = 0; decimal don_gia = 0;
                                                if (resultBangKe.data.FirstOrDefault().info.table == null)
                                                    continue;
                                                var listExpense = resultBangKe.data.FirstOrDefault().info.table.info_table;
                                                if (listExpense != null && listExpense.Count() > 0)
                                                {
                                                    for (var i = 0; i < listExpense.Count(); i++)
                                                    {
                                                        var listItem = listExpense[i];
                                                        for (var j = 0; j < listItem.Count(); j++)
                                                        {
                                                            ht_ocr_bang_ke_chi_tiet itemBangKe = new ht_ocr_bang_ke_chi_tiet();
                                                            var listValue = listItem[j];
                                                            for (var k = 0; k < listValue.Count(); k++)
                                                            {
                                                                if (k == 0)
                                                                {
                                                                    itemBangKe.ten_dich_vu = listValue[k].value;
                                                                }
                                                                else if (k == 1)
                                                                {
                                                                    thanh_tien_str = listValue[k].value.FormatStringToInterger();
                                                                    var ktra_thanh_tien = Decimal.TryParse(thanh_tien_str, out thanh_tien);
                                                                    if (!ktra_thanh_tien)
                                                                        thanh_tien = 0;
                                                                    itemBangKe.thanh_tien = thanh_tien.ToString();
                                                                }
                                                                else if (k == 2)
                                                                {
                                                                    so_luong_str = listValue[k].value.FormatStringToInterger();
                                                                    var ktra_so_luong = Decimal.TryParse(so_luong_str, out so_luong);
                                                                    if (!ktra_so_luong)
                                                                        so_luong = 0;
                                                                    itemBangKe.so_luong = so_luong.ToString(); ;
                                                                }
                                                                else if (k == 3)
                                                                {
                                                                    don_gia_str = listValue[k].value.FormatStringToInterger();
                                                                    var ktra_don_gia = Decimal.TryParse(don_gia_str, out don_gia);
                                                                    if (!ktra_don_gia)
                                                                        don_gia = 0;
                                                                    itemBangKe.don_gia = don_gia.ToString(); ;
                                                                }
                                                            }
                                                            arrBangKe.Add(itemBangKe);
                                                        }
                                                    }
                                                }
                                                var index = 0;
                                                foreach (var model in arrBangKe)
                                                {
                                                    dataSave.Add("arr[" + index + "][ten_dich_vu]", model.ten_dich_vu);
                                                    dataSave.Add("arr[" + index + "][thanh_tien]", model.thanh_tien);
                                                    dataSave.Add("arr[" + index + "][so_luong]", model.so_luong);
                                                    dataSave.Add("arr[" + index + "][don_gia]", model.don_gia);
                                                    index++;
                                                }
                                                dataSave.AddWithExists("hang_muc", item.ma_file);
                                                dataSave.AddWithExists("bt_anh", item.bt.ToString());
                                                BaseRequest rqSave = new BaseRequest(dataSave);
                                                BaseResponse<dynamic> resSave = new BaseResponse<dynamic>();
                                                prefixOCR.Add("arr", true);
                                                resSave.data_info = await _dynamicService.ExcuteDynamicAsync(rqSave, requestSave, prefixOCR, outValue =>
                                                {
                                                    resSave.out_value = outValue;
                                                });
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception("Lỗi OCR bảng kê chi tiết:" + ex.Message);
                                        }
                                    }
                                    else if (item.hang_muc == "HOA_DON_VIEN_PHI" || item.hang_muc == "BIEN_LAI" || item.hang_muc == "BILL")
                                    {
                                        try
                                        {
                                            var resultHoaDon = JsonConvert.DeserializeObject<ocr_hoa_don_vien_phi>(response);
                                            List<ht_ocr_hoa_don_chi_tiet> hoa_don_tong_hop = new List<ht_ocr_hoa_don_chi_tiet>();
                                            if (resultHoaDon.errorCode == "0" && resultHoaDon.errorMessage.ToUpper() == "SUCCESS" && resultHoaDon.data.Count() > 0)
                                            {
                                                requestSave.action = ESCSStoredProcedures.PHT_OCR_HOA_DON_CHI_TIET_NH;
                                                string ten_dich_vu_str = ""; string dvi_tinh_str = "";
                                                string thanh_tien_str = ""; string so_luong_str = ""; string don_gia_str = "";
                                                decimal thanh_tien = 0; decimal so_luong = 0; decimal don_gia = 0;
                                                if (resultHoaDon.data.FirstOrDefault().info.table == null)
                                                    continue;
                                                var listInvoice = resultHoaDon.data.FirstOrDefault().info.table;
                                                if (listInvoice.Count() > 0)
                                                {
                                                    for (var i = 0; i < listInvoice.Count(); i++)
                                                    {
                                                        var listItem = listInvoice[i];
                                                        for (var j = 0; j < listItem.Count(); j++)
                                                        {
                                                            var listValue = listItem[j];
                                                            if (listValue.label == "description")
                                                            {
                                                                ten_dich_vu_str = listValue.value;
                                                            }
                                                            if (listValue.label == "unit")
                                                            {
                                                                dvi_tinh_str = listValue.value;
                                                            }
                                                            if (listValue.label == "quantity")
                                                            {
                                                                so_luong_str = listValue.value.FormatStringToInterger();
                                                            }
                                                            if (listValue.label == "unit_price")
                                                            {
                                                                don_gia_str = listValue.value.FormatStringToInterger();
                                                            }
                                                            if (listValue.label == "amount_total")
                                                            {
                                                                thanh_tien_str = listValue.value.Replace(",", "").Replace(".", "").Replace(" ", "");
                                                            }
                                                            var ktra_so_luong = Decimal.TryParse(so_luong_str, out so_luong);
                                                            if (!ktra_so_luong)
                                                                so_luong = 0;
                                                            var ktra_don_gia = Decimal.TryParse(don_gia_str, out don_gia);
                                                            if (!ktra_don_gia)
                                                                don_gia = 0;
                                                            var ktra_thanh_tien = Decimal.TryParse(thanh_tien_str, out thanh_tien);
                                                            if (!ktra_thanh_tien)
                                                                thanh_tien = 0;
                                                        }
                                                        hoa_don_tong_hop.Add(new ht_ocr_hoa_don_chi_tiet() { ten_dich_vu = ten_dich_vu_str, so_luong = so_luong, dvi_tinh = dvi_tinh_str, don_gia = don_gia, thanh_tien = thanh_tien });
                                                    }
                                                    var index = 0;
                                                    foreach (var itemHoaDon in hoa_don_tong_hop)
                                                    {
                                                        if (itemHoaDon.don_gia == 0 && itemHoaDon.so_luong == 0 && itemHoaDon.thanh_tien == 0)
                                                            continue;
                                                        dataSave.Add("arr[" + index + "][ten_hang_muc]", itemHoaDon.ten_dich_vu);
                                                        dataSave.Add("arr[" + index + "][so_luong]", itemHoaDon.so_luong.ToString());
                                                        dataSave.Add("arr[" + index + "][don_gia]", itemHoaDon.don_gia.ToString());
                                                        dataSave.Add("arr[" + index + "][pt_giam_gia]", itemHoaDon.dvi_tinh.ToString());
                                                        dataSave.Add("arr[" + index + "][thanh_tien]", itemHoaDon.thanh_tien.ToString());
                                                        index++;
                                                    }
                                                    dataSave.AddWithExists("hang_muc", item.ma_file);
                                                    dataSave.AddWithExists("bt_anh", item.bt.ToString());
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
                                            throw new Exception("Lỗi OCR hoá đơn:"+ ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var dataOCR = arrHangMucOCR.Where(n => n.hang_muc == item.ma_file && n.nhom == item.nhom);
                                            List<EAVModel> result = new List<EAVModel>();
                                            foreach (var key in dataOCR)
                                            {
                                                result.AddRange(EAVCommon.GetValueByKey(response, key.ma, key.loai, key.stt));
                                            }
                                            var index = 0;
                                            foreach (var model in result)
                                            {
                                                dataSave.Add("arr[" + index + "][ma]", model.ma);
                                                dataSave.Add("arr[" + index + "][loai]", model.loai);
                                                dataSave.Add("arr[" + index + "][gia_tri]", model.gia_tri);
                                                dataSave.Add("arr[" + index + "][stt]", model.stt);
                                                index++;
                                            }
                                            dataSave.AddWithExists("hang_muc", item.ma_file);
                                            dataSave.AddWithExists("bt", item.bt.ToString());
                                            dataSave.AddWithExists("nhom", item.nhom);
                                            BaseRequest rqSave = new BaseRequest(dataSave);
                                            BaseResponse<dynamic> resSave = new BaseResponse<dynamic>();
                                            prefixOCR.Add("arr", true);
                                            resSave.data_info = await _dynamicService.ExcuteDynamicAsync(rqSave, requestSave, prefixOCR, outValue =>
                                            {
                                                resSave.out_value = outValue;
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception("Có lỗi trong quá trình lưu dữ liệu OCR vào bảng:" + ex.Message);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Lỗi đọc OCR:" + ex.Message);
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Chưa có dữ liệu OCR giấy tờ");
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi :" + ex.Message);
                }





                BaseResponse<dynamic> resSuccess = new BaseResponse<dynamic>();
                resSuccess.state_info.status = STATUS_OK;
                resSuccess.state_info.message_code = STATUS_OK;
                resSuccess.state_info.message_body = "Đọc OCR thành công";
                return Ok(resSuccess);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("qrcode")]
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
            if (action.actionapicode == ESCSStoredProcedures.PBH_BT_HS_QRCODE_LKE_CT)
            {
                bh_bt_hs_qrcode qrcode = await _dynamicService.ExcuteSingleAsync<bh_bt_hs_qrcode>(data_info, header);
                if (qrcode == null || string.IsNullOrEmpty(qrcode.url_file) || !System.IO.File.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, qrcode.url_file).ChuanHoaDuongDanRemote()))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Không xác định được file.";
                    return Ok(res);
                }
                var path = Path.Combine(AppSettings.PathFolderNotDeleteFull, qrcode.url_file).ChuanHoaDuongDanRemote();
                qrcode.file_base64 = Utilities.ConvertFileToBase64String(path);
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
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("read-qrcode")]
        [HttpPost]
        public async Task<IActionResult> ReadQRCode()
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
            var api_key = data_info.GetString("api_key");
            var decript = Utilities.DecryptByKey(api_key, AppSettings.KeyEryptData);
            if (!string.IsNullOrEmpty(decript))
            {
                string[] arr = new string[4];
                arr = decript.Split('&');
                var qrcode = new
                {
                    ma_doi_tac = arr[0].Trim().Split('=')[1],
                    so_id = arr[2].Trim().Split('=')[1],
                    nv = arr[1].Trim().Split('=')[1],
                    loai = arr[3].Trim().Split('=')[1]
                };

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
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("createbv")]
        [HttpPost]
        public async Task<IActionResult> CreateBV()
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
            if (action.actionapicode == ESCSStoredProcedures.PBH_BT_TIEP_NHAN_BAO_LANH_NH)
            {
                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; outPut = outValue; });
                try
                {
                    var ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                    var benh_vien = data_info.GetString("benh_vien");
                    var so_id = data_info.GetString("so_id");

                    if (!string.IsNullOrEmpty(ma_doi_tac_nsd) && !string.IsNullOrEmpty(benh_vien))
                    {
                        var filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE");
                        if (!Directory.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote()))
                            Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());

                        filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE", "BV" + ma_doi_tac_nsd + benh_vien + ".png");
                        if (!System.IO.File.Exists(Path.Combine(ma_doi_tac_nsd, filePath)))
                        {
                            var textQRCode = AppSettings.BVQRCodeLink + "/" + AppSettings.BVQRCodeVersion + "?ma_doi_tac=" + ma_doi_tac_nsd + "&benh_vien=" + benh_vien + "&signature=" + Utilities.Sha256Hash(ma_doi_tac_nsd + benh_vien + AppSettings.KeyEryptData);
                            QRCodeUtils.GenerateQRCode(textQRCode, Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());
                            var headerClone = header.Clone();
                            headerClone.action = ESCSStoredProcedures.PHT_MA_BENH_VIEN_QRCODE_NH;
                            var data = new Dictionary<string, object>();
                            data.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                            data.AddWithExists("benh_vien", data_info.GetString("benh_vien"));
                            data.AddWithExists("nsd", data_info.GetString("nsd"));
                            data.AddWithExists("pas", data_info.GetString("pas"));
                            data.AddWithExists("loai", "BENH_VIEN");
                            data.AddWithExists("url_file", filePath);
                            await _dynamicService.ExcuteDynamicNewAsync(data, headerClone);
                        }

                        var filePathBSHS = Path.Combine(ma_doi_tac_nsd, "QRCODE");
                        if (!Directory.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePathBSHS).ChuanHoaDuongDanRemote()))
                            Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePathBSHS).ChuanHoaDuongDanRemote());

                        filePathBSHS = Path.Combine(ma_doi_tac_nsd, "QRCODE", "BSCTNG" + ma_doi_tac_nsd + so_id + ".png");
                        if (!System.IO.File.Exists(Path.Combine(ma_doi_tac_nsd, filePathBSHS)))
                        {
                            var textQRCodeBSHS = AppSettings.QRBSHSCodeLink + "?hash=" + Utilities.EncryptByKey("ma_doi_tac=" + ma_doi_tac_nsd + "&nv=NG&so_id=" + so_id + "&loai=BSCT", AppSettings.KeyEryptData);
                            QRCodeUtils.GenerateQRCode(textQRCodeBSHS, Path.Combine(AppSettings.PathFolderNotDeleteFull, filePathBSHS).ChuanHoaDuongDanRemote());
                            var headerClone = header.Clone();
                            headerClone.action = ESCSStoredProcedures.PBH_BT_HS_QRCODE_NH;
                            var data = new Dictionary<string, object>();
                            data.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                            data.AddWithExists("so_id", so_id);
                            data.AddWithExists("nv", "NG");
                            data.AddWithExists("loai", "BSCT");
                            data.AddWithExists("nguon", "");
                            data.AddWithExists("url_file", filePathBSHS);
                            await _dynamicService.ExcuteDynamicNewAsync(data, headerClone);
                        }

                    }
                }
                catch (Exception ex)
                {
                }
                SendNotify(action, header, data_info);
                SendSMS(action, header, data_info);
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
        [Route("bvqrcode")]
        [HttpPost]
        public async Task<IActionResult> GetBVQRCode()
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
            if (action.actionapicode == ESCSStoredProcedures.PHT_MA_BENH_VIEN_GEN_QRCODE)
            {
                ht_ma_benh_vien_qrcode qrcode = await _dynamicService.ExcuteSingleAsync<ht_ma_benh_vien_qrcode>(data_info, header);
                if (qrcode == null || string.IsNullOrEmpty(qrcode.url_file) || !System.IO.File.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, qrcode.url_file).ChuanHoaDuongDanRemote()))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                    res.state_info.message_body = "Không xác định được file.";
                    return Ok(res);
                }
                var path = Path.Combine(AppSettings.PathFolderNotDeleteFull, qrcode.url_file).ChuanHoaDuongDanRemote();
                qrcode.file_base64 = Utilities.ConvertFileToBase64String(path);
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
        /// Excute
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
            openid_access_token access_token = null;
            header.check_ip_backlist = true;
            header.ip_remote_ipv4 = define_info["ip_remote_ipv4"].Value<string>();
            header.ip_remote_ipv6 = define_info["ip_remote_ipv6"].Value<string>();
            header.payload = payload;

            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out access_token, out keyCachePartner);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return Ok(res);
            }
            header.envcode = access_token.evncode;
            #endregion
            try
            {
                #region Lấy thông tin action
                var action = _dynamicService.GetConnection(header);
                if (action == null || action.actionapicode != ESCSStoredProcedures.PHT_BH_FILE_API_LKE_CT)
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
                if (action.actionapicode == ESCSStoredProcedures.PHT_BH_FILE_API_LKE_CT)
                {
                    #region Lấy thông tin đường dẫn file
                    byte[] arrOutput = null;
                    BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                    res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { });
                    if (res.data_info == null)
                        return Ok(res);
                    string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                    if (string.IsNullOrEmpty(jsonRes))
                        return Ok(res);
                    bh_file file = JsonConvert.DeserializeObject<bh_file>(jsonRes);
                    string pathFile = Path.Combine(AppSettings.PathFolderNotDeleteFull, file.duong_dan).ChuanHoaDuongDanRemote();
                    if (AppSettings.FolderSharedUsed && pathFile.StartsWith(@"\") && !pathFile.StartsWith(@"\\"))
                        pathFile = @"\" + pathFile;
                    if (!System.IO.File.Exists(pathFile))
                    {
                        BaseResponse<dynamic> resError = new BaseResponse<dynamic>();
                        resError.state_info.status = STATUS_NOTOK;
                        resError.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                        resError.state_info.message_body = "Không tồn tại file";
                        return Ok(resError);
                    }
                    var fileDownloadName = file.ma_file + "_" + file.bt + Path.GetExtension(pathFile);
                    arrOutput = System.IO.File.ReadAllBytes(pathFile);
                    return File(
                            fileContents: arrOutput,
                            contentType: System.Net.Mime.MediaTypeNames.Application.Octet,
                            fileDownloadName: fileDownloadName
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

                    if (AppSettings.ConnectApiCorePartnerHealth && config != null && config.Partner == CoreApiConfigContants.MIC)
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
                    else if (AppSettings.ConnectApiCorePartnerHealth && config != null && config.Partner == CoreApiConfigContants.OPES)
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
                            catch
                            {

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
                                            continue;
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
                            _logRequestService.AddLogAsync(new LogException("SENDSMS", ex.Message));
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
                                //if (SignatureFileConfig.Partner == CoreApiConfigContants.OPES)
                                //{
                                if (mau_in.trang == null || mau_in.x == null || mau_in.width == null || mau_in.height == null ||
                                    mau_in.trang < 0 || mau_in.x < 0 || mau_in.y < 0 || mau_in.width <= 0 || mau_in.height <= 0
                                )
                                    continue;
                                arrByte = await ApiGatewayOPES.KySoFile(ma_doi_tac_nsd, arrByte, mau_in.trang, mau_in.x, mau_in.y, mau_in.width, mau_in.height, mau_in.signer);
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
    }
}