using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.SMS.FPT
{
    public class FPTSMSService
    {
        public async Task<fpt_response_token> GetToken(string base_url, string api_auth, fpt_request_token request)
        {
            try
            {
                using (var service = new HttpClient())
                {
                    service.BaseAddress = new Uri(base_url);
                    service.DefaultRequestHeaders.Clear();
                    service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await service.PostAsJsonAsync(api_auth, request);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<fpt_response_token>(jsonString);
                }
            }
            catch(Exception ex)
            {
                return new fpt_response_token() { error = 500, error_description = ex.Message };
            }
        }
        public async Task<fpt_response_send_sms> SendSMS(string base_url, string api_send_sms, fpt_request_send_sms request)
        {
            try
            {
                using (var service = new HttpClient())
                {
                    service.BaseAddress = new Uri(base_url);
                    service.DefaultRequestHeaders.Clear();
                    service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await service.PostAsJsonAsync(api_send_sms, request);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<fpt_response_send_sms>(jsonString);
                }
            }
            catch(Exception ex)
            {
                return new fpt_response_send_sms() { error_description = ex.Message };
            }
        }
    }
}
