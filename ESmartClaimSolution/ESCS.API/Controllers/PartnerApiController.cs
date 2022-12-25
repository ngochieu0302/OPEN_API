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
using ESCS.COMMON.Request.ApiGateway.OPES;
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
    [Route("api/partner")]
    [ApiController]
    [ESCSAuth]
    public class PartnerApiController : BaseController
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
        public PartnerApiController(
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PBH_BT_XE_GD_TIM_XE)
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
            #region Thực thi api trước khi excute db
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    res.state_info.message_body = "Chưa thiết lập cấu hình kết nối.";
                    return Ok(res);
                };

                var dataRequest = ApiGateway.RequestApiTruyVanXe(data_info);
                string json = await ApiGateway.CallApiTruyVanXe(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "TIM_KIEM_GCN", JsonConvert.SerializeObject(dataRequest), json);
                try
                {
                    var data = JObject.Parse(json);
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    throw new Exception("THÔNG BÁO: " + ex.Message);
                }
            }
            else if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.PJICO)
            {
                if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(config.Domain) || config.EndPoint == null || 
                    string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    res.state_info.message_body = "Chưa thiết lập cấu hình kết nối.";
                    return Ok(res);
                };

                var dataRequest = ApiGatewayPJICO.RequestApiTruyVanXe(data_info);
                string json = await ApiGatewayPJICO.CallApiTruyVanXe(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "TIM_KIEM_GCN", JsonConvert.SerializeObject(dataRequest), json);
                try
                {
                    var dataPjico = JsonConvert.DeserializeObject<PJICOResponse<List<GiayChungNhanPjico>>>(json);
                    if (dataPjico.returnCode != 0)
                        throw new Exception("THÔNG BÁO: " + dataPjico.errMess + "("+ dataPjico.returnCode + ")");
                    //Mapping lại dữ liệu ở đây
                    BaseResponse<DSGiayChungNhanESCS> dataESCS = new BaseResponse<DSGiayChungNhanESCS>();
                    dataESCS.data_info = new DSGiayChungNhanESCS();
                    dataESCS.data_info.hd = new List<GiayChungNhanESCS>();
                    if (dataPjico.data==null|| dataPjico.data.Count()<=0)
                        return Ok(dataESCS);
                    dataESCS.data_info.hd = ESCSConverter.ConvertGCNPjicoToESCS(dataPjico.data);
                    return Ok(dataESCS);
                }
                catch (Exception ex)
                {
                    throw new Exception("THÔNG BÁO: " + ex.Message);
                }
            }
            else if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_code = ErrorCodeOpenIDConstants.BAD_REQUEST;
                    res.state_info.message_body = "Chưa thiết lập cấu hình kết nối.";
                    return Ok(res);
                };

                var dataRequest = ApiGatewayOPES.RequestApiTruyVanXe(data_info);
                string json = await ApiGatewayOPES.CallApiTruyVanXe(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "TIM_KIEM_GCN", JsonConvert.SerializeObject(dataRequest), json);
                try
                {
                    var data = JObject.Parse(json);
                    return Ok(data);
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PTICH_HOP_HOP_DONG_XE)
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
            #region Xóa cache nếu action này thực hiện việc xóa cache
            await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PBH_HT_MA_HANG_XE_CACHE);
            await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PBH_HT_MA_HIEU_XE_CACHE);
            await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PHT_MA_LOAI_XE_CACHE);
            #endregion
            #region Thực thi api trước khi excute db
            string pas_tmp = data_info.GetString("pas");
            bool sdbs = false;
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string ma_chi_nhanh = data_info.GetString("ma_chi_nhanh");
            string so_id_hd = data_info.GetString("so_id_hd");
            string nsd = data_info.GetString("nsd");

            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                List<GiayChungNhan> gcn_tmp = new List<GiayChungNhan>();
                List<DieuKhoan> dk_tmp = new List<DieuKhoan>();
                decimal? so_id_hdong = 0;
                do
                {
                    var dataRequest = ApiGateway.RequestApiThongTinGCN(data_info);
                    string json = await ApiGateway.CallApiThongTinGCN(ma_doi_tac_nsd, dataRequest);
                    AddLog(header, ma_doi_tac_nsd, nsd, "XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);

                    BaseResponse<KQThongTinGCN> response = new BaseResponse<KQThongTinGCN>();
                    response = JsonConvert.DeserializeObject<BaseResponse<KQThongTinGCN>>(json);
                    if (response.state_info.status != STATUS_OK)
                        return Ok(response);

                    if (sdbs && (response.data_info.gcn == null || response.data_info.gcn.Count() <= 0))
                    {
                        response.data_info.gcn = gcn_tmp;
                        response.data_info.dk = dk_tmp;
                        if (response.data_info.gcn!=null && response.data_info.gcn.Count()>0)
                        {
                            foreach (var item in response.data_info.gcn)
                                item.so_id_hdong = so_id_hdong.Value.ToString();
                        }
                        if (response.data_info.dk != null && response.data_info.dk.Count() > 0)
                        {
                            foreach (var item in response.data_info.dk)
                                item.so_id_hdong = so_id_hdong.Value.ToString();
                        }
                    }

                    if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                    {
                        response.state_info.status = STATUS_NOTOK;
                        response.state_info.message_body = "Không tìm thấy hợp đồng/gcn trên core";
                        return Ok(response);
                    }
                    json = JsonConvert.SerializeObject(response);
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_XE;
                    await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                    var hd = response.data_info.hd.FirstOrDefault();
                    sdbs = hd.so_id_hdong != hd.so_id_hdong_dau;
                    data_info.AddWithExists("so_id_hd", hd.so_id_hdong_dau);

                    if (sdbs)
                    {
                        gcn_tmp = response.data_info.gcn;
                        dk_tmp = response.data_info.dk;
                        so_id_hdong = hd.so_id_hdong_dau;
                    }    

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
                        dicRequest.AddWithExists("nv", "XE");
                        var headerGateway = header.Clone();
                        headerGateway.action = ESCSStoredProcedures.PBH_HD_KY_THANH_TOAN_MIC_NH;
                        try { await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway); } catch { };
                    }
                }
                catch
                {

                }
            }
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                do
                {
                    var dataRequest = ApiGatewayOPES.RequestApiThongTinGCN(data_info);
                    string json = await ApiGatewayOPES.CallApiThongTinGCN(ma_doi_tac_nsd, dataRequest);
                    AddLog(header, ma_doi_tac_nsd, nsd, "XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);

                    BaseResponse<KQThongTinGCN> response = new BaseResponse<KQThongTinGCN>();
                    response = JsonConvert.DeserializeObject<BaseResponse<KQThongTinGCN>>(json);
                    if (response.state_info.status != STATUS_OK)
                        return Ok(response);
                    //Import hợp đồng
                    if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                    {
                        response.state_info.status = STATUS_NOTOK;
                        response.state_info.message_body = "Không tìm thấy hợp đồng/gcn trên core";
                        return Ok(response);
                    }
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_XE_OPES;
                    await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                    var hd = response.data_info.hd.FirstOrDefault();
                    sdbs = hd.so_id_hdong != hd.so_id_hdong_dau;
                    data_info.AddWithExists("so_id_hd", hd.so_id_hdong_dau);

                } while (sdbs);
                try
                {
                    data_info.AddWithExists("ma_chi_nhanh_ql", ma_chi_nhanh);
                    data_info.AddWithExists("so_id_hd", so_id_hd);
                    var dataRequestTTTTPhi = ApiGatewayOPES.RequestApiThanhToanPhi(data_info);
                    string json = await ApiGatewayOPES.CallApiThanhToanPhi(ma_doi_tac_nsd, dataRequestTTTTPhi);
                    AddLog(header, ma_doi_tac_nsd, nsd, "THANH_TOAN_PHI", JsonConvert.SerializeObject(dataRequestTTTTPhi), json);
                    BaseResponse<KQTinhTrangThanhToanPhiOpes> response = new BaseResponse<KQTinhTrangThanhToanPhiOpes>();
                    response = JsonConvert.DeserializeObject<BaseResponse<KQTinhTrangThanhToanPhiOpes>>(json);
                    if (response.state_info.status == STATUS_OK)
                    {
                        IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                        dicRequest = json.ConvertStringJsonToDictionary();
                        dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                        dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                        dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                        dicRequest.AddWithExists("pas", pas_tmp);
                        dicRequest.AddWithExists("so_id", data_info.GetString("so_id_hd"));
                        dicRequest.AddWithExists("nv", "XE");
                        var headerGateway = header.Clone();
                        headerGateway.action = ESCSStoredProcedures.PBH_HD_KY_THANH_TOAN_OPES_NH;
                        try { await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway); } catch (Exception ex) { };
                    }
                }
                catch
                {

                }
            }
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.PJICO)
            {
                List<GiayChungNhan> gcn_tmp = new List<GiayChungNhan>();
                List<DieuKhoan> dk_tmp = new List<DieuKhoan>();
                do
                {
                    var dataRequest = ApiGatewayPJICO.RequestApiThongTinGCN(data_info);
                    string json = await ApiGatewayPJICO.CallApiThongTinGCN(ma_doi_tac_nsd, dataRequest);
                    AddLog(header, ma_doi_tac_nsd, nsd, "XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);
                    PJICOResponse<List<KQThongTinGCNPjico>> dataPjico = JsonConvert.DeserializeObject<PJICOResponse<List<KQThongTinGCNPjico>>>(json);
                    //Kiểm tra trạng thái và convert data
                    if (dataPjico.returnCode != 0)
                        throw new Exception("THÔNG BÁO: " + dataPjico.errMess + "(" + dataPjico.returnCode + ")");

                    BaseResponse<KQThongTinGCN> response = new BaseResponse<KQThongTinGCN>();
                    response.data_info = ESCSConverter.ConvertGCNPjicoToESCSDetail(data_info.GetString("so_hdong"), data_info.GetString("so_id_hd"), data_info.GetString("so_id_gcn"), dataPjico.data);
                    if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                    {
                        response.state_info.status = STATUS_NOTOK;
                        response.state_info.message_body = "Không tìm thấy hợp đồng/gcn trên core";
                        return Ok(response);
                    }
                    json = JsonConvert.SerializeObject(response);
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_XE_PJICO;
                    await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                } while (sdbs);
                //try
                //{
                //    data_info.AddWithExists("ma_chi_nhanh_ql", ma_chi_nhanh);
                //    data_info.AddWithExists("so_id_hd", so_id_hd);
                //    var dataRequestTTTTPhi = ApiGateway.RequestApiThanhToanPhi(data_info);
                //    string json = await ApiGateway.CallApiThanhToanPhi(ma_doi_tac_nsd, dataRequestTTTTPhi);
                //    AddLog(header, ma_doi_tac_nsd, nsd, "THANH_TOAN_PHI", JsonConvert.SerializeObject(dataRequestTTTTPhi), json);
                //    BaseResponse<KQTinhTrangThanhToanPhi> response = new BaseResponse<KQTinhTrangThanhToanPhi>();
                //    response = JsonConvert.DeserializeObject<BaseResponse<KQTinhTrangThanhToanPhi>>(json);
                //    if (response.state_info.status == STATUS_OK)
                //    {
                //        IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                //        dicRequest = json.ConvertStringJsonToDictionary();
                //        dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                //        dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                //        dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                //        dicRequest.AddWithExists("pas", pas_tmp);
                //        dicRequest.AddWithExists("so_id", data_info.GetString("so_id_hd"));
                //        dicRequest.AddWithExists("nv", "XE");
                //        var headerGateway = header.Clone();
                //        headerGateway.action = ESCSStoredProcedures.PBH_HD_KY_THANH_TOAN_MIC_NH;
                //        try { await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway); } catch { };
                //    }
                //}
                //catch
                //{

                //}
            }
            return Ok(res);
            #endregion
        }
        /// <summary>
        /// Xem thông tin giấy chứng nhận
        /// </summary>
        /// <returns></returns>
        [Route("view-policy")]
        [HttpPost]
        public async Task<IActionResult> ViewPolicy()
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PBH_BT_XE_GD_HD_CT)
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
            #region Thực thi api trước khi excute db
            string pas_tmp = data_info.GetString("pas");
            bool sdbs = true;
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PBH_HT_MA_HANG_XE_CACHE);
                await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PBH_HT_MA_HIEU_XE_CACHE);
                await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PHT_MA_LOAI_XE_CACHE);
                do
                {
                    data_info.AddWithExists("so_id_gcn", data_info.GetString("so_id_dt"));
                    var dataRequest = ApiGateway.RequestApiThongTinGCN(data_info);
                    string json = await ApiGateway.CallApiThongTinGCN(ma_doi_tac_nsd, dataRequest);
                    AddLog(header, ma_doi_tac_nsd, nsd, "XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);

                    BaseResponse<KQThongTinGCN> response = new BaseResponse<KQThongTinGCN>();
                    response = JsonConvert.DeserializeObject<BaseResponse<KQThongTinGCN>>(json);
                    if (response.state_info.status != STATUS_OK)
                        return Ok(response);
                    //Import hợp đồng
                    if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                    {
                        response.state_info.status = STATUS_NOTOK;
                        response.state_info.message_body = "Không tìm thấy hợp đồng/gcn trên core";
                        return Ok(response);
                    }
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_XE;
                    await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                    var hd = response.data_info.hd.FirstOrDefault();
                    sdbs = hd.so_id_hdong != hd.so_id_hdong_dau;
                    data_info.AddWithExists("so_id_hd", hd.so_id_hdong_dau);

                } while (sdbs);
            }
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.PJICO)
            {
                sdbs = false;
                await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PBH_HT_MA_HANG_XE_CACHE);
                await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PBH_HT_MA_HIEU_XE_CACHE);
                await _dynamicService.ClearCacheActions(header, _cacheServer, ESCSStoredProcedures.PHT_MA_LOAI_XE_CACHE);

                List<GiayChungNhan> gcn_tmp = new List<GiayChungNhan>();
                List<DieuKhoan> dk_tmp = new List<DieuKhoan>();
                do
                {
                    data_info.AddWithExists("so_id_gcn", data_info.GetString("so_id_dt"));
                    if (string.IsNullOrEmpty(data_info.GetString("so_id_hd")))
                        data_info.AddWithExists("so_id_hd", data_info.GetString("so_id_dt"));

                    var dataRequest = ApiGatewayPJICO.RequestApiThongTinGCN(data_info);
                    string json = await ApiGatewayPJICO.CallApiThongTinGCN(ma_doi_tac_nsd, dataRequest);
                    AddLog(header, ma_doi_tac_nsd, nsd, "XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);
                    PJICOResponse<List<KQThongTinGCNPjico>> dataPjico = JsonConvert.DeserializeObject<PJICOResponse<List<KQThongTinGCNPjico>>>(json);
                    //Kiểm tra trạng thái và convert data
                    if (dataPjico.returnCode != 0)
                        throw new Exception("THÔNG BÁO: " + dataPjico.errMess + "(" + dataPjico.returnCode + ")");

                    BaseResponse<KQThongTinGCN> response = new BaseResponse<KQThongTinGCN>();
                    response.data_info = ESCSConverter.ConvertGCNPjicoToESCSDetail(data_info.GetString("so_hdong"), data_info.GetString("so_id_hd"), data_info.GetString("so_id_gcn"), dataPjico.data);
                    if (response.data_info.hd == null || response.data_info.gcn == null || response.data_info.hd.Count <= 0 || response.data_info.gcn.Count <= 0)
                    {
                        response.state_info.status = STATUS_NOTOK;
                        response.state_info.message_body = "Không tìm thấy hợp đồng/gcn trên core";
                        return Ok(response);
                    }
                    json = JsonConvert.SerializeObject(response);
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PTICH_HOP_HOP_DONG_XE_PJICO;
                    await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway);
                } while (sdbs);
            }

            var outPut = new Dictionary<string, object>();
            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
            {
                res.out_value = outValue;
            });
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
            if (action == null || (action.actionapicode != ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE && action.actionapicode != ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE_OPES))
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
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            BaseResponse<dynamic> response = new BaseResponse<dynamic>();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC && action.actionapicode == ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE)
            {
                BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                var outPut = new Dictionary<string, object>();
                resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                {
                    resData.out_value = outValue;
                });
                DuLieuBoiThuong duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuong>(JsonConvert.SerializeObject(resData.data_info));
                if (duLieuBT != null && duLieuBT.hs != null)
                {
                    duLieuBT.hs.so_cv_kn = (duLieuBT.hs.so_cv_kn == null || string.IsNullOrEmpty(duLieuBT.hs.so_cv_kn.Trim())) ? "" : duLieuBT.hs.so_cv_kn.Trim();
                    duLieuBT.hs.so_hs = (duLieuBT.hs.so_hs == null || string.IsNullOrEmpty(duLieuBT.hs.so_hs.Trim())) ? "" : duLieuBT.hs.so_hs.Trim();
                }
                data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));

                string json = await ApiGateway.CallApiChuyenDLBoiThuong(ma_doi_tac_nsd, duLieuBT);
                AddLog(header, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);

                response = JsonConvert.DeserializeObject<BaseResponse<dynamic>>(json);
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
            }
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES && action.actionapicode == ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE_OPES)
            {
                BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                var outPut = new Dictionary<string, object>();
                resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                {
                    resData.out_value = outValue;
                });
                DuLieuBoiThuongOpes duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongOpes>(JsonConvert.SerializeObject(resData.data_info));
                data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));
                string json = await ApiGatewayOPES.CallApiChuyenDLBoiThuong(ma_doi_tac_nsd, duLieuBT);
                AddLog(header, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                var resTichHop = JsonConvert.DeserializeObject<BaseResponse<TichHopOpesResponse>>(json);
                if (resTichHop==null || resTichHop.data_info == null || string.IsNullOrEmpty(resTichHop.data_info.so_hs))
                    throw new Exception("[Thông báo từ core] - Có lỗi trong quá trình tích hợp");

                response.state_info = resTichHop.state_info;
                response.out_value = new { so_ho_so = resTichHop.data_info?.so_hs, so_tn = resTichHop.data_info?.so_tn };
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
            }
            return Ok(response);
            #endregion
        }
        /// <summary>
        /// Xem tình trạng thanh toán phí
        /// </summary>
        /// <returns></returns>
        [Route("get-payment")]
        [HttpPost]
        public async Task<IActionResult> GetPayment()
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
            #region Thực thi api trước khi excute db
            var pas_tmp = data_info.GetString("pas");
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");

            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                var dataRequest = ApiGateway.RequestApiThanhToanPhi(data_info);
                string json = await ApiGateway.CallApiThanhToanPhi(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "THANH_TOAN_PHI", JsonConvert.SerializeObject(dataRequest), json);
                BaseResponse<KQTinhTrangThanhToanPhi> response = new BaseResponse<KQTinhTrangThanhToanPhi>();
                response = JsonConvert.DeserializeObject<BaseResponse<KQTinhTrangThanhToanPhi>>(json);
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
                IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                dicRequest = json.ConvertStringJsonToDictionary();
                dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                dicRequest.AddWithExists("pas", pas_tmp);
                dicRequest.AddWithExists("so_id", data_info.GetString("so_id_hd"));
                dicRequest.AddWithExists("nv", data_info.GetString("nv"));
                var headerGateway = header.Clone();
                headerGateway.action = ESCSStoredProcedures.PBH_HD_KY_THANH_TOAN_MIC_NH;
                try { await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway); } catch { };
            }
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                var dataRequestTTTTPhi = ApiGatewayOPES.RequestApiThanhToanPhi(data_info);
                string json = await ApiGatewayOPES.CallApiThanhToanPhi(ma_doi_tac_nsd, dataRequestTTTTPhi);
                AddLog(header, ma_doi_tac_nsd, nsd, "THANH_TOAN_PHI", JsonConvert.SerializeObject(dataRequestTTTTPhi), json);
                BaseResponse<KQTinhTrangThanhToanPhiOpes> response = new BaseResponse<KQTinhTrangThanhToanPhiOpes>();
                response = JsonConvert.DeserializeObject<BaseResponse<KQTinhTrangThanhToanPhiOpes>>(json);
                if (response.state_info.status == STATUS_OK)
                {
                    IDictionary<string, object> dicRequest = new Dictionary<string, object>();
                    dicRequest = json.ConvertStringJsonToDictionary();
                    dicRequest.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                    dicRequest.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                    dicRequest.AddWithExists("nsd", data_info.GetString("nsd"));
                    dicRequest.AddWithExists("pas", pas_tmp);
                    dicRequest.AddWithExists("so_id", data_info.GetString("so_id_hd"));
                    dicRequest.AddWithExists("nv", "XE");
                    var headerGateway = header.Clone();
                    headerGateway.action = ESCSStoredProcedures.PBH_HD_KY_THANH_TOAN_OPES_NH;
                    try { await _dynamicService.ExcuteDynamicNewAsync(dicRequest, headerGateway); } catch (Exception ex) { };
                }
            }
            res.state_info.status = STATUS_OK;
            var resData = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
            res.data_info = resData;
            return Ok(res);
            #endregion
        }
        /// <summary>
        /// Lấy danh sách file
        /// </summary>
        /// <returns></returns>
        [Route("list-file")]
        [HttpPost]
        public async Task<IActionResult> GetListFile()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            var data_info = Request.GetData(out var define_info, out string payload);
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
            if (action == null && action.actionapicode != ESCSStoredProcedures.PHT_BH_FILE_THUMNAIL)
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
            #region Thực thi api trước khi excute db
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            BaseResponse<List<KQDanhSachFile>> response = new BaseResponse<List<KQDanhSachFile>>();

            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
            {

                var dataRequest = ApiGateway.RequestApiDanhSachFile(data_info);
                string json = await ApiGateway.CallApiDanhSachFile(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "LAY_DS_FILE", JsonConvert.SerializeObject(dataRequest), json);

                response = JsonConvert.DeserializeObject<BaseResponse<List<KQDanhSachFile>>>(json);
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
                var so_id = data_info.GetString("so_id");
                var ma_chi_nhanh = data_info.GetString("ma_chi_nhanh");
                if (response.data_info != null && response.data_info.Count > 0)
                {
                    foreach (var item in response.data_info)
                    {
                        item.ma_chi_nhanh = ma_chi_nhanh;
                        item.nhom_anh = item.ten_file;
                        item.so_id = so_id;
                        item.pm = "API";
                        item.loai = item.loai_file;
                        if (item.loai_file != "DGRR")
                        {
                            item.duong_dan = item.ma_file;
                            item.extension = ".pdf";
                        }
                        else
                        {
                            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "damage.png");
                            item.duong_dan = Utilities.ConvertFileToBase64String(path);
                            item.extension = ".png";
                        }

                    }
                }
            }

            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                BaseResponse<KQDanhSachFileObj> responseTmp = new BaseResponse<KQDanhSachFileObj>();
                var dataRequest = ApiGatewayOPES.RequestApiDanhSachFile(data_info);
                string json = await ApiGatewayOPES.CallApiDanhSachFile(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "LAY_DS_FILE", JsonConvert.SerializeObject(dataRequest), json);
                responseTmp = JsonConvert.DeserializeObject<BaseResponse<KQDanhSachFileObj>>(json);
                response.state_info = responseTmp.state_info;
                response.data_info = responseTmp.data_info?.ds_file;

                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
                var so_id = data_info.GetString("so_id");
                var ma_chi_nhanh = data_info.GetString("ma_chi_nhanh");

                if (response != null && response.data_info != null && response.data_info != null && response.data_info.Count > 0)
                {
                    foreach (var item in response.data_info)
                    {
                        item.ma_chi_nhanh = ma_chi_nhanh;
                        item.nhom_anh = item.ten_file;
                        item.so_id = so_id;
                        item.pm = "API";
                        item.loai = item.loai_file;
                        if (item.loai_file == "GCN")
                        {
                            item.nhom_anh = "Giấy chứng nhận bảo hiểm";
                        }
                        if (item.loai_file != "DGRR")
                        {
                            item.duong_dan = item.ma_file;
                            item.extension = ".pdf";
                        }
                        else
                        {
                            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "ImageDefault", "damage.png");
                            item.duong_dan = Utilities.ConvertFileToBase64String(path);
                            item.extension = ".png";
                        }

                    }
                }
            }
            return Ok(response);
            #endregion
        }
        /// <summary>
        /// Lấy thông tin chi tiết file
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
            if (action == null && action.actionapicode != ESCSStoredProcedures.PHT_BH_FILE_TAI_FILE)
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
            #region Thực thi api trước khi excute db
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            BaseResponse<KQDanhSachFile> response = new BaseResponse<KQDanhSachFile>();
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                var dataRequest = ApiGateway.RequestApiLayFile(data_info);
                string json = await ApiGateway.CallApiLayFile(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "XEM_FILE", JsonConvert.SerializeObject(dataRequest), "");

                response = JsonConvert.DeserializeObject<BaseResponse<KQDanhSachFile>>(json);
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
                if (response.data_info == null || string.IsNullOrEmpty(response.data_info.file))
                {
                    response.state_info.status = STATUS_NOTOK;
                    response.state_info.message_body = "Không lấy được thông tin file từ core";
                    return Ok(response);
                }
                response.data_info.duong_dan = response.data_info.file;
                response.data_info.extension = Utilities.GetFileExtension(response.data_info.file);
            }
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                var dataRequest = ApiGatewayOPES.RequestApiLayFile(data_info);
                string json = await ApiGatewayOPES.CallApiLayFile(ma_doi_tac_nsd, dataRequest);
                AddLog(header, ma_doi_tac_nsd, nsd, "XEM_FILE", JsonConvert.SerializeObject(dataRequest), "");

                response = JsonConvert.DeserializeObject<BaseResponse<KQDanhSachFile>>(json);
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);
                if (response.data_info == null || string.IsNullOrEmpty(response.data_info.file))
                {
                    response.state_info.status = STATUS_NOTOK;
                    response.state_info.message_body = "Không lấy được thông tin file từ core";
                    return Ok(response);
                }
                response.data_info.duong_dan = response.data_info.file;
                response.data_info.extension = Utilities.GetFileExtension(response.data_info.file);
            }
            return Ok(response);
            #endregion
        }
        /// <summary>
        /// Send mail v1 OPES
        /// </summary>
        /// <returns></returns>
        [Route("send-email")]
        [HttpPost]
        public async Task<IActionResult> SendEmail()
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
            var action = _dynamicService.GetConnection(header);
            if (action == null && action.actionapicode != ESCSStoredProcedures.PBH_BT_XE_HS_LAY_TTIN_EMAIL_TEMP)
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
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            if (string.IsNullOrEmpty(ma_doi_tac_nsd))
                ma_doi_tac_nsd = data_info.GetString("ma_doi_tac");

            string nsd = data_info.GetString("nsd");
            decimal so_id = Decimal.Parse(data_info.GetString("so_id"));
            string nv = data_info.GetString("nv");
            string ma_email = data_info.GetString("loai");
            #region Thực thi type excute db
            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { resData.out_value = outValue; });
            #endregion
            if (ma_doi_tac_nsd == CoreApiConfigContants.OPES && so_id != 0 && ma_email == "TEMPLATE_EMAIL_BSHS" && nv == "XE")
            {
                try
                {
                    HeaderRequest requestDB = header.Clone();
                    requestDB.action = ESCSStoredProcedures.PBH_BT_HO_SO_GIAY_TO_NSD_YCBS;
                    IDictionary<string, object> listFiles = new Dictionary<string, object>();
                    listFiles.Add("ma_doi_tac", ma_doi_tac_nsd);
                    listFiles.Add("so_id", so_id);
                    listFiles.Add("nv", nv);
                    var dataBSHS = await _dynamicService.ExcuteListAsync<ds_tai_lieu_yc_bo_sung>(listFiles, requestDB);
                    if (dataBSHS != null && dataBSHS.Count() > 0)
                    {
                        opes_yc_bshs tai_lieu = new opes_yc_bshs();
                        tai_lieu.so_id_hs = (long)so_id;
                        tai_lieu.tl_bs = new List<opes_tai_lieu>();
                        foreach (var item in dataBSHS)
                            tai_lieu.tl_bs.Add(new opes_tai_lieu() { ma_tl = item.ma_tl, ten_tl = item.ten_tl, ghi_chu = item.ghi_chu });
                        var resBSHS = await ApiGatewayOPES.YeuCauBSCT(ma_doi_tac_nsd, tai_lieu);
                        AddLog(requestDB, ma_doi_tac_nsd, nsd, "YC_BSHS", JsonConvert.SerializeObject(tai_lieu) , resBSHS);
                    }
                    else
                    {
                        AddLog(requestDB, ma_doi_tac_nsd, nsd, "YC_BSHS_FOUND", JsonConvert.SerializeObject(listFiles), "");
                    }    
                }
                catch (Exception ex)
                {
                    AddLog(header, ma_doi_tac_nsd, nsd, "YC_BSHS_EX", ex.Message, "");
                }
            }
            return Ok(resData);
        }
        /// <summary>
        /// Khách hàng phản hồi upload tài liệu
        /// </summary>
        /// <returns></returns>
        [Route("customer-confirm")]
        [HttpPost]
        public async Task<IActionResult> CustomerConfirm()
        {
            #region Lấy thông tin header và kiểm tra token
            HeaderRequest header = Request.GetHeader();
            string payload = string.Empty;
            var data = Request.GetData(out var define_info, out payload);
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
            var action = _dynamicService.GetConnection(header);
            if (action == null && action.actionapicode != ESCSStoredProcedures.PBH_BT_HS_Y_KIEN_KH_NH)
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
            string ma_doi_tac_nsd = data.GetString("ma_doi_tac_nsd");
            if (string.IsNullOrEmpty(ma_doi_tac_nsd))
                ma_doi_tac_nsd = data.GetString("ma_doi_tac");
            string nhom = data.GetString("nhom");
            string so_id = data.GetString("so_id");
            string nv = data.GetString("nv");
            #region Thực thi type excute db
            BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
            var outPut = new Dictionary<string, object>();
            resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data, header, outValue => { resData.out_value = outValue; });
            #endregion
            if (ma_doi_tac_nsd == CoreApiConfigContants.OPES && !string.IsNullOrEmpty(so_id) && so_id != "0" && nhom == "BSCT" && nv == "XE")
            {
                try
                {
                    IDictionary<string, object> listFiles = new Dictionary<string, object>();
                    listFiles.Add("ma_doi_tac", ma_doi_tac_nsd);
                    listFiles.Add("so_id", so_id);
                    var resKHBS = await ApiGatewayOPES.KhachHangPhanHoiBSCT(ma_doi_tac_nsd, listFiles);
                    AddLog(header, ma_doi_tac_nsd, "", "KH_BSCT", so_id, resKHBS);
                }
                catch (Exception ex)
                {
                    AddLog(header, ma_doi_tac_nsd, "", "KH_BSCT_EX", so_id, ex.Message);
                }
            }
            return Ok(resData);
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