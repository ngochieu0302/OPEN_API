using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ESCS.COMMON.Auth;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Http;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using Microsoft.AspNetCore.Http;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ESCS.COMMON.ExtensionMethods
{
    public static class RequestExtensionMethod
    {
        public const string SESSION_KEY = "ESCS_OPENID_SESSION";
        public static object GetDataToObject(this HttpRequest rq)
        {
            string requestData = "";
            using (StreamReader reader = new StreamReader(rq.Body, Encoding.UTF8))
            {
                requestData = reader.ReadToEnd();
            }
            Dictionary<string, object> dicData = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(requestData))
            {
                string[] arr = requestData.Split('&');
                if (arr != null && arr.Count() > 0)
                {
                    foreach (var item in arr)
                    {
                        var arrKeyVal = item.Split('=');
                        if (arrKeyVal != null && arrKeyVal.Count() > 0)
                        {
                            var value = "";
                            if (arrKeyVal.Count() >= 2 && arrKeyVal[1] != null)
                            {
                                value = arrKeyVal[1];
                            }
                            try { dicData.Add(arrKeyVal[0], value); } catch { };
                        }
                    }
                    if (dicData.Count() > 0)
                    {
                        return dicData;
                    }
                    return new { };
                }
                return new { };
            }
            return new { };
        }

        public static dynamic GetDataRequest(this HttpRequest rq, object data = null)
        {
            if (data != null)
            {
                return GetDictionaryDataFromRequest(JsonConvert.SerializeObject(data));
            }
            string requestData = "";
            using (StreamReader reader = new StreamReader(rq.Body, Encoding.UTF8))
            {
                requestData = reader.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(requestData))
            {
                requestData = HttpUtility.UrlDecode(requestData);
            }
            Dictionary<string, string> dicData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(requestData))
            {
                if (rq.Method.ToUpper() == "GET")
                {
                    string[] arr = requestData.Split('&');
                    if (arr != null && arr.Count() > 0)
                    {
                        foreach (var item in arr)
                        {
                            var arrKeyVal = item.Split('=');
                            if (arrKeyVal != null && arrKeyVal.Count() > 0)
                            {
                                var value = "";
                                if (arrKeyVal.Count() >= 2 && arrKeyVal[1] != null)
                                {
                                    value = arrKeyVal[1];
                                }
                                try { dicData.Add(arrKeyVal[0], value); } catch { };
                            }
                        }
                        if (dicData.Count() > 0)
                        {
                            return dicData;
                        }
                    }
                    return null;
                }
                if (rq.Method.ToUpper() == "POST")
                {
                    if (!string.IsNullOrEmpty(requestData))
                    {
                        return JsonConvert.DeserializeObject(requestData);
                    }
                    return null;
                }
                return null;
            }
            return null;
        }
        public static dynamic GetFormDataRequest(this HttpRequest rq, out IFormFileCollection files, object data = null)
        {
            files = rq.Form.Files;
            if (data != null)
            {
                return GetDictionaryDataFromRequest(JsonConvert.SerializeObject(data));
            }
            dynamic objParam = new ExpandoObject();

            if (rq.Method.ToUpper() == "POST")
            {
                string requestData = "";
                if (!Utilities.IsMultipartContentType(rq.ContentType))
                {
                    using (StreamReader reader = new StreamReader(rq.Body, Encoding.UTF8))
                    {
                        requestData = reader.ReadToEnd();
                    }
                }
                else if (rq.Form != null)
                {
                    requestData = Utilities.FormCollectionToJson(rq.Form);
                }
                if (!string.IsNullOrEmpty(requestData))
                {
                    requestData = HttpUtility.UrlDecode(requestData);
                }

                if (!string.IsNullOrEmpty(requestData))
                {
                    dynamic objData = JsonConvert.DeserializeObject(requestData);
                    return objData;
                }
                return objParam;
            }
            return objParam;
        }
        public static Dictionary<string, string> GetDataToDic(this HttpRequest rq, object data = null)
        {
            if (data != null)
            {
                return GetDictionaryDataFromRequest(JsonConvert.SerializeObject(data));
            }
            string requestData = "";
            using (StreamReader reader = new StreamReader(rq.Body, Encoding.UTF8))
            {
                requestData = reader.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(requestData))
            {
                requestData = HttpUtility.UrlDecode(requestData);
            }
            Dictionary<string, string> dicData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(requestData))
            {
                if (rq.Method.ToUpper() == "GET")
                {
                    string[] arr = requestData.Split('&');
                    if (arr != null && arr.Count() > 0)
                    {
                        foreach (var item in arr)
                        {
                            var arrKeyVal = item.Split('=');
                            if (arrKeyVal != null && arrKeyVal.Count() > 0)
                            {
                                var value = "";
                                if (arrKeyVal.Count() >= 2 && arrKeyVal[1] != null)
                                {
                                    value = arrKeyVal[1];
                                }
                                try { dicData.Add(arrKeyVal[0], value); } catch { };
                            }
                        }
                        if (dicData.Count() > 0)
                        {
                            return dicData;
                        }
                    }
                    return null;
                }
                if (rq.Method.ToUpper() == "POST")
                {
                    if (!string.IsNullOrEmpty(requestData))
                    {
                        var dic = GetDictionaryDataFromRequest(requestData);
                        return dic;
                    }
                    return null;
                }
                return null;
            }
            return null;
        }
        public static int GetPageIndex(this HttpRequest rq, string key)
        {
            string requestData = "";
            using (StreamReader reader = new StreamReader(rq.Body, Encoding.UTF8))
            {
                requestData = reader.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(requestData))
            {
                requestData = HttpUtility.UrlDecode(requestData);
            }
            Dictionary<string, string> dicData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(requestData))
            {
                string[] arr = requestData.Split('&');
                if (arr != null && arr.Count() > 0)
                {
                    foreach (var item in arr)
                    {
                        var arrKeyVal = item.Split('=');
                        if (arrKeyVal != null && arrKeyVal.Count() > 0 && arrKeyVal[0] == key)
                        {
                            var value = "";
                            if (arrKeyVal.Count() >= 2 && arrKeyVal[1] != null)
                            {
                                value = arrKeyVal[1];
                                if (!string.IsNullOrEmpty(value))
                                {
                                    return Convert.ToInt32(value);
                                }
                            }
                            return 1;
                        }
                    }
                    return 1;
                }
                return 1;
            }
            return 1;
        }
        public static BaseResponse<T> Result<T>(this HttpResponseMessage res)
        {
            var json_serializer = new JavaScriptSerializer();
            var jsonString = res.Content.ReadAsStringAsync().Result;
            var routes_list = JsonConvert.DeserializeObject<BaseResponse<T>>(jsonString);
            return routes_list;
        }
        public static object Result(this HttpResponseMessage res)
        {
            var json_serializer = new JavaScriptSerializer();
            var jsonString = res.Content.ReadAsStringAsync().Result;
            var routes_list = json_serializer.DeserializeObject(jsonString);
            return routes_list;
        }
        public static async Task<object> GetRespone(this HttpRequest rq, string action, object data = null)
        {
            var timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            IHttpService _service = new HttpService();
            authentication auth = new authentication();
            if (HttpConfiguration.CatPartner == "PUBLIC")
            {
                auth = JsonConvert.DeserializeObject<authentication>(rq.HttpContext.Session.GetString(SESSION_KEY));
                if (timeNow >= Convert.ToInt64(auth.time_exprive.NumberToDateTime().Value.AddMinutes(-3).ToString("yyyyMMddHHmmss")))
                {
                    var resRefresh = await _service.RefreshToken(auth);
                    auth = resRefresh.Result<authentication>().data_info;
                    rq.HttpContext.Session.SetString(SESSION_KEY, JsonConvert.SerializeObject(auth));
                }
            }
            else
            {
                auth.access_token = HttpConfiguration.AccessToken;
            }
            dynamic rqData = rq.GetDataRequest(data);
            BaseRequestEscs rqBase = new BaseRequestEscs(rqData);
            var rp = await _service.PostAsync(action, auth.access_token, rqBase);
            return rp.Result();
        }
        public static async Task<BaseResponse<T>> GetRespone<T>(this HttpRequest rq, string action, object data = null)
        {
            var timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            IHttpService _service = new HttpService();
            authentication auth = new authentication();
            if (HttpConfiguration.CatPartner == "PUBLIC")
            {
                auth = JsonConvert.DeserializeObject<authentication>(rq.HttpContext.Session.GetString(SESSION_KEY));
                if (timeNow >= Convert.ToInt64(auth.time_exprive.NumberToDateTime().Value.AddMinutes(-3).ToString("yyyyMMddHHmmss")))
                {
                    var resRefresh = await _service.RefreshToken(auth);
                    auth = resRefresh.Result<authentication>().data_info;
                    rq.HttpContext.Session.SetString(SESSION_KEY, JsonConvert.SerializeObject(auth));
                }
            }
            else
            {
                auth.access_token = HttpConfiguration.AccessToken;
            }
            Dictionary<string, string> rqData = rq.GetDataToDic(data);
            BaseRequestOpenId rqBase = new BaseRequestOpenId(rqData);
            var rp = await _service.PostAsync(action, auth.access_token, rqBase);
            return rp.Result<T>();
        }
        public static HeaderRequest GetHeader(this HttpRequest rq)
        {
            HeaderRequest header = new HeaderRequest();
            rq.Headers.TryGetValue("ePartnerCode", out var partner_code);
            rq.Headers.TryGetValue("eAction", out var action);
            rq.Headers.TryGetValue("eAuthToken", out var token);
            rq.Headers.TryGetValue("eSignature", out var signature);
            rq.Headers.TryGetValue("eEnvirontment", out var environment);
            header.partner_code = partner_code.ToString();
            header.action = action.ToString();
            header.token = token.ToString();
            header.signature = signature.ToString();
            if (!string.IsNullOrEmpty(environment.ToString()))
            {
                header.envcode = environment.ToString();
            }
            return header;
        }
        public static Dictionary<string, string> GetDictionaryDataFromRequest(string json)
        {
            var dicObj = DeserializeData(json);
            Dictionary<string, string> dString = dicObj.ToDictionary(k => k.Key, k => k.Value == null ? "" : k.Value.ToString());
            return dString;
        }
        public static IDictionary<string, object> DeserializeData(string data)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            return DeserializeData(values);
        }
        public static IDictionary<string, object> DeserializeData(IDictionary<string, object> data)
        {
            Dictionary<string, object> dicChildren = new Dictionary<string, object>();
            List<string> keyExist = new List<string>();
            foreach (var key in data.Keys.ToArray())
            {
                var value = data[key];
                if (value is JArray)
                {
                    keyExist.Add(key);
                    //IList<object> lstObject = DeserializeData(value as JArray);
                    var lstObject = (value as JArray).ToObject<List<object>>();
                    if (lstObject != null)
                    {
                        int i = 0;
                        foreach (var item in lstObject)
                        {
                            if (item is JObject)
                            {
                                var itemDic = (item as JObject).ToObject<Dictionary<String, Object>>();
                                foreach (var itemTmp in itemDic)
                                {
                                    dicChildren.Add(key + "[" + i + "]." + itemTmp.Key, itemTmp.Value);
                                }
                            }
                            else
                            {
                                dicChildren.Add(key + "[" + i + "]", item);
                            }
                            i++;
                        }
                    }
                }
            }
            foreach (var key in keyExist)
            {
                data.Remove(key);
            }
            if (dicChildren != null)
            {
                foreach (var item in dicChildren)
                {
                    data.Add(item);
                }
            }
            return data;
        }
        public static IDictionary<string, object> DeserializeData(JObject data)
        {
            var dict = data.ToObject<Dictionary<String, Object>>();
            return DeserializeData(dict);
        }
        public static IList<object> DeserializeData(JArray data)
        {
            var list = data.ToObject<List<object>>();

            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];

                if (value is JObject)
                    list[i] = DeserializeData(value as JObject);

                if (value is JArray)
                    list[i] = DeserializeData(value as JArray);
            }

            return list;
        }
        //----------------------------------------------------------------
        public static IDictionary<string, object> GetData(this HttpRequest rq, out JObject define_info, out string payload)
        {
            string requestData = "";
            using (StreamReader reader = new StreamReader(rq.Body, Encoding.UTF8))
            {
                requestData = reader.ReadToEnd();// HttpUtility.UrlDecode();
            }
            var obj = JObject.Parse(requestData);
            define_info = (JObject)obj["define_info"];
            var jsonData = JsonConvert.SerializeObject(obj["data_info"]);
            payload = JWTHelper.Base64UrlEncode(jsonData);

            var dicData = ((JObject)obj["data_info"]).ToObject<Dictionary<string, object>>();
            var jArr = dicData.Where(n => n.Value is JArray && ((JArray)n.Value).FirstOrDefault() != null && ((JArray)n.Value).FirstOrDefault() is JObject);
            if (jArr != null && jArr.Count() > 0)
            {
                Dictionary<string, object> newItem = new Dictionary<string, object>();
                foreach (var item in jArr)
                {

                    var arr = (JArray)item.Value;
                    var first = arr.FirstOrDefault();
                    if (first == null)
                    {
                        break;
                    }
                    var lstProperty = ((JObject)first).Properties().Select(n => n.Name);
                    foreach (var pro in lstProperty)
                    {
                        string keyNew = item.Key + "_" + pro;
                        var value = new JArray(arr.Select(n => n[pro]));
                        newItem.Add(keyNew, value);
                    }
                }
                dicData.AddRange(newItem);
            }
            return dicData;
        }
        public static IDictionary<string, object> ConvertStringJsonToDictionary(this string objJson)
        {
            var obj = JObject.Parse(objJson);
            var state_info = JsonConvert.SerializeObject(obj["state_info"]);
            StateInfo sate = JsonConvert.DeserializeObject<StateInfo>(state_info);
            if (sate.status == "NotOK")
                return null;

            var jsonData = JsonConvert.SerializeObject(obj["data_info"]);
            var dicData = ((JObject)obj["data_info"]).ToObject<Dictionary<string, object>>();
            var jArr = dicData.Where(n => n.Value is JArray && ((JArray)n.Value).FirstOrDefault() != null && ((JArray)n.Value).FirstOrDefault() is JObject);
            if (jArr != null && jArr.Count() > 0)
            {
                Dictionary<string, object> newItem = new Dictionary<string, object>();
                foreach (var item in jArr)
                {

                    var arr = (JArray)item.Value;
                    var first = arr.FirstOrDefault();
                    if (first == null)
                    {
                        break;
                    }
                    var lstProperty = ((JObject)first).Properties().Select(n => n.Name);
                    foreach (var pro in lstProperty)
                    {
                        string keyNew = item.Key + "_" + pro;
                        var value = new JArray(arr.Select(n => n[pro]));
                        newItem.Add(keyNew, value);
                    }
                }
                dicData.AddRange(newItem);
            }
            return dicData;
        }

        public static string AddPropertyStringJson(this string json, string key, string value)
        {
            if (json != "{}")
            {
                json = json.Substring(0, json.Length - 1) + ",\"" + key + "\":\"" + value + "\"}";
            }
            else
            {
                json = json.Substring(0, json.Length - 1) + "\"" + key + "\":\"" + value + "\"}";
            }
            return json;
        }
        public static string AddPropertyStringJson(this string json, string key, bool value)
        {
            if (json != "{}")
            {
                if (value)
                {
                    json = json.Substring(0, json.Length - 1) + ",\"" + key + "\": true}";
                }
                else
                {
                    json = json.Substring(0, json.Length - 1) + ",\"" + key + "\": false}";
                }
            }
            else
            {
                if (value)
                {
                    json = json.Substring(0, json.Length - 1) + "\"" + key + "\": true}";
                }
                else
                {
                    json = json.Substring(0, json.Length - 1) + "\"" + key + "\": false}";
                }

            }
            return json;
        }
        public static string AddPropertyStringJson(this string json, string key, decimal? value)
        {
            if (json != "{}")
            {
                json = json.Substring(0, json.Length - 1) + ",\"" + key + "\":" + value + "}";
            }
            else
            {
                json = json.Substring(0, json.Length - 1) + "\"" + key + "\":" + value + "}";
            }

            return json;
        }
        public static async Task<string> ReadAsStringAsync(this IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            return result.ToString();
        }
    }
}
