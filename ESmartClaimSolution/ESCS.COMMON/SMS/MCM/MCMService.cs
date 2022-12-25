using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.SMS.MCM
{
    public class MCMService
    {
        public static mcm_request GetRequest(string partner_code, mcm_dich_vu dich_vu, mcm_lich_gui lich_gui, List<mcm_lich_gui_param> lich_gui_param)
        {
            mcm_request rq = new mcm_request();
            rq.ApiKey = dich_vu.api_key;
            rq.SecretKey = dich_vu.secret_key;
            rq.Phone = lich_gui.dthoai;
            rq.Channels = lich_gui.channel.ToLower().Split(",").ToList();
            rq.Data = new List<object>();
            mcm_request_zalo zalo = new mcm_request_zalo()
            {
                OAID = dich_vu.oaid,
                TempID = lich_gui.tempid,
                Params = lich_gui_param.Where(n => n.loai == "ZALO").OrderBy(n => n.stt).Select(n => n.gia_tri).ToList(),
                campaignid = lich_gui.ten_chien_dich,
                CallbackUrl = MCMConfiguration.CallbackUrl+"/"+ partner_code,
            };
            mcm_request_sms sms = new mcm_request_sms()
            {
                Content = lich_gui.nd_sms,
                IsUnicode = 0,
                SmsType = 2,
                Brandname = dich_vu.brandname,
                RequestId = lich_gui.bt.ToString(),
                campaignid = lich_gui.ten_chien_dich,
                CallbackUrl = MCMConfiguration.CallbackUrl + "/" + partner_code,
            };
            var param_sms = lich_gui_param.Where(n => n.loai == "SMS").ToList();
            if (param_sms != null && param_sms.Count() > 0)
                foreach (var item in param_sms)
                    sms.Content = sms.Content.Replace("{{" + item.param_code.Trim() + "}}", item.gia_tri);

            rq.Data.Add(zalo);
            rq.Data.Add(sms);
            return rq;
        }
        public static mcm_request_sms_single GetRequestSMS(string partner_code, mcm_dich_vu dich_vu, mcm_lich_gui lich_gui, List<mcm_lich_gui_param> lich_gui_param)
        {
            mcm_request_sms_single rq = new mcm_request_sms_single();
            rq.ApiKey = dich_vu.api_key;
            rq.SecretKey = dich_vu.secret_key;
            rq.Phone = lich_gui.dthoai;

            rq.Content = lich_gui.nd_sms;
            rq.IsUnicode = 0;
            rq.SmsType = 2;
            rq.Brandname = dich_vu.brandname;
            rq.RequestId = lich_gui.bt.ToString();
            rq.campaignid = lich_gui.ten_chien_dich;
            rq.CallbackUrl = MCMConfiguration.CallbackUrl + "/" + partner_code;
            return rq;
        }
        /// <summary>
        ///100 Request đã được nhận và xử lý thành công.
        ///101 Sai ApiKey hoặc ScretKey
        ///103 Tài khoản không đủ tiền gửi tin
        ///104 Brandname không tồn tại hoặc đã bị hủy
        ///118 Loại tin nhắn không hợp lệ
        ///119 Sai TempID
        ///99 Lỗi không xác định
        ///101 Sai ApiKey hoặc SecretKey
        ///103 Tài khoản không đủ tiền
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        public static async Task<mcm_response> SendMultipleChanel(mcm_request rq)
        {
            try
            {
                using (var service = new HttpClient())
                {
                    service.BaseAddress = new Uri(MCMConfiguration.BaseUrl);
                    service.DefaultRequestHeaders.Clear();
                    service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await service.PostAsJsonAsync("/MainService.svc/json/MultiChannelMessage", rq);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<mcm_response>(jsonString);
                }
            }
            catch(Exception ex)
            {
                return new mcm_response() { CodeResult = ex.Message };
            }
            
        }
        public static async Task<mcm_response> SendSMS(mcm_request_sms_single rq)
        {
            try
            {
                using (var service = new HttpClient())
                {
                    service.BaseAddress = new Uri(MCMConfiguration.BaseUrl2);
                    service.DefaultRequestHeaders.Clear();
                    service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await service.PostAsJsonAsync("/MainService.svc/xml/SendMultipleMessage_V4_post_json", rq);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<mcm_response>(jsonString);
                }
            }
            catch (Exception ex)
            {
                return new mcm_response() { CodeResult = ex.Message };
            }
        }
    }
}
