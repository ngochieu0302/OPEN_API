using ESCS.COMMON.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.Http
{
    public class ApiGatewayOmiCall
    {
        private static string GetTokenOpesString()
        {
            if (!OmiCallConfiguration.Enable)
                return null;
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(OmiCallConfiguration.BaseUrl);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = httpClient.GetAsync("/api/auth?apiKey="+ OmiCallConfiguration.SecretKey).Result;
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    OmiCallToken tokenMessage = JsonConvert.DeserializeObject<OmiCallToken>(jsonString);
                    return tokenMessage?.payload?.access_token;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private static List<OmiCallCustomer> ConvertCustomer(List<EscsCustomer> customers)
        {
            if (customers == null || customers.Count <= 0)
                return null;
            var omicall_customers = new List<OmiCallCustomer>();
            foreach (var item in customers)
            {
                OmiCallCustomer omi = new OmiCallCustomer();
                omi.tags = new string[] { "Khách lẻ", "Khách doanh nghiệp"};
                omi.more_infomation = new List<OmiCallMoreInfomation>();
                omi.refId = item.ma;
                omi.refCode = item.ma;
                omi.gender = "other";
                omi.fullName = item.ten;
                omi.emails = new List<OmiCallEmail>();
                omi.phones = new List<OmiCallPhone>();
                omi.phones.Add(new OmiCallPhone()
                {
                    data = item.dthoai,
                    value = "personal",
                    valueType = "Cá nhân"
                }) ;

            }
            return omicall_customers;
        }
        public static async Task SaveCustomer(List<EscsCustomer> customers)
        {
            if (!OmiCallConfiguration.Enable || customers == null || customers.Count <= 0)
                return;
            var token = GetTokenOpesString();
            if (string.IsNullOrEmpty(token))
                return;
            var omicall_customers = ConvertCustomer(customers);
            if (omicall_customers==null || omicall_customers.Count <= 0)
                return;
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(OmiCallConfiguration.BaseUrl);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    await httpClient.PostAsJsonAsync("/api/contacts/add-more", omicall_customers);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

    }
    public class OmiCallToken
    {
        public decimal? status_code { get; set; }
        public string instance_id { get; set; }
        public string instance_version { get; set; }
        public OmiCallTokenPayload payload { get; set; }
        public bool key_enabled { get; set; }
    }
    public class OmiCallTokenPayload
    {
        public string access_token { get; set; }
        public string access_type { get; set; }
        public string token_type { get; set; }
    }
    public class EscsCustomer
    {
        public string ma_doi_tac { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string dthoai { get; set; }
    }
    public class OmiCallCustomer
    {
        public string[] tags { get; set; }
        public List<OmiCallMoreInfomation> more_infomation { get; set; }
        public string user_owner_email { get; set; }
        public string refId { get; set; }
        public string refCode { get; set; }
        public string job_title { get; set; }
        public string note { get; set; }
        public string birthday { get; set; }
        public string gender { get; set; }
        public string fullName { get; set; }
        public string passport { get; set; }
        public string address { get; set; }
        public List<OmiCallEmail> emails { get; set; }
        public List<OmiCallPhone> phones { get; set; }
    }
    public class OmiCallMoreInfomation
    {
        public string value_type { get; set; }
        public string display_value { get; set; }
    }
    public class OmiCallEmail
    {
        public string data { get; set; }
        public string value { get; set; }
        public string valueType { get; set; }
    }
    public class OmiCallPhone
    {
        public string data { get; set; }
        public string value { get; set; }
        public string valueType { get; set; }
    }
}
