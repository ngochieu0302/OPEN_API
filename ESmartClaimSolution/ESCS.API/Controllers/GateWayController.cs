using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESCS.API.Hubs;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.AI;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Http;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.MODEL.ESCS;
using ESCS.MODEL.OpenID.ModelView;
using FirebaseAdmin.Messaging;
using Google.Cloud.Vision.V1;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ESCS.API.Controllers
{
    [Route("api/esmartclaim/gateway")]
    [ApiController]
    public class GateWayController : BaseController
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogMongoService<LogSendNotify> _logNotify;
        private readonly ILogMongoService<LogContent> _logContent;
        /// <summary>
        /// ServiceController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="dynamicService"></param>
        /// <param name="openIdCacheService"></param>
        public GateWayController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider,
            IUserConnectionManager userConnectionManager,
            ILogMongoService<LogSendNotify> logNotify,
            ILogMongoService<LogContent> logContent)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _userConnectionManager = userConnectionManager;
            _logNotify = logNotify;
            _logContent = logContent;
        }
        [HttpPost]
        [Route("send-notify")]
        public async Task<IActionResult> SendNotifyFirebase(MulticastMessage message)
        {
            BaseResponse<List<string>> res = new BaseResponse<List<string>>();
            Request.Headers.TryGetValue("eToken", out var token);
            if (token != ApiGatewayConfig.TokenCommon)
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = "UNAUTHEN";
                res.state_info.message_body = "Token không hợp lệ";
                return Ok(res);
            }
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(true);
                res.state_info.status = STATUS_OK;
                res.state_info.message_code = "SEND_SUCCESS";
                res.state_info.message_body = "Gửi notify thành công";
                if (response.FailureCount > 0)
                {
                    var failedTokens = new List<string>();
                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                        {
                            failedTokens.Add(message.Tokens[i]);
                        }
                    }
                    res.state_info.message_code = "SEND_ERROR";
                    res.state_info.message_body = "Có gửi lỗi notify";
                    res.data_info = failedTokens;
                    //_logContent.AddLogAsync(new LogContent("Danh sách gửi notify firebase lỗi: " + JsonConvert.SerializeObject(failedTokens)));
                }
            }
            catch
            {
                res.state_info.status = STATUS_NOTOK;
                res.state_info.message_code = "SEND_EXCEPTION";
                res.state_info.message_body = "Đã có lỗi xảy ra";
                //_logContent.AddLogAsync(new LogContent("Gửi notify firebase lỗi (Exception): " + JsonConvert.SerializeObject(message)));
            }
            return Ok(res);
        }
        [HttpPost]
        [Route("crop-image")]
        public async Task<IActionResult> CropImage()
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
            if (!Utilities.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest();
            }
            IFormFileCollection files;
            var rqData = Request.GetFormDataRequest(out files);
            if (files.Count<=0)
            {
                BaseResponse<dynamic> resErr001 = new BaseResponse<dynamic>();
                resErr001.state_info.status = STATUS_NOTOK;
                resErr001.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                resErr001.state_info.message_body = "Không tìm thấy file upload";
                return Ok(resErr001);
            }
            string strJson = rqData.data;
            BaseRequest<crop_image> crop = JsonConvert.DeserializeObject<BaseRequest<crop_image>>(strJson);

            HeaderRequest headerAI = header.Clone();
            headerAI.action = ESCSStoredProcedures.PBH_DICH_VU_KIEM_TRA;
            IDictionary<string, object> kiemTraDichVu = new Dictionary<string, object>();
            kiemTraDichVu.Add("ma_doi_tac_nsd", crop.data_info.ma_doi_tac_nsd);
            kiemTraDichVu.Add("ma_chi_nhanh_nsd", crop.data_info.ma_chi_nhanh_nsd??"");
            kiemTraDichVu.Add("nsd", crop.data_info.nsd);
            kiemTraDichVu.Add("pas", crop.data_info.pas);
            kiemTraDichVu.Add("ma_dich_vu", "DICH_VU_AI");
            var cauHinh = await _dynamicService.ExcuteDynamicNewAsync(kiemTraDichVu, headerAI);
            string json = JsonConvert.SerializeObject(cauHinh);
            bh_dich_vu<bh_dich_vu_ai> cauHinhDV = JsonConvert.DeserializeObject<bh_dich_vu<bh_dich_vu_ai>>(json);
            var dich_vu = cauHinhDV.dich_vu;
            if (dich_vu == null || dich_vu.ap_dung == 0)
            {
                BaseResponse<dynamic> resErr001 = new BaseResponse<dynamic>();
                resErr001.state_info.status = STATUS_NOTOK;
                resErr001.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                resErr001.state_info.message_body = "Chưa cấu hình dịch hoặc dịch vụ không được áp dụng.";
                return Ok(resErr001);
            }
            BaseResponse<dynamic> res = new BaseResponse<dynamic>();
            using (var ms = new MemoryStream())
            {
                files[0].CopyTo(ms);
                var image_crop = ms.ToArray();
                var result = await OCRService.CropImage(dich_vu.base_url, dich_vu.api_key, image_crop, "1");
                res.data_info = result;
            }
            return Ok(res);
        }

        [HttpPost]
        [Route("google-crop-image")]
        public IActionResult GoogleCropImage()
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
            if (!Utilities.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest();
            }
            IFormFileCollection files;
            var rqData = Request.GetFormDataRequest(out files);
            if (files.Count <= 0)
            {
                BaseResponse<dynamic> resErr001 = new BaseResponse<dynamic>();
                resErr001.state_info.status = STATUS_NOTOK;
                resErr001.state_info.message_code = ErrorCodeOpenIDConstants.ERROR_ACTION_NOTFOUND;
                resErr001.state_info.message_body = "Không tìm thấy file upload";
                return Ok(resErr001);
            }
            string strJson = rqData.data;
            BaseRequest<crop_image> crop = JsonConvert.DeserializeObject<BaseRequest<crop_image>>(strJson);
            using (var ms = new MemoryStream())
            {
                files[0].CopyTo(ms);
                Image imageCrop = Image.FromStream(ms);
            }
            return Ok();
        }
    }
}
