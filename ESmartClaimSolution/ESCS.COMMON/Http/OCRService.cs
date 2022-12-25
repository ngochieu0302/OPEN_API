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
    public class OCRService
    {
        private readonly static string baseUrlOCR = "https://demo.computervision.com.vn/api/v2/ocr/document/get_label_claim?get_thumb=false&format_type=file";
        private readonly static string ApiKeyOCR = "4f1c443035f14252a5bbee6ebe166e5e";
        private readonly static string SecretKeyOCR = "b8e602871c0cf17d4cf5085f0bfbb0593de8afbd807b69c3c78dc2f47dda99cc";

        public static async Task<T> OCRCar<T>(string baseUrl, string api_key, byte[] file)
        {
            var decript = Utilities.DecryptByKey(api_key, AppSettings.KeyEryptData);
            if (!string.IsNullOrEmpty(decript))
                api_key = decript;
            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("api_key", api_key);

                    form.Add(new ByteArrayContent(file), "file", "file.jpg");
                    var response = await client.PostAsync("/api/v1/gtcn/extract-sync", form);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
        }
        public static async Task<T> OCRFPTBangLaiXe<T>(string baseUrl, string api_key, byte[] file)
        {
            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("api_key", api_key);

                    form.Add(new ByteArrayContent(file), "image", "image.jpg");
                    var response = await client.PostAsync("/vision/dlr/vnm", form);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
        }
        public static async Task<T> OCRFPTDangKy<T>(string baseUrl, string api_key, byte[] file)
        {
            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("api_key", api_key);

                    form.Add(new ByteArrayContent(file), "image", "image.jpg");
                    var response = await client.PostAsync("/vision/vrr/vnm", form);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
        }
        public static async Task<T> OCRFPTDangKiem<T>(string baseUrl, string api_key, byte[] file)
        {
            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("api_key", api_key);

                    form.Add(new ByteArrayContent(file), "image", "image.jpg");
                    var response = await client.PostAsync("/vision/vnm/inspection-cert", form);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
        }
        public static async Task<T> NhanDienAnhTonThat<T>(string baseUrl, string api_key, byte[] file)
        {
            var decript = Utilities.DecryptByKey(api_key, AppSettings.KeyEryptData);
            if (!string.IsNullOrEmpty(decript))
                api_key = decript;

            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("api-key", api_key);

                    form.Add(new ByteArrayContent(file), "image", "image.jpg");
                    form.Add(new StringContent("true"), "return_base64");
                    var response = await client.PostAsync("/dmp/cardam/detect", form);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
            }
        }
        public static async Task<object> CropImage(string baseUrl, string api_key, byte[] file, string face = "0")
        {
            var decript = Utilities.DecryptByKey(api_key, AppSettings.KeyEryptData);
            if (!string.IsNullOrEmpty(decript))
                api_key = decript;

            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("api-key", api_key);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        form.Add(new ByteArrayContent(file), "image", "image.jpg");
                        form.Add(new StringContent(face), "face");
                        var response = await client.PostAsync("/vision/idr/vnm", form);
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject(jsonString);
                    }
                }
            }
        }

        public static async Task<string> OCRLabelClaim(byte[] file)
        {
            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    var authToken = Encoding.ASCII.GetBytes($"{ApiKeyOCR}:{SecretKeyOCR}");
                    client.BaseAddress = new Uri(baseUrlOCR);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        form.Add(new ByteArrayContent(file), "img", "img.jpg");
                        var response = await client.PostAsync("/api/v2/ocr/document/get_label_claim?get_thumb=true&format_type=file", form);
                        return response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
        }
        public static async Task<T> AINhanDienAnhTonThat<T>(string baseUrl, string api_key, string secretkey, byte[] file)
        {
            using (var client = new HttpClient())
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    var authToken = Encoding.ASCII.GetBytes($"{api_key}:{secretkey}");
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        form.Add(new ByteArrayContent(file), "img", "img.jpg");
                        var response = await client.PostAsync("/api/v2/vision/car_damage_assessment?get_thumb=true&format_type=file", form);
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<T>(jsonString);
                    }
                }
            }
        }
    }
}
