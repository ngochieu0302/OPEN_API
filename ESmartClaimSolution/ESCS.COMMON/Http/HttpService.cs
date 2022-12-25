using ESCS.COMMON.Auth;
using ESCS.COMMON.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.Http
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> RefreshToken(authentication model);
        Task<HttpResponseMessage> Login(account model);
        Task<HttpResponseMessage> PostAsync(string url, string accessToken, object model);
        Task<HttpResponseMessage> GetAsync(string url);
    }
    public class HttpService: IHttpService
    {
        public HttpClient service;
        private static string url = "/api/esmartclaim/excute";
        private static string urlRefresh = "/api/esmartclaim/refresh-token";
        private static string urlLogin = "/api/esmartclaim/auth";
        public HttpService()
        {
            service = new HttpClient();
            service.BaseAddress = new Uri(HttpConfiguration.BaseUrl);
            service.DefaultRequestHeaders.Clear();
            service.DefaultRequestHeaders.Add("ePartnerCode", HttpConfiguration.PartnerCode);
            service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<HttpResponseMessage> RefreshToken(authentication model)
        {
            service.DefaultRequestHeaders.Add("eSignature", "");
            return await service.PostAsJsonAsync(urlRefresh, model);
        }
        public async Task<HttpResponseMessage> Login(account model)
        {
            service.DefaultRequestHeaders.Add("eSignature", "");
            return await service.PostAsJsonAsync(urlLogin, model);
        }
        public async Task<HttpResponseMessage> PostAsync(string action, string accessToken, object model)
        {
            service.DefaultRequestHeaders.Add("eAuthToken", accessToken);
            service.DefaultRequestHeaders.Add("eAction", action);
            service.DefaultRequestHeaders.Add("eSignature", "");
            return await service.PostAsJsonAsync(url, model);
        }
        public async Task<HttpResponseMessage> GetAsync(string action)
        {
            service.DefaultRequestHeaders.Add("eAuthToken", "");
            service.DefaultRequestHeaders.Add("eAction", action);
            service.DefaultRequestHeaders.Add("eSignature", "");
            return await service.GetAsync(url);
        }
    }
    public class HttpConfiguration
    {
        public static string BaseUrl { get; set; }
        public static string AccessToken { get; set; }
        public static string SecretKey { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string PartnerCode { get; set; }
        public static string CatPartner { get; set; }
        public static int SessionTimeOut { get; set; }
        public static string Environment { get; set; }
    }
}
