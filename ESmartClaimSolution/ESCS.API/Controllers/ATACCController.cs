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
using ESCS.MODEL.ATACC;
using ESCS.MODEL.ATACC.MOBILE;

/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    /// <summary>
    /// Excute stored procedure
    /// </summary>
    [Route("api/atacc")]
    [ApiController]
    public class ATACCController : BaseController
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string PARTNER_CODE = "ESCS_MOBILE_CUSTOMER";
        private readonly bool CHECKSUM = false;
        /// <summary>
        /// ServiceController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public ATACCController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider,
            IUserConnectionManager userConnectionManager,
            IWebHostEnvironment webHostEnvironment)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _userConnectionManager = userConnectionManager;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// API đẩy thông tin bồi thường sang cho ABIC - Lấy theo ngày tạo thông tin hồ sơ
        /// </summary>
        /// <returns></returns>
        [Route("get_data_hsbt")]
        [HttpPost]
        public async Task<IActionResult> GetDataHSBT(rq_push_data_abic_create model)
        {
            HeaderRequest header = Request.GetHeader();
            if (model.hash != AppSettings.HashABIC)
            {
                data_abic_create reject = new data_abic_create();
                reject.RspCode = "99";
                reject.Message = "ERROR";
                reject.Result = "Hashcode không đúng!";
                reject.data = null;
                return BadRequest(reject);
            }
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var headerGateway = header.Clone();
            headerGateway.action = ESCSStoredProcedures.P_ESCS_ATACC_PUSH_DATA_ABIC_CREATE;
            try { 
                var response = await _dynamicService.ExcuteListAsync<res_push_data_abic_create>(dataRequest, headerGateway);
                data_abic_create result = new data_abic_create();
                result.RspCode = "00";
                result.Message = "SUCCESS";
                result.Result = null;
                result.data = response;
                return Ok(result);
            } catch(Exception ex)
            {
                data_abic_create reject = new data_abic_create();
                reject.RspCode = "99";
                reject.Message = "ERROR";
                reject.Result = ex.Message ;
                reject.data = null;
                return BadRequest(reject);
            };
        }
        /// <summary>
        /// API đẩy thông tin bồi thường sang cho MIC
        /// </summary>
        /// <returns></returns>
        [Route("get_data_hsbt_mic")]
        [HttpPost]
        public async Task<IActionResult> GetDataHSBTMIC(rq_push_data_mic model)
        {
            HeaderRequest header = Request.GetHeader();
            res_push_data_mic res = new res_push_data_mic();
            if (model.hash != AppSettings.HashMIC)
            {
                data_mic reject = new data_mic();
                reject.RspCode = "99";
                reject.Message = "ERROR";
                reject.Result = "Hashcode không đúng!";
                reject.data = null;
                return BadRequest(reject);
            }
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var headerGateway = header.Clone();
            headerGateway.action = ESCSStoredProcedures.P_ESCS_ATACC_PUSH_DATA_MIC;
            try
            {
                var response = await _dynamicService.ExcuteListAsync<res_push_data_mic>(dataRequest, headerGateway);
                data_mic result = new data_mic();
                result.RspCode = "00";
                result.Message = "SUCCESS";
                result.Result = null;
                result.data = response;
                return Ok(result);
            }
            catch (Exception ex)
            {
                data_mic reject = new data_mic();
                reject.RspCode = "99";
                reject.Message = "ERROR";
                reject.Result = ex.Message;
                reject.data = null;
                return BadRequest(reject);
            };
        }
        //------------------------------------------------------------------------------------
        [Route("mobile/getVersionApp")]
        [HttpGet]
        public async Task<IActionResult> GetVersionApp(string platformDevice, string appName)
        {
            atacc_response_v4<string> res = new atacc_response_v4<string>();
            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_VERSION;
            Dictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("ma_app", appName); 
            dataRequest.AddWithExists("platform", platformDevice);
            try
            {
                var response = await _dynamicService.ExcuteSingleAsync<atacc_version_app>(dataRequest, header);
                res.data = response.version_app;
                res.Message = "Get android version success: " + response.version_app;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
            }
            return Ok(res);
        }
        /// <summary>
        /// Đăng ký App Mobile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/1")]
        [HttpPost]
        public async Task<IActionResult> DangKy(dang_ky model)
        {
            atacc_response<string> res = new atacc_response<string>();
            if (!ATACCUtils.XacThucDangKy(model, out string checksum) && CHECKSUM)
            {
                res.resultmessage = "Chữ ký (checksum) không hợp lệ.";
                return Ok(res);
            }    

            model.mat_khau = Utilities.Sha256Hash(model.mat_khau);
            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_DANG_KY;
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            try
            {
                var response = await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);
            }
            catch(Exception ex)
            {
                res.resultmessage = ex.Message;
            }
            return Ok(res);
        }
        /// <summary>
        /// Đăng nhập App Mobile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/login")]
        [HttpPost]
        public async Task<IActionResult> DangNhap(dang_nhap model)
        {
            atacc_response<IEnumerable<dang_nhap_result>> res = new atacc_response<IEnumerable<dang_nhap_result>>();
            if (!ATACCUtils.XacThucDangNhap(model, out string checksum) && CHECKSUM)
            {
                res.resultmessage = "Chữ ký (checksum) không hợp lệ.";
                return Ok(res);
            }

            model.pass = Utilities.Sha256Hash(model.pass);
            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_DANG_NHAP;
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            try
            {
                var response = await _dynamicService.ExcuteListAsync<dang_nhap_result>(dataRequest, header);
                res.resultlist = response;
            }   
            catch(Exception ex)
            {
                res.resultmessage = ex.Message;
            }

            return Ok(res);
        }
        /// <summary>
        /// Đăng nhập App Mobile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/logout")]
        [HttpPost]
        public async Task<IActionResult> DangXuat(dang_xuat model)
        {
            atacc_response<string> res = new atacc_response<string>();
            return Ok(res);
        }
        /// <summary>
        /// Tự động đăng nhập
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/2")]
        [HttpPost]
        public async Task<IActionResult> TuDongDangNhap(dang_nhap_tu_dong model)
        {
            atacc_response<IEnumerable<dang_nhap_result>> res = new atacc_response<IEnumerable<dang_nhap_result>>();
            if (!ATACCUtils.XacThucTuDongDangNhap(model, out string checksum) && CHECKSUM)
            {
                res.resultmessage = "Chữ ký (checksum) không hợp lệ.";
                return Ok(res);
            }

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_TU_DONG_DANG_NHAP;
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            try
            {
                var response = await _dynamicService.ExcuteListAsync<dang_nhap_result>(dataRequest, header);
                res.resultlist = response;
            }
            catch (Exception ex)
            {
                res.resultmessage = ex.Message;
            }
            return Ok(res);
        }
        /// <summary>
        /// Thay đối mật khẩu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/changePass")]
        [HttpPost]
        public async Task<IActionResult> ThayDoiMatKhau(thay_doi_mat_khau model)
        {
            atacc_response_v3<string> res = new atacc_response_v3<string>();
            if (!ATACCUtils.XacThucThayDoiMatKhau(model, out string checksum) && CHECKSUM)
            {
                res.resultmessage = "Chữ ký (checksum) không hợp lệ.";
                return Ok(res);
            }

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_THAY_DOI_MAT_KHAU;
            model.pass = Utilities.Sha256Hash(model.pass);
            model.new_pass = Utilities.Sha256Hash(model.new_pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            try
            {
                await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);
            }
            catch(Exception ex)
            {
                res.resultmessage = ex.Message;
            }
            return Ok(res);
        }
        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/forgotPass2")]
        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(quen_mat_khau model)
        {
            atacc_response_v2<IEnumerable<quen_mat_khau_result>> res = new atacc_response_v2<IEnumerable<quen_mat_khau_result>>();
            if (!ATACCUtils.XacThucQuyenMatKhau(model, out string checksum) && CHECKSUM)
            {
                res.code = 0;
                res.response_message = "Chữ ký (checksum) không hợp lệ.";
                return Ok(res);
            }

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_QUEN_MAT_KHAU;
            model.new_pass = Utilities.RandomString(8);
            model.new_pass_hash = Utilities.Sha256Hash(model.new_pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            try
            {
                var response = await _dynamicService.ExcuteListAsync<quen_mat_khau_result>(dataRequest, header);
                res.data = response;
            }
            catch(Exception ex)
            {
                res.code = 0;
                res.response_message = ex.Message;
            }

            return Ok(res);
        }
        /// <summary>
        /// Sửa thông tin tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/suaThongTin")]
        [HttpPost]
        public async Task<IActionResult> SuaTTDangKy(sua_tt_dang_ky model)
        {
            atacc_response_v3<string> res = new atacc_response_v3<string>();
            if (!ATACCUtils.XacThucSuaTTDangKy(model, out string checksum) && CHECKSUM)
            {
                res.resultmessage = "Chữ ký (checksum) không hợp lệ.";
                return Ok(res);
            }

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_SUA_TT_TAI_KHOAN;

            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            try
            {
                await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);
            }
            catch (Exception ex)
            {
                res.resultmessage = ex.Message;
            }

            return Ok(res);
        }
        /// <summary>
        /// Tìm kiếm Giấy chứng nhận
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/view_hd_kh")]
        [HttpPost]
        public async Task<IActionResult> TimKiemGCN(view_hd_kh model)
        {
            IEnumerable<view_hd_kh_result> res = new List<view_hd_kh_result>();
            if (!ATACCUtils.XacThucDsNguoiThan(model, out string checksum) && CHECKSUM)
                return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_VIEW_HD_KH;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            res = await _dynamicService.ExcuteListAsync<view_hd_kh_result>(dataRequest, header);
            return Ok(res);
        }
        /// <summary>
        /// Xem chi tiết Giấy chứng nhận
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/view_hd_kh_ct")]
        [HttpPost]
        public async Task<IActionResult> XemChiTietGCN(view_hd_kh_ct model)
        {
            IEnumerable<view_hd_kh_ct_result> res = new List<view_hd_kh_ct_result>();
            //if (!ATACCUtils.XacThucQloiChiTiet(model, out string checksum)&& CHECKSUM)
            //    return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_VIEW_HD_KH_CT;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            res = await _dynamicService.ExcuteListAsync<view_hd_kh_ct_result>(dataRequest, header);
            return Ok(res);
        }
        /// <summary>
        /// Khai báo hồ sơ bồi thường con người
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("mobile/kb_hs_btcn")]
        [HttpPost]
        public async Task<IActionResult> KhaiBaoHSBT(kb_hs_btcn model)
        {
            atacc_response_v2<kb_hs_btcn_result> res = new atacc_response_v2<kb_hs_btcn_result>();
            //if (!ATACCUtils.XacThucKhaiBaoHSBT(model, out string checksum) && CHECKSUM)
            //{
            //    res.code = 0;
            //    res.response_message = "Chữ ký (checksum) không hợp lệ.";
            //    return Ok(res);
            //}

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_KB_HS_BTCN;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            try
            {
                var response = await _dynamicService.ExcuteSingleAsync<kb_hs_btcn_result_table>(dataRequest, header);
                kb_hs_btcn_result result = new kb_hs_btcn_result();
                result.Table = new List<kb_hs_btcn_result_table>();
                result.Table.Add(response);
                return Ok(res);
            }
            catch(Exception ex)
            {
                res.code = 0;
                res.response_message = ex.Message;
            }
            return Ok(res);
        }
        


        [Route("p/truy_van_bt_ct")]
        [HttpPost]
        public async Task<IActionResult> XemCtietHSBT(truy_van_bt_ct model)
        {
            atacc_response_v4<truy_van_bt_ct_result> res = new atacc_response_v4<truy_van_bt_ct_result>();
            //if (!ATACCUtils.XacThucXemCtietHSBT(model, out string checksum)&& CHECKSUM)
            //    return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_TRUY_VAN_BT;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var response = await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);

            return Ok(res);
        }

        [Route("mobile/check_ndbh_btcn")]
        [HttpPost]
        public async Task<IActionResult> KtraNDBH(check_ndbh_btcn model)
        {
            atacc_response_v2<check_ndbh_btcn_result> res = new atacc_response_v2<check_ndbh_btcn_result>();
            //if (!ATACCUtils.XacThucKtraNDBH(model, out string checksum)&& CHECKSUM)
            //    return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_CHECK_NDBH_BTCN;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var response = await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);

            return Ok(res);
        }

        [Route("mobile/bshsSendEmail")]
        [HttpPost]
        public async Task<IActionResult> GuiEmailBSHS(bo_sung_hs model)
        {
            atacc_response_v5<string> res = new atacc_response_v5<string>();
            //if (!ATACCUtils.XacThucGuiEmailBSHS(model, out string checksum)&& CHECKSUM)
            //    return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_BSHS_GUI_EMAIL;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var response = await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);

            return Ok(res);
        }

        [Route("mobile/banner_qc")]
        [HttpPost]
        public async Task<IActionResult> BannerQC(banner_qc model)
        {
            List<banner_qc_result> res = new List<banner_qc_result>();
            //if (!ATACCUtils.XacThucBannerQC(model, out string checksum)&& CHECKSUM)
            //    return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_BANNER_QC;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var response = await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);

            return Ok(res);
        }

        [Route("p/truy_van_bt")]
        [HttpPost]
        public async Task<IActionResult> TruyVanBT(truy_van_bt model)
        {
            List<truy_van_bt_result> res = new List<truy_van_bt_result>();
            //if (!ATACCUtils.XacThucTruyVanBT(model, out string checksum)&& CHECKSUM)
            //    return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_TRUY_VAN_BT;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var response = await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);

            return Ok(res);
        }

        [Route("upload_images")]
        [HttpPost]
        public async Task<IActionResult> Upload(atacc_upload model)
        {
            //if (!ATACCUtils.XacThucUpload(model, out string checksum)&& CHECKSUM)
            //    return Unauthorized();

            HeaderRequest header = Request.GetHeader();
            header.partner_code = PARTNER_CODE;
            header.action = ESCSStoredProcedures.P_ESCS_ATACC_UPLOAD;
            model.pass = Utilities.Sha256Hash(model.pass);
            IDictionary<string, object> dataRequest = model.ModelToDictionary();
            var response = await _dynamicService.ExcuteDynamicNewAsync(dataRequest, header);

            return Ok("1");
        }
    }
}