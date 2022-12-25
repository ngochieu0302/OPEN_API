using ESCS.COMMON.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.Http
{
    public class AICycleService
    {
        private readonly static string AccessToken = "197aca:6de500858d1245f1b4e259beaeb78e495dcc1a54ecfd496caeadf1f32bc819e7";
        public static async Task<string> AICycleGetLinkUpload(string jsonFilePath)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AICycleConfig.BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(jsonFilePath, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/images/url", content);
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        public static async Task<HttpResponseMessage> AICycleUpLoadImages(string url, byte[] file)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage httpResponse = new HttpResponseMessage();
                var content = new ByteArrayContent(file);
                return await client.PutAsync(url, content);
            }
        }
        public static async Task<string> AICycleCallAPIEngine(string jsonClaimImages)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AICycleConfig.BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(jsonClaimImages, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/claimimages/triton-assessment", content);
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
