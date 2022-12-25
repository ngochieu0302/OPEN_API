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
    [Route("api/carclaim")]
    [ApiController]
    [ESCSAuth]
    public class CarClaimController : BaseController
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
        public CarClaimController(
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
            ILogger<CarClaimController> logger)
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PBH_BT_XE_GD_CAP_NHAT_SO_HS)
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
            var headerKtraPhanCapUoc = header.Clone();
            headerKtraPhanCapUoc.action = ESCSStoredProcedures.PBH_BT_XE_KTRA_PHAN_CAP_UOC;
            await _dynamicService.ExcuteDynamicNewAsync(data_info, headerKtraPhanCapUoc);

            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
            {
                var headerMIC = header.Clone();
                headerMIC.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE;

                BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                var outPut = new Dictionary<string, object>();
                resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerMIC, outValue =>
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
                AddLog(headerMIC, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);

                var responseMIC = JsonConvert.DeserializeObject<BaseResponse<dynamic, out_value_tich_hop>>(json);
                if (responseMIC.state_info.status != STATUS_OK)
                    return Ok(responseMIC);

                headerMIC.action = ESCSStoredProcedures.PTICH_HOP_SO_HS;
                data_info.AddWithExists("so_hs", responseMIC.out_value.so_ho_so);
                var responseLaySo = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerMIC);
                return Ok(responseLaySo);

            }
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                var headerOPES = header.Clone();
                headerOPES.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE_OPES;

                BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                var outPut = new Dictionary<string, object>();
                resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerOPES, outValue =>
                {
                    resData.out_value = outValue;
                });
                DuLieuBoiThuongOpes duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongOpes>(JsonConvert.SerializeObject(resData.data_info));
                data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));
                string json = await ApiGatewayOPES.CallApiChuyenDLBoiThuong(ma_doi_tac_nsd, duLieuBT);
                AddLog(headerOPES, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                var resTichHop = JsonConvert.DeserializeObject<BaseResponse<TichHopOpesResponse>>(json);
                if (resTichHop == null || resTichHop.data_info == null || string.IsNullOrEmpty(resTichHop.data_info.so_hs))
                    throw new Exception("[Thông báo từ core] - Có lỗi trong quá trình tích hợp");
                response.state_info = resTichHop.state_info;
                response.out_value = new { so_ho_so = resTichHop.data_info?.so_hs, so_tn = resTichHop.data_info?.so_tn };
                if (response.state_info.status != STATUS_OK)
                    return Ok(response);

                headerOPES.action = ESCSStoredProcedures.PTICH_HOP_SO_HS_SO_TN;
                data_info.AddWithExists("so_hs", resTichHop.data_info?.so_hs);
                data_info.AddWithExists("so_tn", resTichHop.data_info?.so_tn);
                var responseLaySo = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerOPES);
                return Ok(responseLaySo);

            }
            response.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => {
                response.out_value = outValue;
            });
            return Ok(response);
            #endregion
        }
        /// <summary>
        /// Tiếp nhận hồ sơ
        /// Kiểm tra xem có chuyển giám định luôn ko
        /// </summary>
        /// <returns></returns>
        [Route("receive")]
        [HttpPost]
        public async Task<IActionResult> TiepNhanHS()
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

            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            string pas = data_info.GetString("pas");
            var nguon_api = data_info.GetString("nguon_api");
            if (!string.IsNullOrEmpty(nguon_api) && !string.IsNullOrEmpty(ma_doi_tac_nsd) && nguon_api== "MOBILE_PARTNER")
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.MIC)
                {
                    //Tích hợp hợp đồng ở đây
                    List<GiayChungNhan> gcn_tmp = new List<GiayChungNhan>();
                    List<DieuKhoan> dk_tmp = new List<DieuKhoan>();
                    bool sdbs = false;
                    decimal? so_id_hdong = 0;
                    do
                    {
                        //ma_chi_nhanh - Có trong tham số
                        //so_id_hd - Có trong tham số
                        //so_id_gcn - Chưa có trong tham số
                        if (string.IsNullOrEmpty(data_info.GetString("ma_chi_nhanh")) 
                            || string.IsNullOrEmpty(data_info.GetString("so_id_hd"))
                            || string.IsNullOrEmpty(data_info.GetString("so_id_dt")))
                        {
                            throw new Exception("Thiếu thông tin xác định đối tượng tổn thất (ma_chi_nhanh, so_id_hd, so_id_dt)");
                        }

                        data_info.AddWithExists("so_id_gcn", data_info.GetString("so_id_dt"));
                        var dataRequest = ApiGateway.RequestApiThongTinGCN(data_info);
                        string json = await ApiGateway.CallApiThongTinGCN(ma_doi_tac_nsd, dataRequest);
                        AddLog(header, ma_doi_tac_nsd, nsd, "XEM_CIET_GCN", JsonConvert.SerializeObject(dataRequest), json);
                        BaseResponse<KQThongTinGCN> response = new BaseResponse<KQThongTinGCN>();
                        try
                        {
                            response = JsonConvert.DeserializeObject<BaseResponse<KQThongTinGCN>>(json);
                        }
                        catch(Exception ex)
                        {
                            throw new Exception("Không tích hợp được thông tin hợp đồng sang hệ thống bồi thường");
                        }
                        if (response.state_info.status != STATUS_OK)
                        {
                            response.state_info.message_body = "[THÔNG BÁO TỪ CORE] - " + response.state_info.message_body;
                            return Ok(response);
                        }    

                        if (sdbs && (response.data_info.gcn == null || response.data_info.gcn.Count() <= 0))
                        {
                            response.data_info.gcn = gcn_tmp;
                            response.data_info.dk = dk_tmp;
                            if (response.data_info.gcn != null && response.data_info.gcn.Count() > 0)
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
                        dicRequest.AddWithExists("pas", pas);
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
                        //ma_chi_nhanh_ql - Chưa có trong tham số
                        //so_id_hd - Đã có trong tham số
                        data_info.AddWithExists("ma_chi_nhanh_ql", data_info.GetString("ma_chi_nhanh"));
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
                            dicRequest.AddWithExists("pas", pas);
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
            }

            var outPut = new Dictionary<string, object>();
            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue; outPut = outValue; });
            
            var so_id = outPut.GetString("so_id");
            var nv = outPut.GetString("nv");
            //Nếu có cấu hình yêu cầu lấy số từ core khi chuyển hồ sơ tiếp nhận sang giám định
            if (res.out_value != null && res.out_value.GetOutValue("ch_lay_so") == "C")
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
                {
                    try
                    {
                        BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                        header.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE_OPES;
                        data_info.AddWithExists("so_id", res.out_value.GetOutValue("so_id"));
                        data_info.AddWithExists("hanh_dong", "LAY_SO_HS");
                        resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                        {
                            resData.out_value = outValue;
                        });
                        DuLieuBoiThuongOpes duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongOpes>(JsonConvert.SerializeObject(resData.data_info));
                        data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));
                        string json = await ApiGatewayOPES.CallApiChuyenDLBoiThuong(ma_doi_tac_nsd, duLieuBT);
                        AddLog(header, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                        var resTichHop = JsonConvert.DeserializeObject<BaseResponse<TichHopOpesResponse>>(json);
                        if (resTichHop.state_info.status == STATUS_OK)
                        {
                            header.action = ESCSStoredProcedures.PTICH_HOP_SO_HS_SO_TN;
                            data_info.AddWithExists("ma_doi_tac", ma_doi_tac_nsd);
                            data_info.AddWithExists("so_hs", resTichHop.data_info?.so_hs);
                            data_info.AddWithExists("so_tn", resTichHop.data_info?.so_tn);
                            await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(ma_doi_tac_nsd) && !string.IsNullOrEmpty(so_id))
                {
                    var filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE");
                    if (!Directory.Exists(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote()))
                        Directory.CreateDirectory(Path.Combine(AppSettings.PathFolderNotDeleteFull, filePath).ChuanHoaDuongDanRemote());
                    if (string.IsNullOrEmpty(nv))
                        nv = "XE";
                    filePath = Path.Combine(ma_doi_tac_nsd, "QRCODE", "HOSO"+ nv + ma_doi_tac_nsd + so_id + ".png");
                    if (!System.IO.File.Exists(Path.Combine(ma_doi_tac_nsd, filePath)))
                    {
                        var textQRCode = Utilities.EncryptByKey("ma_doi_tac=" + ma_doi_tac_nsd + "&nv=" + nv + "&so_id=" + so_id + "&loai=HOSO", AppSettings.KeyEryptData);
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
            FileAction(action, header, data_info);
            return Ok(res);
            #endregion
        }
        /// <summary>
        /// Chuyển giám định 
        /// Kiểm tra xem có lấy số luôn không
        /// </summary>
        /// <returns></returns>
        [Route("assessment/tranfer")]
        [HttpPost]
        public async Task<IActionResult> ChuyenGD()
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
            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue => { res.out_value = outValue;});
            ////Nếu có cấu hình yêu cầu lấy số từ core khi chuyển hồ sơ tiếp nhận sang giám định
            if (res.out_value != null && res.out_value.GetOutValue("ch_lay_so") == "C")
            {
                string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
                string nsd = data_info.GetString("nsd");
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
                {
                    try
                    {
                        BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                        header.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE_OPES;
                        data_info.AddWithExists("hanh_dong", "LAY_SO_HS");
                        resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>
                        {
                            resData.out_value = outValue;
                        });
                        DuLieuBoiThuongOpes duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongOpes>(JsonConvert.SerializeObject(resData.data_info));
                        data_info.AddWithExists("pas_tmp", data_info.GetString("pas"));
                        string json = await ApiGatewayOPES.CallApiChuyenDLBoiThuong(ma_doi_tac_nsd, duLieuBT);
                        AddLog(header, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                        var resTichHop = JsonConvert.DeserializeObject<BaseResponse<TichHopOpesResponse>>(json);
                        if (resTichHop.state_info.status==STATUS_OK)
                        {
                            header.action = ESCSStoredProcedures.PTICH_HOP_SO_HS_SO_TN;
                            data_info.AddWithExists("ma_doi_tac", ma_doi_tac_nsd);
                            data_info.AddWithExists("so_hs", resTichHop.data_info?.so_hs);
                            data_info.AddWithExists("so_tn", resTichHop.data_info?.so_tn);
                            await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
            SendNotify(action, header, data_info);
            SendSMS(action, header, data_info);
            FileAction(action, header, data_info);
            return Ok(res);
            #endregion
        }
        /// <summary>
        /// Xác nhận thanh toán
        /// </summary>
        /// <returns></returns>
        [Route("approve_payment")]
        [HttpPost]
        public async Task<IActionResult> XacNhanThanhToan()
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PBH_BT_THANH_TOAN_XAC_NHAN)
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                return Ok(res);
            }
            if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                return NotFound();
            BaseResponse<dynamic> response = new BaseResponse<dynamic>();
            response.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header); ;
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                var headerClone = header.Clone();
                headerClone.action = ESCSStoredProcedures.PBH_BT_THANH_TOAN_XAC_NHAN_LKE_TICH_HOP;
                var dsHoSo = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerClone);
                if (dsHoSo!=null)
                {
                    List<ho_so_thanh_toan> dsach = JsonConvert.DeserializeObject<List<ho_so_thanh_toan>>(JsonConvert.SerializeObject(dsHoSo));
                    if (dsach != null && dsach.Count > 0)
                    {
                        foreach (var hs in dsach)
                        {
                            try
                            {
                                BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                                var headerCloneTichHop = header.Clone();
                                headerCloneTichHop.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE_OPES;
                                var dataTichHop = new Dictionary<string, object>();
                                dataTichHop.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                                dataTichHop.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                                dataTichHop.AddWithExists("nsd", data_info.GetString("nsd"));
                                dataTichHop.AddWithExists("pas", data_info.GetString("pas"));
                                dataTichHop.AddWithExists("so_id", hs.so_id_hs);
                                dataTichHop.AddWithExists("hanh_dong", "CHUYEN_DU_LIEU_BT");
                                resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(dataTichHop, headerCloneTichHop);
                                DuLieuBoiThuongOpes duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongOpes>(JsonConvert.SerializeObject(resData.data_info));
                                string json = await ApiGatewayOPES.CallApiChuyenDLBoiThuong(ma_doi_tac_nsd, duLieuBT);
                                AddLog(header, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                            }
                            catch { }
                        }
                    }
                }
                
            }
            return Ok(response);
        }
        /// <summary>
        /// Hủy xác nhận thanh toán
        /// </summary>
        /// <returns></returns>
        [Route("unapprove_payment")]
        [HttpPost]
        public async Task<IActionResult> HuyXacNhanThanhToan()
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
            if (action == null || action.actionapicode != ESCSStoredProcedures.PBH_BT_THANH_TOAN_HUY_XAC_NHAN)
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                res.state_info.message_body = "Hành động không hợp lệ hoặc không được phân quyền sử dụng.";
                return Ok(res);
            }
            if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                return NotFound();
            BaseResponse<dynamic> response = new BaseResponse<dynamic>();
            response.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header); ;
            string ma_doi_tac_nsd = data_info.GetString("ma_doi_tac_nsd");
            string nsd = data_info.GetString("nsd");
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (AppSettings.ConnectApiCorePartner && config != null && config.Partner == CoreApiConfigContants.OPES)
            {
                var headerClone = header.Clone();
                headerClone.action = ESCSStoredProcedures.PBH_BT_THANH_TOAN_XAC_NHAN_LKE_TICH_HOP;
                var dsHoSo = await _dynamicService.ExcuteDynamicNewAsync(data_info, headerClone);
                if (dsHoSo != null)
                {
                    List<ho_so_thanh_toan> dsach = JsonConvert.DeserializeObject<List<ho_so_thanh_toan>>(JsonConvert.SerializeObject(dsHoSo));
                    if (dsach != null && dsach.Count > 0)
                    {
                        foreach (var hs in dsach)
                        {
                            try
                            {
                                BaseResponse<dynamic> resData = new BaseResponse<dynamic>();
                                var headerCloneTichHop = header.Clone();
                                headerCloneTichHop.action = ESCSStoredProcedures.PTICH_HOP_DL_BOI_THUONG_XE_OPES;
                                var dataTichHop = new Dictionary<string, object>();
                                dataTichHop.AddWithExists("ma_doi_tac_nsd", data_info.GetString("ma_doi_tac_nsd"));
                                dataTichHop.AddWithExists("ma_chi_nhanh_nsd", data_info.GetString("ma_chi_nhanh_nsd"));
                                dataTichHop.AddWithExists("nsd", data_info.GetString("nsd"));
                                dataTichHop.AddWithExists("pas", data_info.GetString("pas"));
                                dataTichHop.AddWithExists("so_id", hs.so_id_hs);
                                dataTichHop.AddWithExists("hanh_dong", "CHUYEN_DU_LIEU_BT");
                                resData.data_info = await _dynamicService.ExcuteDynamicNewAsync(dataTichHop, headerCloneTichHop);
                                DuLieuBoiThuongOpes duLieuBT = JsonConvert.DeserializeObject<DuLieuBoiThuongOpes>(JsonConvert.SerializeObject(resData.data_info));
                                string json = await ApiGatewayOPES.CallApiChuyenDLBoiThuong(ma_doi_tac_nsd, duLieuBT);
                                AddLog(header, ma_doi_tac_nsd, nsd, "CHUYEN_DU_LIEU_BT", JsonConvert.SerializeObject(duLieuBT), json);
                            }
                            catch { }
                        }
                    }
                }

            }
            return Ok(response);
        }
        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("get-list-video")]
        [HttpPost]
        public async Task<IActionResult> GetListVideo()
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
            BaseResponse<IEnumerable<bh_file_video>> resLink = new BaseResponse<IEnumerable<bh_file_video>>();
            resLink.data_info = await _dynamicService.ExcuteListAsync<bh_file_video>(data_info, header);
            if (resLink.data_info!=null && resLink.data_info.Count() > 0)
            {
                foreach (var item in resLink.data_info)
                {
                    var access_token = item.ma_doi_tac_nsd + "|" + item.ma_chi_nhanh_nsd + "|" + item.nsd + "|" + item.pas + "|" + item.bt + "|" + DateTime.Now.AddDays(1).ToString("yyyyMMddHHmmss");
                    item.link_video = "/api/carclaim/video-mobile/" + Utilities.Base64UrlEncode(Utilities.EncryptByKey(access_token, AppSettings.KeyEryptData));
                    item.ma_doi_tac_nsd = "";
                    item.ma_chi_nhanh_nsd = "";
                    item.nsd = "";
                    item.pas = "";
                }    
            }
            return Ok(resLink);
            #endregion
        }

        /// <summary>
        /// Excute
        /// </summary>
        /// <returns></returns>
        [Route("estimate-money")]
        [HttpPost]
        public async Task<IActionResult> LayUocDPGoiY()
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
            if (action == null|| action.actionapicode != ESCSStoredProcedures.PBH_BT_XE_UOC_TON_THAT_TU_DONG)
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
            res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header, outValue =>{ res.out_value = outValue; });
            return Ok(res);
            #endregion
        }

        [HttpGet]
        [Route("video-mobile/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<FileResult> VideoMobile(string token)
        {
            HeaderRequest header = Request.GetHeader();
            header.action = ESCSStoredProcedures.PBH_FILE_VIDEO_LKE_CT;
            string ma_doi_tac_nsd ="", ma_chi_nhanh_nsd = "", nsd = "", pas = "", id = "", timelive = "";
            token = Utilities.DecryptByKey(Utilities.Base64UrlDecode(token), AppSettings.KeyEryptData);
            if (string.IsNullOrEmpty(token))
                throw new Exception("Cấm xâm nhập");
            var arr = token.Split("|");
            if (arr == null && arr.Count() != 6)
                throw new Exception("Cấm xâm nhập");
            ma_doi_tac_nsd = arr[0];
            ma_chi_nhanh_nsd = arr[1];
            nsd = arr[2];
            pas = arr[3];
            id = arr[4];
            timelive = arr[5];
            long curentDate = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (curentDate > Int64.Parse(timelive))
                throw new Exception("Link video hết hiệu lực");

            var keyCacheVideo = "DANH_SACH_VIDEO:VIDEO" + id;
            var duong_dan = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCacheVideo, RedisCacheMaster.DatabaseIndex);
            if (string.IsNullOrEmpty(duong_dan))
            {
                header.envcode = "DEV";
                var action = _dynamicService.GetConnection(header);
                if (action == null)
                    throw new Exception("Hành động không hợp lệ hoặc không được phân quyền sử dụng.");
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return null;

                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                var data_info = new Dictionary<string, object>();
                data_info.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd);
                data_info.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd);
                data_info.AddWithExists("nsd", nsd);
                data_info.AddWithExists("pas", pas);
                data_info.AddWithExists("bt", id);

                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                if (res.data_info == null)
                    throw new Exception("Không xác định được file");
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    throw new Exception("Không xác định được file");
                bh_file_video file = JsonConvert.DeserializeObject<bh_file_video>(jsonRes);
                duong_dan = file.duong_dan;
                _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCacheVideo, duong_dan, DateTime.Now.AddMinutes(1440) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }

            string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, duong_dan).ChuanHoaDuongDanRemote();
            if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                fileName = @"\" + fileName;

            return PhysicalFile(fileName, contentType: "application/octet-stream", enableRangeProcessing: true);
        }
        [HttpGet]
        [Route("video/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<FileResult> Video(string id)
        {
            HeaderRequest header = Request.GetHeader();
            Request.Headers.TryGetValue("ma_doi_tac_nsd", out var ma_doi_tac_nsd);
            Request.Headers.TryGetValue("ma_chi_nhanh_nsd", out var ma_chi_nhanh_nsd);
            Request.Headers.TryGetValue("nsd", out var nsd);
            Request.Headers.TryGetValue("pas", out var pas);

            if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(ma_chi_nhanh_nsd) || 
                string.IsNullOrEmpty(nsd) || string.IsNullOrEmpty(pas))
                throw new Exception("Cấm xâm nhập");

            var keyCacheVideo = "DANH_SACH_VIDEO:VIDEO" + id;
            var duong_dan = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCacheVideo, RedisCacheMaster.DatabaseIndex);
            if (string.IsNullOrEmpty(duong_dan))
            {
                header.envcode = "DEV";
                var action = _dynamicService.GetConnection(header);
                if (action == null)
                    throw new Exception("Hành động không hợp lệ hoặc không được phân quyền sử dụng.");
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return null;

                BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                var data_info = new Dictionary<string, object>();
                data_info.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd.ToString());
                data_info.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd.ToString());
                data_info.AddWithExists("nsd", nsd.ToString());
                data_info.AddWithExists("pas", pas.ToString());
                data_info.AddWithExists("bt", id);

                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                if (res.data_info == null)
                    throw new Exception("Không xác định được file");
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    throw new Exception("Không xác định được file");
                bh_file_video file = JsonConvert.DeserializeObject<bh_file_video>(jsonRes);
                duong_dan = file.duong_dan;
                _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCacheVideo, duong_dan, DateTime.Now.AddMinutes(1440) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }

            string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, duong_dan).ChuanHoaDuongDanRemote();
            if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                fileName = @"\" + fileName;

            return PhysicalFile(fileName, contentType: "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpGet]
        [Route("video-link/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VideoLink(string id)
        {
            HeaderRequest header = Request.GetHeader();
            Request.Headers.TryGetValue("ma_doi_tac_nsd", out var ma_doi_tac_nsd);
            Request.Headers.TryGetValue("ma_chi_nhanh_nsd", out var ma_chi_nhanh_nsd);
            Request.Headers.TryGetValue("nsd", out var nsd);
            Request.Headers.TryGetValue("pas", out var pas);

            if (string.IsNullOrEmpty(ma_doi_tac_nsd) || string.IsNullOrEmpty(ma_chi_nhanh_nsd) ||
                string.IsNullOrEmpty(nsd) || string.IsNullOrEmpty(pas))
                throw new Exception("Cấm xâm nhập");

            var keyCacheVideo = "DANH_SACH_VIDEO_LINK_MP4:VIDEO" + id;
            var duong_dan = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCacheVideo, RedisCacheMaster.DatabaseIndex);
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            if (string.IsNullOrEmpty(duong_dan))
            {
                header.envcode = "DEV";
                var action = _dynamicService.GetConnection(header);
                if (action == null)
                    throw new Exception("Hành động không hợp lệ hoặc không được phân quyền sử dụng.");
                if ((AppSettings.Internal == "PRIVATE" && action.is_internal != "PRIVATE") ||
                    (AppSettings.Internal == "PUBLIC" && action.is_internal != "PUBLIC"))
                    return NotFound();

                var data_info = new Dictionary<string, object>();
                data_info.AddWithExists("ma_doi_tac_nsd", ma_doi_tac_nsd.ToString());
                data_info.AddWithExists("ma_chi_nhanh_nsd", ma_chi_nhanh_nsd.ToString());
                data_info.AddWithExists("nsd", nsd.ToString());
                data_info.AddWithExists("pas", pas.ToString());
                data_info.AddWithExists("bt", id);

                res.data_info = await _dynamicService.ExcuteDynamicNewAsync(data_info, header);
                if (res.data_info == null)
                    throw new Exception("Không xác định được file");
                string jsonRes = JsonConvert.SerializeObject(res.data_info, new JsonSerializerSettings() { ContractResolver = OpenIDConfig.LowercaseContractResolver, MaxDepth = int.MaxValue });
                if (string.IsNullOrEmpty(jsonRes))
                    throw new Exception("Không xác định được file");
                bh_file_video file = JsonConvert.DeserializeObject<bh_file_video>(jsonRes);
                duong_dan = file.duong_dan;
                _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCacheVideo, duong_dan, DateTime.Now.AddMinutes(1440) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }

            string fileName = Path.Combine(AppSettings.PathFolderNotDeleteFull, duong_dan).ChuanHoaDuongDanRemote();
            if (AppSettings.FolderSharedUsed && fileName.StartsWith(@"\") && !fileName.StartsWith(@"\\"))
                fileName = @"\" + fileName;

            var timeTemp = DateTime.Now.ToString("yyyyMMddHH");
            var pathCopy = Path.Combine("public", timeTemp);
            string copyToPath = Path.Combine(_env.WebRootPath, pathCopy);
            if (!System.IO.Directory.Exists(copyToPath))
                System.IO.Directory.CreateDirectory(copyToPath);
            string filenameNew = Path.GetFileName(fileName);
            var link = Path.Combine(pathCopy, filenameNew);
            if (!System.IO.File.Exists(Path.Combine(_env.WebRootPath, link)))
                System.IO.File.Copy(fileName, Path.Combine(_env.WebRootPath, link));
            res.data_info = "/" + link.ChuanHoaLink();
            return Ok(res);
        }

        #region Private method
        /// <summary>
        /// Gửi thông báo tách luồng
        /// </summary>
        /// <param name="action"></param>
        /// <param name="headerBase"></param>
        /// <param name="data_info_base"></param>
        private void SendNotifyHospital(ActionConnection action, HeaderRequest headerBase, IDictionary<string, object> data_info_base)
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
                        header.action = ESCSStoredProcedures.PHT_THONG_BAO_BV_GUI;
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
                                    var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(true);
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
                                if (chanel=="SMS")
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
                        catch(Exception ex) {
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
        /// Chung login api
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<BaseResponse<authentication>> LoginApi(account user, bool isHash = true)
        {
            BaseResponse<authentication> res = new BaseResponse<authentication>();
            if (user == null)
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Bạn chưa nhập tài khoản và mật khẩu.";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH001;
                return res;
            }
            if (string.IsNullOrEmpty(user.username))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Bạn chưa nhập tài khoản.";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH002;
                return res;
            }
            if (string.IsNullOrEmpty(user.password))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Bạn chưa nhập mật khẩu.";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH003;
                return res;
            }
            Request.Headers.TryGetValue("ePartnerCode", out var vbi_partner_code);
            if (string.IsNullOrEmpty(vbi_partner_code.ToString()))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Bạn chưa nhập mã đối tác [ePartnerCode].";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH004;
                return res;
            }
            //Chưa kiểm tra chữ ký trước khi login (sử dụng JWT để kiểm tra chữ ký dữ liệu)
            Request.Headers.TryGetValue("eSignature", out var vbi_signature);
            string pass = isHash ? Utilities.Sha256Hash(user.password) : user.password;
            string keyCache = CachePrefixKeyConstants.GetKeyCachePartnerPublic(vbi_partner_code.ToString(), user.username);
            string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            sys_partner_cache userdb = null;
            if (string.IsNullOrEmpty(json))
            {
                BaseRequest rq = new BaseRequest();
                rq.data_info = new Dictionary<string, string>();
                rq.data_info.Add("partner_code", vbi_partner_code.ToString());
                rq.data_info.Add("authen", "authen");
                rq.data_info.Add("api_username", user.username);
                rq.data_info.Add("api_password", pass);
                rq.data_info.Add("token", "");
                rq.data_info.Add("cat_partner", "PUBLIC");
                var obj = await _openIdService.GetPartnerWithConfig(rq);
                if (obj == null)
                {
                    res.state_info.status = STATUS_NOTOK;
                    res.state_info.message_body = "Không tìm thấy thông tin đăng nhập";
                    res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH005;
                    return res;
                }
                string jsonRes = JsonConvert.SerializeObject(obj);
                userdb = JsonConvert.DeserializeObject<sys_partner_cache>(jsonRes);
                _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(userdb), DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }
            else
            {
                userdb = JsonConvert.DeserializeObject<sys_partner_cache>(json);
            }
            string jsonLockAccount = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, CachePrefixKeyConstants.GetKeyCacheAccountLock(vbi_partner_code.ToString(), user.username), RedisCacheMaster.DatabaseIndex);
            if (!string.IsNullOrEmpty(jsonLockAccount))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Tài khoản của bạn tạm khóa. Vui lòng thử lại sau ít phút.";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH007;
                return res;
            }
            if (userdb.config_isactive.Value == AccountConstants.LOCK)
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với quản trị viên.";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH007;
                return res;
            }
            if (userdb.config_password != pass)
            {
                int count = 0;
                string countErrorLogin = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, CachePrefixKeyConstants.GetKeyCacheLoginErrorCount(vbi_partner_code.ToString(), user.username), RedisCacheMaster.DatabaseIndex);
                if (!string.IsNullOrEmpty(countErrorLogin))
                {
                    count = Convert.ToInt32(countErrorLogin);
                }
                count++;
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_body = "Mật khẩu không chính xác (" + count + ")";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH006;

                if (count == 3)
                {
                    res.state_info.message_body = "Nhập sai mật khẩu nhiều lần, tài khoản bị tạm khóa 5 phút. Vui lòng thử lại sau.";
                    res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH007;
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheAccountLock(vbi_partner_code.ToString(), user.username), count.ToString(), new TimeSpan(0, 5, 0), RedisCacheMaster.DatabaseIndex);
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheLoginErrorCount(vbi_partner_code.ToString(), user.username), count.ToString(), new TimeSpan(0, 10, 0), RedisCacheMaster.DatabaseIndex);
                    return res;
                }
                if (count == 4)
                {
                    res.state_info.message_body = "Nhập sai mật khẩu nhiều lần, tài khoản bị tạm khóa 10 phút. Vui lòng thử lại sau.";
                    res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH007;
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheAccountLock(vbi_partner_code.ToString(), user.username), count.ToString(), new TimeSpan(0, 10, 0), RedisCacheMaster.DatabaseIndex);
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheLoginErrorCount(vbi_partner_code.ToString(), user.username), count.ToString(), new TimeSpan(0, 15, 0), RedisCacheMaster.DatabaseIndex);
                    return res;
                }
                if (count == 5)
                {
                    res.state_info.message_body = "Nhập sai mật khẩu nhiều lần, tài khoản bị tạm khóa 15 phút. Vui lòng thử lại sau.";
                    res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH007;
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheAccountLock(vbi_partner_code.ToString(), user.username), count.ToString(), new TimeSpan(0, 15, 0), RedisCacheMaster.DatabaseIndex);
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheLoginErrorCount(vbi_partner_code.ToString(), user.username), count.ToString(), new TimeSpan(0, 20, 0), RedisCacheMaster.DatabaseIndex);
                    return res;
                }
                if (count > 5)
                {
                    BaseRequest rqLock = new BaseRequest();
                    rqLock.data_info = new Dictionary<string, string>();
                    rqLock.data_info.Add("partner_code", vbi_partner_code.ToString());
                    rqLock.data_info.Add("username", user.username);
                    await _openIdService.LogAccount(rqLock);
                    res.state_info.message_body = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với quản trị viên.";
                    res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH007;
                    userdb.config_isactive = AccountConstants.LOCK;
                    _cacheServer.Remove(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheLoginErrorCount(vbi_partner_code.ToString(), user.username), RedisCacheMaster.DatabaseIndex);
                    _cacheServer.Remove(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, CachePrefixKeyConstants.GetKeyCacheAccountLock(vbi_partner_code.ToString(), user.username), RedisCacheMaster.DatabaseIndex);
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(userdb), DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                }
                return res;
            }
            else
            {
                long timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                long timeExpriveAccessToken = Convert.ToInt64(DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveAccessTokenMinute).ToString("yyyyMMddHHmmss"));
                long timeExpriveRefreshToken = 0;
                var jsonPayload = new TokenPayload()
                {
                    partner_code = vbi_partner_code.ToString().ToUpper(),
                    envcode = userdb.config_envcode,
                    username = user.username,
                    password = pass,
                    time_exprive = timeExpriveAccessToken,
                    time_begin_session = timeNow
                };
                var header = JWTHelper.GetTokenPublicHeader();
                var payload = JWTHelper.GetTokenPublicPayload(jsonPayload);
                string access_token = JWTHelper.GetToken(header, payload, userdb.config_secret_key);
                authentication auth = new authentication();
                auth.access_token = access_token;
                auth.environment = userdb.config_envcode;
                auth.time_exprive = timeExpriveAccessToken;
                auth.time_connect = timeNow;
                auth.refesh_token = "";
                if (userdb.config_session_time_live != null && userdb.config_session_time_live > OpenIDConfig.TimeLiveAccessTokenMinute)
                {
                    timeExpriveRefreshToken = Convert.ToInt64(DateTime.Now.AddMinutes(userdb.config_session_time_live.Value).ToString("yyyyMMddHHmmss"));
                    openid_refresh_token openidRefreshToken = new openid_refresh_token();
                    openidRefreshToken.access_token = access_token;
                    openidRefreshToken.partner_code = vbi_partner_code.ToString().ToUpper();
                    openidRefreshToken.time_exprive = timeExpriveRefreshToken;
                    var payloadRefresh = JWTHelper.GetTokenPublicPayload(openidRefreshToken);
                    string refreshToken = JWTHelper.GetToken(header, payloadRefresh, userdb.config_secret_key);
                    auth.refesh_token = refreshToken;
                }
                res.data_info = auth;
                res.state_info.status = STATUS_OK;
                res.state_info.message_body = "Đăng nhập thành công.";
                res.state_info.message_code = ErrorCodeOpenIDConstants.AUTH000;
            }
            return res;
        }
        /// <summary>
        /// Cấp lại token mới
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        private BaseResponse<authentication> RefreshTokenApi(authentication auth)
        {
            BaseResponse<authentication> res = new BaseResponse<authentication>();
            HeaderRequest header = Request.GetHeader();
            if (auth == null || string.IsNullOrEmpty(auth.access_token) || string.IsNullOrEmpty(auth.refesh_token) || string.IsNullOrEmpty(auth.token_type) || header == null || string.IsNullOrEmpty(header.partner_code))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = NOTFOUND;
                res.state_info.message_body = "Thiếu thông tin cần thiết.";
                return res;
            }
            openid_access_token token = null;
            header.token = auth.access_token;
            string keyCachePartner = string.Empty;
            var vetifyTokenMessage = VetifyToken(header, out token, out keyCachePartner, false, false);
            if (!string.IsNullOrEmpty(vetifyTokenMessage))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyTokenMessage;
                return res;
            }
            authentication authNew = null;
            var vetifyRefreshTokenMessage = VetifyRefreshToken(header, auth, keyCachePartner, out authNew);
            if (!string.IsNullOrEmpty(vetifyRefreshTokenMessage))
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_CONN001;
                res.state_info.message_body = vetifyRefreshTokenMessage;
                return res;
            }
            res.data_info = authNew;
            res.state_info.status = STATUS_OK;
            res.state_info.message_code = SUCCESS;
            res.state_info.message_body = "Cấp token mới thành công.";
            return res;
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
        /// <summary>
        /// Gửi Email tách luồng
        /// </summary>
        /// <param name="action"></param>
        /// <param name="headerBase"></param>
        /// <param name="data_info_base"></param>
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
        #endregion
    }
}