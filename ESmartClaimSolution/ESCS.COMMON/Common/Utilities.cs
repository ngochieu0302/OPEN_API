using DevExpress.XtraRichEdit;
using DevExpress.Office;
using DevExpress.XtraRichEdit.API.Native;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Http;
using ESCS.COMMON.Oracle;
using ESCS.COMMON.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing.Text;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.Office.Utils;
using Spire.Pdf;
using Spire.Pdf.General.Find;
using Spire.Pdf.Graphics;
using System.Xml.Linq;

namespace ESCS.COMMON.Common
{
    public class Utilities
    {
        const string GS_KEY = "escs@2020";
        public static string Encrypt(string toEncrypt, bool useHashing = true)
        {
            if (string.IsNullOrEmpty(toEncrypt))
                return "";
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(GS_KEY));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(GS_KEY);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string cipherString, bool useHashing = true)
        {
            if (string.IsNullOrEmpty(cipherString))
                return "";
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            string key = GS_KEY;
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static string EncryptByKey(string toEncrypt, string hashkey, bool useHashing = true)
        {
            if (string.IsNullOrEmpty(toEncrypt))
                return "";
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hashkey));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(hashkey);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string DecryptByKey(string cipherString, string hashkey, bool useHashing = true)
        {
            try
            {
                if (string.IsNullOrEmpty(cipherString))
                    return "";
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);
                string key = hashkey;
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);
                }
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(
                                     toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return null;
            }
        }
        public static string Sha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static string HMACSHA256(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        public static string HMACSHA256Default(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        public static string HmacSha256Digest(string message, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        public static string HMACSHA1(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha1 = new HMACSHA1(keyByte))
            {
                byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        public static Dictionary<string, string> ConvertStringJsonToDictionary(string json)
        {
            var dicObj = DeserializeData(json);
            if (dicObj != null && dicObj.Count() > 0)
            {
                Dictionary<string, string> dString = dicObj.ToDictionary(k => k.Key, k => k.Value == null ? "" : k.Value.ToString());
                return dString;
            }
            return null;
        }
        public static IDictionary<string, object> DeserializeData(string data)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            return DeserializeData(values);
        }
        public static IDictionary<string, object> DeserializeData(JObject data)
        {
            var dict = data.ToObject<Dictionary<String, Object>>();
            return DeserializeData(dict);
        }
        public static IDictionary<string, object> DeserializeData(IDictionary<string, object> data)
        {
            foreach (var key in data.Keys.ToArray())
            {
                var value = data[key];

                if (value is JObject)
                    data[key] = DeserializeData(value as JObject);

                if (value is JArray)
                    data[key] = DeserializeData(value as JArray);
            }

            return data;
        }
        public static IList<Object> DeserializeData(JArray data)
        {
            var list = data.ToObject<List<Object>>();

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
        public static int TinhTuoi(string ngay_sinh, string ngay_hl = null)
        {
            if (string.IsNullOrEmpty(ngay_hl))
            {
                ngay_hl = DateTime.Now.ToString("dd/MM/yyyy");
            }
            var arr_ngay_sinh = ngay_sinh.Split('/');
            var arr_ngay_hl = ngay_hl.Split('/');

            var ngay_sinh_year = int.Parse(arr_ngay_sinh[2]);
            var ngay_sinh_month = int.Parse(arr_ngay_sinh[1]);
            var ngay_sinh_day = int.Parse(arr_ngay_sinh[0]);

            var ngay_hl_year = int.Parse(arr_ngay_hl[2]);
            var ngay_hl_month = int.Parse(arr_ngay_hl[1]);
            var ngay_hl_day = int.Parse(arr_ngay_hl[0]);

            if (ngay_sinh_month == 0)
            {
                ngay_sinh_month++;
                ngay_hl_month++;
            }
            var numberOfMonths = (ngay_hl_year - ngay_sinh_year) * 12 + (ngay_hl_month - ngay_sinh_month);
            if (ngay_hl_month == ngay_sinh_month)
            {
                if (ngay_hl_day < ngay_sinh_day)
                {
                    numberOfMonths = numberOfMonths - 1;
                }
            }
            var age = Math.Floor((decimal)numberOfMonths / 12);
            return (int)age;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static void SaveImage(Stream stream, string path, long QualityImage = 100)
        {
            Bitmap image = new Bitmap(stream);
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, QualityImage);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            image.Save(path, jpegCodec, encoderParams);
        }
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }
        public static T ConvertStringJsonToObject<T>(string stringJson)
        {
            if (string.IsNullOrEmpty(stringJson))
            {
                return default(T);
            }
            JsonConvert.DefaultSettings = SettingMaxLengtJson;
            return JsonConvert.DeserializeObject<T>(stringJson);
        }
        public static string ConvertObjectToStringJson<T>(T obj)
        {
            JsonConvert.DefaultSettings = SettingMaxLengtJson;
            return JsonConvert.SerializeObject(obj);
        }
        public static JsonSerializerSettings SettingMaxLengtJson()
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
            jsonSetting.MaxDepth = int.MaxValue;
            return jsonSetting;
        }
        public static object ToObjectJson(dynamic data)
        {
            if (data == null)
            {
                return null;
            }
            String objJson = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new LowercaseContractResolver(), MaxDepth = int.MaxValue });
            object json = JsonConvert.DeserializeObject(objJson, typeof(object));
            return json;
        }
        public static object ToListObjectJson(IEnumerable<dynamic> data)
        {
            if (data == null)
            {
                return null;
            }
            String objJson = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new LowercaseContractResolver(), MaxDepth = int.MaxValue });
            object dataJson = JsonConvert.DeserializeObject(objJson, typeof(object));
            return dataJson;
        }
        public static DataTable ToDataTable(IEnumerable<dynamic> data)
        {
            if (data == null)
            {
                return null;
            }
            String objJson = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { Formatting = Formatting.Indented, ContractResolver = new LowercaseContractResolver(), MaxDepth = int.MaxValue });
            DataTable dataJson = (DataTable)JsonConvert.DeserializeObject(objJson, typeof(DataTable));
            return dataJson;
        }
        public static DataTable ConvertToDataTable<TSource>(IEnumerable<TSource> source)
        {
            var props = typeof(TSource).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(
              props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray()
            );
            source.ToList().ForEach(
              i => dt.Rows.Add(props.Select(p => p.GetValue(i, null)).ToArray())
            );
            return dt;
        }
        public static bool IsCollectionType(Type type)
        {
            return (type.GetInterface(nameof(ICollection)) != null);
        }
        public static KeyValuePair<T, V> CastFrom<T, V>(Object obj)
        {
            return (KeyValuePair<T, V>)obj;
        }
        public static BaseRequest GetRequestData(object model, out Dictionary<string, bool> prefix_arr)
        {
            var json = JsonConvert.SerializeObject(model);
            IDictionary<string, object> data = Utilities.DeserializeData(json);
            object obj_data_info = null;
            if (data != null && data.ContainsKey("data_info"))
            {
                obj_data_info = data["data_info"];
            }
            data["data_info"] = null;
            BaseRequest rq = new JavaScriptSerializer().Deserialize<BaseRequest>(json);
            rq.data_info = new Dictionary<string, string>();
            prefix_arr = new Dictionary<string, bool>();
            if (obj_data_info == null)
            {
                return rq;
            }
            var json_data_info = JsonConvert.SerializeObject(obj_data_info);
            if (string.IsNullOrEmpty(json_data_info))
            {
                return rq;
            }
            IDictionary<string, object> data_info = Utilities.DeserializeData(json_data_info);
            if (data_info == null || data_info.Count() <= 0)
            {
                return rq;
            }
            foreach (var key in data_info.Keys.ToArray())
            {
                var value = data_info[key];
                bool isCollection = value == null ? false : Utilities.IsCollectionType(value.GetType());
                if (isCollection)
                {
                    bool isObject = false;
                    IEnumerable<object> lstData = value as IEnumerable<object>;
                    int index = 0;
                    foreach (var item in lstData)
                    {
                        if (Utilities.IsCollectionType(item.GetType()))
                        {
                            isObject = true;
                            var jsonItem = JsonConvert.SerializeObject(item);
                            IDictionary<string, object> dataItem = Utilities.DeserializeData(jsonItem);
                            if (dataItem != null && dataItem.Count() > 0)
                            {
                                foreach (var itemKey in dataItem.Keys)
                                {
                                    object valueItem = dataItem[itemKey];
                                    rq.data_info.Add(key + "[" + index + "][" + itemKey + "]", valueItem != null ? valueItem.ToString() : "");
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            isObject = false;
                            rq.data_info.Add(key + "[" + index + "]", item != null ? item.ToString() : "");
                            index++;
                        }
                    }
                    prefix_arr.Add(key, isObject);
                }
                else
                {
                    rq.data_info.Add(key, value != null ? value.ToString() : "");
                }
            }
            return rq;
        }
        public static BaseRequestMail GetRequestDataSendEmail(object model, out Dictionary<string, bool> prefix_arr)
        {
            var json = JsonConvert.SerializeObject(model);
            IDictionary<string, object> data = Utilities.DeserializeData(json);
            object obj_data_info = data["data_info"];
            data["data_info"] = null;
            BaseRequestMail rq = data.GetObject<BaseRequestMail>();
            rq.data_info = null;
            prefix_arr = new Dictionary<string, bool>();
            if (obj_data_info == null)
            {
                return rq;
            }
            var json_data_info = JsonConvert.SerializeObject(obj_data_info);
            if (string.IsNullOrEmpty(json_data_info))
            {
                return rq;
            }
            IDictionary<string, object> data_info = Utilities.DeserializeData(json_data_info);
            if (data_info == null || data_info.Count() <= 0)
            {
                return rq;
            }
            foreach (var key in data_info.Keys.ToArray())
            {
                var value = data_info[key];
                bool isCollection = value == null ? false : Utilities.IsCollectionType(value.GetType());
                if (isCollection)
                {
                    bool isObject = false;
                    IEnumerable<object> lstData = value as IEnumerable<object>;
                    int index = 0;
                    foreach (var item in lstData)
                    {
                        if (Utilities.IsCollectionType(item.GetType()))
                        {
                            isObject = true;
                            var jsonItem = JsonConvert.SerializeObject(item);
                            IDictionary<string, object> dataItem = Utilities.DeserializeData(jsonItem);
                            if (dataItem != null && dataItem.Count() > 0)
                            {
                                foreach (var itemKey in dataItem.Keys)
                                {
                                    object valueItem = dataItem[itemKey];
                                    rq.data_info.Add(key + "[" + index + "][" + itemKey + "]", valueItem != null ? valueItem.ToString() : "");
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            isObject = false;
                            rq.data_info.Add(key + "[" + index + "]", item != null ? item.ToString() : "");
                            index++;
                        }
                    }
                    prefix_arr.Add(key, isObject);
                }
                else
                {
                    rq.data_info.Add(key, value != null ? value.ToString() : "");
                }
            }
            return rq;
        }
        public static string GetKeyCache(ActionConnection action, BaseRequest rq = null)
        {
            if (action == null || string.IsNullOrEmpty(action.type_cache) || action.type_cache == "NONE")
                return null;

            switch (action.type_cache)
            {
                case "ALLOW_CACHE":
                    string KEY_ALLOW_CACHE = CachePrefixKeyConstants.GetKeyCacheResponseAction(action.env, "*", action.db_name, action.schemadb, action.actionprocode, action.action_key_cache);
                    string[] param_caches = string.IsNullOrEmpty(action.param_cache) ? null : action.param_cache.Split(',');
                    if (param_caches != null && param_caches.Count() > 0 && rq != null && rq.data_info != null && rq.data_info.Count() > 0)
                    {
                        string key_search = "";
                        foreach (var item in param_caches)
                        {
                            string key = item;
                            if (item.StartsWith("b_"))
                            {
                                key = item.Substring(2, item.Length - 2);
                            }
                            if (rq.data_info.ContainsKey(key))
                            {
                                var val = rq.data_info[key];
                                if (!string.IsNullOrEmpty(val))
                                {
                                    if (!string.IsNullOrEmpty(key_search))
                                        key_search = string.Format("{0}.{1}", key_search, key.ToUpper() + "[" + val + "]");
                                    else
                                        key_search = string.Format("{0}{1}", key_search, key.ToUpper() + "[" + val + "]");
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(key_search))
                        {
                            KEY_ALLOW_CACHE = CachePrefixKeyConstants.GetKeyCacheResponseAction(action.env, action.partner_code, action.db_name, action.schemadb, action.actionprocode, action.action_key_cache, key_search);
                        }
                    }
                    return KEY_ALLOW_CACHE;
                case "BEHAVIORS_CACHE":
                    return "";
                default:
                    return null;
            }
        }
        public static string GetKeyCacheNew(ActionConnection action, IDictionary<string, object> data_info = null)
        {
            if (action == null || string.IsNullOrEmpty(action.type_cache) || action.type_cache == "NONE")
                return null;

            switch (action.type_cache)
            {
                case "ALLOW_CACHE":
                    string KEY_ALLOW_CACHE = CachePrefixKeyConstants.GetKeyCacheResponseAction(action.env, action.partner_code, action.db_name, action.schemadb, action.actionprocode, action.action_key_cache);
                    string[] param_caches = string.IsNullOrEmpty(action.param_cache) ? null : action.param_cache.Split(',');
                    if (param_caches != null && param_caches.Count() > 0 && data_info != null && data_info.Count() > 0)
                    {
                        string key_search = "";
                        foreach (var item in param_caches)
                        {
                            string key = item;
                            if (item.StartsWith("b_"))
                            {
                                key = item.Substring(2, item.Length - 2);
                            }
                            if (data_info.ContainsKey(key))
                            {
                                var val = data_info[key];
                                if (val != null && !string.IsNullOrEmpty(val.ToString()))
                                {
                                    if (!string.IsNullOrEmpty(key_search))
                                        key_search = string.Format("{0}.{1}", key_search, key.ToUpper() + "[" + val + "]");
                                    else
                                        key_search = string.Format("{0}{1}", key_search, key.ToUpper() + "[" + val + "]");
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(key_search))
                        {
                            KEY_ALLOW_CACHE = CachePrefixKeyConstants.GetKeyCacheResponseAction(action.env, action.partner_code, action.db_name, action.schemadb, action.actionprocode, action.action_key_cache, key_search);
                        }
                    }
                    return KEY_ALLOW_CACHE.Replace("#", "");
                case "BEHAVIORS_CACHE":
                    return "";
                default:
                    return null;
            }
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static string ReadStringByPathFile(string pathFile)
        {
            try
            {
                using (StreamReader reader = File.OpenText(pathFile))
                {
                    string fileContent = reader.ReadToEnd();
                    if (fileContent != null && fileContent != "")
                    {
                        return fileContent;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }
        public static List<StatusUploadFile> UploadFiles(IFormFileCollection files, string baseUrl, string type_folder, OptionSaveFile config, List<FileInfoData> listFileData, string[] extensionFile = null, string prefixFileName = null)
        {
            var extImage = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
            var extDoc = new string[] { ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".xml" };

            if (files == null)
                throw new ArgumentNullException("files");
            List<StatusUploadFile> lstFile = new List<StatusUploadFile>();
            var dateNow = DateTime.Now;
            var year = dateNow.Year.ToString();
            var month = dateNow.Month.ToString();
            var day = dateNow.Day.ToString();
            if (month.Length < 2)
                month = "0" + month;
            if (day.Length < 2)
                day = "0" + day;
            string typePath = Path.Combine(config.ma_doi_tac, type_folder, year, month, day);
            var path = Path.Combine(baseUrl, typePath);
            if (AppSettings.FolderSharedUsed && path.StartsWith(@"\") && !path.StartsWith(@"\\"))
                path = @"\" + path;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            foreach (var file in files)
            {
                #region Kiểm tra thông tin file và lấy ra tên file
                string extensionItem = Path.GetExtension(file.FileName).ToLower();
                var dataFile = listFileData.Where(n => n.key_file == file.Name).FirstOrDefault();
                string nhom_anh = dataFile == null ? "" : string.IsNullOrEmpty(dataFile.nhom) ? "" : dataFile.nhom;
                decimal? stt = dataFile == null ? 0 : dataFile.stt == null ? 0 : dataFile.stt;
                decimal x = 0, y = 0;
                if (extensionFile != null && !extensionFile.Contains(extensionItem))
                {
                    lstFile.Add(new StatusUploadFile("", "", file.FileName, "", nhom_anh, extensionItem, stt, StatusUploadFileConstant.ERROR, "File không có trong danh sách định dạng được lưu."));
                    continue;
                }
                string filenameNew = config.so_id + "_" + Guid.NewGuid().ToString("N") + extensionItem;
                if (!string.IsNullOrEmpty(prefixFileName))
                {
                    filenameNew = config.so_id + "_" + prefixFileName + "_" + Guid.NewGuid().ToString("N") + extensionItem;
                }
                var pathFile = Path.Combine(typePath, filenameNew);
                var item = new StatusUploadFile(baseUrl, pathFile, file.FileName, filenameNew, nhom_anh, extensionItem, stt, StatusUploadFileConstant.SUCCESS, "", StreamToByteArray(file.OpenReadStream()), x, y);
                item.SetPhanLoai(dataFile);
                lstFile.Add(item);
                #endregion
            }
            if (lstFile != null && lstFile.Count() > 0 && lstFile.Where(n => n.status_upload == StatusUploadFileConstant.ERROR).Count() <= 0)
            {
                foreach (var status in lstFile)
                {
                    try
                    {
                        #region Lưu file thumnail đồng bộ
                        byte[] imageBytes = status.file;
                        if (extImage.Contains(status.extension_file) && config.is_duplicate_mini_file)
                        {
                            string filenameMini = config.config_mini_file.prefix + "_" + status.file_name_new;
                            var pathFileMini = Path.Combine(baseUrl, typePath, filenameMini);
                            if (AppSettings.FolderSharedUsed && pathFileMini.StartsWith(@"\") && !pathFileMini.StartsWith(@"\\"))
                                pathFileMini = @"\" + pathFileMini;

                            Utilities.SaveFileUpoad(imageBytes, pathFileMini, config.config_mini_file.width.Value, config.config_mini_file.max_length.Value);
                        }
                        #endregion
                        #region Lưu file chính
                        var pathFileChinh = Path.Combine(baseUrl, status.path_file);
                        if (AppSettings.FolderSharedUsed && pathFileChinh.StartsWith(@"\") && !pathFileChinh.StartsWith(@"\\"))
                            pathFileChinh = @"\" + pathFileChinh;

                        if (extImage.Contains(status.extension_file))
                            Utilities.SaveFileUpoad(imageBytes, pathFileChinh, config.max_width, config.max_length);
                        if (extDoc.Contains(status.extension_file))
                            File.WriteAllBytes(pathFileChinh, status.file);
                        #endregion
                        status.status_upload = StatusUploadFileConstant.SUCCESS;
                    }
                    catch (Exception ex)
                    {
                        status.status_upload = StatusUploadFileConstant.ERROR;
                        status.error_message = ex.Message;
                    }

                }
            }
            return lstFile;
        }
        public static byte[] SaveFileUpoad(byte[] imageBytes, string path, int? max_width, long? max_content_length = null)
        {
            byte[] array = null;
            max_content_length = max_content_length ?? 1000000;
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                using (Image img = Image.FromStream(ms, true, false))
                {
                    var current_size = img.Size;
                    int height = current_size.Height;
                    if (max_width != null)
                    {
                        if (current_size.Width > max_width)
                        {
                            height = max_width.Value * height / current_size.Width;
                        }
                        else
                        {
                            max_width = current_size.Width;
                        }
                    }
                    using (Bitmap b = new Bitmap(img, new Size(max_width.Value, height)))
                    {
                        using (var streamNew = Utilities.DownscaleImage(b, max_content_length.Value))
                        {
                            FileStream fileNew = new FileStream(path, FileMode.Create, FileAccess.Write);
                            streamNew.WriteTo(fileNew);
                            fileNew.Close();
                            array = streamNew.ToArray();
                            return array;
                        }
                    }
                }
            }
        }
        public static Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //Get the image current width
            int sourceWidth = imgToResize.Width;
            //Get the image current height
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                using (var ms = new MemoryStream(byteArrayIn))
                {
                    return Image.FromStream(ms, true, true);
                }

            }
            catch { }
            return null;
        }
        public static void SaveFileCommon(IFormFile file, string path)
        {
            if (file != null && file.Length > 0)
            {
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
        }
        public static void Rotate(Bitmap bmp)
        {
            PropertyItem pi = bmp.PropertyItems.Select(x => x)
                                               .FirstOrDefault(x => x.Id == 0x0112);
            if (pi == null) return;

            byte o = pi.Value[0];

            if (o == 2) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            if (o == 3) bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            if (o == 4) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (o == 5) bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
            if (o == 6) bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (o == 7) bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
            if (o == 8) bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
        }
        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static string FormCollectionToJson(IFormCollection obj)
        {
            dynamic json = new JObject();
            if (obj.Keys.Any())
            {
                foreach (string key in obj.Keys)
                {
                    var value = obj[key][0];
                    json.Add(key, value);
                }
            }
            return JsonConvert.SerializeObject(json);
        }
        public static byte[] DowloadZipFile(List<string> listPath)
        {
            List<ZipItem> files = new List<ZipItem>();
            foreach (var path in listPath)
            {
                ZipItem zipItem = new ZipItem(Path.GetFileName(path), FileToByteArray(path));
                files.Add(zipItem);
            }
            return GetZipArchive(files);
        }
        public static byte[] FileToByteArray(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                return null;
            }
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(fileName).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }
        public static byte[] GetZipArchive(List<ZipItem> files)
        {
            byte[] archiveFile;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(file.Content, 0, file.Content.Length);
                    }
                }

                archiveFile = archiveStream.ToArray();
            }
            return archiveFile;
        }
        public static string ConvertFileToBase64String(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Byte[] bytes = File.ReadAllBytes(path);
                return Convert.ToBase64String(bytes);
            }
            return null;
        }
        public static MemoryStream DownscaleImage(Image photo, long max_content_length)
        {
            long MAX_PHOTO_SIZE = max_content_length;
            MemoryStream resizedPhotoStream = new MemoryStream();
            long resizedSize = 0;
            var quality = 100;
            //long lastSizeDifference = 0;
            do
            {
                resizedPhotoStream.SetLength(0);

                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                ImageCodecInfo ici = GetEncoderInfo("image/jpeg");

                photo.Save(resizedPhotoStream, ici, eps);
                resizedSize = resizedPhotoStream.Length;

                //long sizeDifference = resizedSize - MAX_PHOTO_SIZE;
                //Console.WriteLine(resizedSize + "(" + sizeDifference + " " + (lastSizeDifference - sizeDifference) + ")");
                //lastSizeDifference = sizeDifference;
                quality--;

            } while (resizedSize > MAX_PHOTO_SIZE);

            resizedPhotoStream.Seek(0, SeekOrigin.Begin);
            return resizedPhotoStream;
        }
        public static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
        public static string Base64UrlDecode(string input)
        {
            var bytearr = WebEncoders.Base64UrlDecode(input);
            return System.Text.Encoding.UTF8.GetString(bytearr);
        }
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
        }
        public static T Deserialize<T>(string data)
        {
            try
            {
                T ret = JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public static void CreateFolderWhenNotExist(string baseFolder, string path, NetworkCredentialItem network = null, bool ignoreFilename = true)
        {
            if (network != null && !network.IsLocal)
            {
                NetworkCredential credentials = new NetworkCredential(Utilities.Decrypt(network.UserName), Utilities.Decrypt(network.Password));
                using (new ConnectToSharedFolder(network.FullPath, credentials))
                {
                    Directory.CreateDirectory(Path.Combine(baseFolder, path));
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(baseFolder, path));
            }
        }
        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }
        public static System.Drawing.Rectangle GetRectangleFromDocumentPosition(RichEditControl richEditControl, DocumentPosition pos)
        {
            return Units.DocumentsToPixels(richEditControl.GetBoundsFromPosition(pos),
                richEditControl.DpiX, richEditControl.DpiY);
        }
        public static PositionSignature GetPositionSignature(string localPath, string path)
        {
            string pathFileTemplate = Path.Combine(localPath, path);
            if (AppSettings.FolderSharedUsed)
                pathFileTemplate = Path.Combine(@"\" + localPath, path);

            System.Xml.XmlDocument xmlTemplateDoc = new System.Xml.XmlDocument();
            xmlTemplateDoc.Load(pathFileTemplate);
            CExWordMLFiller filler = new CExWordMLFiller(new DataSet(), xmlTemplateDoc.OuterXml);
            using (var stream = new MemoryStream())
            {
                filler.WordMLDocument.Save(stream);
                RichEditDocumentServer server = new RichEditDocumentServer();

                stream.Seek(0, SeekOrigin.Begin);
                server.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.WordML);
                Document doc = server.Document;

                PositionSignature position = new PositionSignature();
                position.pageCount = server.DocumentLayout.GetPageCount();
                var layout = server.DocumentLayout.GetPage(0);
                DocumentRange rangeFound = doc.FindAll("PSIGNATURE", SearchOptions.None).FirstOrDefault();
                if (rangeFound != null)
                {
                    var element = server.DocumentLayout.GetElement(rangeFound.Start, LayoutType.PageArea).Bounds;
                    if (element != null)
                    {
                        int widthA4 = 2480;
                        int heightA4 = 3508;
                        var x = element.X * widthA4 / element.Width;
                        var y = element.Y * heightA4 / element.Height;
                        position.X = x;
                        position.Y = y;
                    }
                }
                else
                {
                    position.X = 0;
                    position.Y = 0;
                }
                return position;
            }

        }
        public static PositionSignatureF GetPositionSignature(byte[] file)
        {
            try
            {
                PositionSignatureF position = new PositionSignatureF();
                PdfDocument doc = new PdfDocument();
                doc.LoadFromBytes(file);
                int widthA4 = 2480;
                int heightA4 = 3508;
                foreach (PdfPageBase page in doc.Pages)
                {
                    PdfTextFind results = page.FindText("CHUKYSO_DIENTU").Finds.FirstOrDefault();
                    if (results != null)
                    {
                        RectangleF p = results.Bounds;
                        position.X = (decimal)p.Left;
                        position.Y = (decimal)p.Top;
                        break;
                    }
                }
                position.X = Math.Round(position.X.Value);
                position.Y = Math.Round(position.Y.Value);
                position.pageCount = 1;
                return position;
            }
            catch
            {
                return new PositionSignatureF() { };
            }
        }
        public static byte[] ExportToPDF(DataSet ds, string localPath, string path, string urlImgWatermark = null, Image img = null)
        {
            var extension = Path.GetExtension(path);
            if (extension.ToLower() == ".docx")
                return ESCSExportPDF.ToTrinhPASC(path, ds, urlImgWatermark);

            string pathFileTemplate = Path.Combine(localPath, path);
            if (AppSettings.FolderSharedUsed)
                pathFileTemplate = Path.Combine(@"\" + localPath, path);

            byte[] arrOutput = null;
            System.Xml.XmlDocument xmlTemplateDoc = new System.Xml.XmlDocument();
            xmlTemplateDoc.Load(pathFileTemplate);
            #region Loại bỏ đoạn xml không hiển thị ở đây (BEGIN_<Tên nhóm>, END_<Tên nhóm>) (nhom, hien_thi (0,1))
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Contains("curs_an_hien"))
            {
                List<BookmarkAnHien> dsNhomAnHien = ds.Tables["curs_an_hien"].ConvertDataTable<BookmarkAnHien>();
                if (dsNhomAnHien != null && dsNhomAnHien.Count > 0)
                {
                    dsNhomAnHien = dsNhomAnHien.Where(n => n.hien_thi == BookmarkAnHienConstants.KHONG_HIEN_THI).ToList();
                    System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager((System.Xml.XmlNameTable)new System.Xml.NameTable());
                    nsmgr.AddNamespace("w", "http://schemas.microsoft.com/office/word/2003/wordml");
                    nsmgr.AddNamespace("aml", "http://schemas.microsoft.com/aml/2001/core");
                    nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
                    foreach (var bookMark in dsNhomAnHien)
                    {
                        var fromNode = xmlTemplateDoc.SelectSingleNode("//w:p/aml:annotation[@w:name='BEGIN_" + bookMark.nhom + "']", nsmgr)?.ParentNode;
                        var toNode = xmlTemplateDoc.SelectSingleNode("//w:p/aml:annotation[@w:name='END_" + bookMark.nhom + "']", nsmgr)?.ParentNode;
                        if (fromNode!=null && toNode!=null)
                        {
                            var nodeParent = toNode.ParentNode;
                            int indexFromNode = -1;
                            int indexToNode = -1;
                            if (nodeParent.ChildNodes != null && nodeParent.ChildNodes.Count > 0)
                            {
                                for (int i = 0; i < nodeParent.ChildNodes.Count; i++)
                                {
                                    var node = nodeParent.ChildNodes[i];
                                    if (fromNode.InnerXml == node.InnerXml)
                                        indexFromNode = i;
                                    if (toNode.InnerXml == node.InnerXml)
                                        indexToNode = i;
                                }
                            }
                            if (indexFromNode >= 0 && indexToNode >= 0 && indexToNode >= indexFromNode)
                            {
                                for (int i = indexToNode; i >= indexFromNode; i--)
                                    nodeParent.RemoveChild(nodeParent.ChildNodes[i]);
                            }
                        }
                        
                    }

                }
            }
            #endregion
            CExWordMLFiller filler = new CExWordMLFiller(ds, xmlTemplateDoc.OuterXml);
            if (!filler.OperationFailed)
            {
                filler.Transform();
                if (filler.OperationFailed)
                    foreach (string err in filler.ErrorList)
                        return null;
            }
            else
            {
                foreach (string err in filler.ErrorList)
                    return null;
            }
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                filler.WordMLDocument.Save(stream);
                RichEditDocumentServer server = new RichEditDocumentServer();

                stream.Seek(0, SeekOrigin.Begin);
                server.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.WordML);
                Document doc = server.Document;
                doc.BeginUpdate();
                if (filler.imageSources != null && filler.imageSources.Count() > 0)
                {
                    foreach (var item in filler.imageSources)
                    {
                        int width = item.width_px == null ? 0 : (int)item.width_px;
                        int height = item.height_px == null ? 0 : (int)item.height_px;
                        var pathImg = Path.Combine(localPath, string.IsNullOrEmpty(item.source) ? "" : item.source);
                        if (AppSettings.FolderSharedUsed)
                            pathImg = @"\" + pathImg;

                        AddImage(doc, item.psource.Trim(), pathImg, width, height);
                    }
                }


                if (!string.IsNullOrEmpty(urlImgWatermark))
                    AddImageByteArray(doc, "PSIGNATURE", img);
                else
                    RemoveImageByteArray(doc, "PSIGNATURE");

                Regex regexServiceWords = new Regex(@"<b>(.*?)</b>", RegexOptions.Compiled);
                DocumentRange[] insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Bold = true;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<i>(.*?)</i>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Italic = true;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<u>(.*?)</u>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Underline = UnderlineType.Single;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<strike>(.*?)</strike>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Strikeout = StrikeoutType.Single;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<b>|</b>|<i>|</i>|<u>|</u>|<strike>|</strike>|<tmp>|</tmp>", RegexOptions.Compiled);
                doc.ReplaceAll(regexServiceWords, "");

                doc.EndUpdate();
                MemoryStream convertStream = new MemoryStream();
                server.ExportToPdf(convertStream);
                convertStream.Seek(0, SeekOrigin.Begin);
                arrOutput = convertStream.ToArray();
                return arrOutput;
            }
        }
        public static byte[] ExportHoaDon(DataSet ds, string localPath, string path)
        {
            string pathFileTemplate = Path.Combine(localPath, path);
            if (AppSettings.FolderSharedUsed)
                pathFileTemplate = Path.Combine(@"\" + localPath, path);

            byte[] arrOutput = null;
            System.Xml.XmlDocument xmlTemplateDoc = new System.Xml.XmlDocument();
            xmlTemplateDoc.Load(pathFileTemplate);
            CExWordMLFiller filler = new CExWordMLFiller(ds, xmlTemplateDoc.OuterXml);
            if (!filler.OperationFailed)
            {
                filler.Transform();
                if (filler.OperationFailed)
                    foreach (string err in filler.ErrorList)
                        return null;
            }
            else
            {
                foreach (string err in filler.ErrorList)
                    return null;
            }
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                filler.WordMLDocument.Save(stream);
                RichEditDocumentServer server = new RichEditDocumentServer();
                stream.Seek(0, SeekOrigin.Begin);
                server.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.WordML);
                Document doc = server.Document;
                doc.BeginUpdate();
                if (filler.imageSources != null && filler.imageSources.Count() > 0)
                {
                    foreach (var item in filler.imageSources)
                    {
                        int width = item.width_px == null ? 0 : (int)item.width_px;
                        int height = item.height_px == null ? 0 : (int)item.height_px;

                        var pathImg = Path.Combine(localPath, string.IsNullOrEmpty(item.source) ? "" : item.source);
                        if (AppSettings.FolderSharedUsed)
                            pathImg = @"\" + pathImg;

                        AddImageHoaDon(doc, item.psource.Trim(), pathImg, width, height);
                    }
                }
                Regex regexServiceWords = new Regex(@"<b>(.*?)</b>", RegexOptions.Compiled);
                DocumentRange[] insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Bold = true;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<i>(.*?)</i>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Italic = true;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<u>(.*?)</u>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Underline = UnderlineType.Single;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<strike>(.*?)</strike>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                if (insertedTextRange != null)
                {
                    for (int i = 0; i < insertedTextRange.Length; i++)
                    {
                        CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                        cp.Strikeout = StrikeoutType.Single;
                        doc.EndUpdateCharacters(cp);
                    }
                }
                regexServiceWords = new Regex(@"<b>|</b>|<i>|</i>|<u>|</u>|<strike>|</strike>|<tmp>|</tmp>", RegexOptions.Compiled);
                doc.ReplaceAll(regexServiceWords, "");

                doc.EndUpdate();
                MemoryStream convertStream = new MemoryStream();
                server.ExportToPdf(convertStream);
                convertStream.Seek(0, SeekOrigin.Begin);
                arrOutput = convertStream.ToArray();
                return arrOutput;
            }
        }
        public static string RemoveSensitiveProperties(string json, IEnumerable<Regex> regexes)
        {
            JToken token = JToken.Parse(json);
            RemoveSensitiveProperties(token, regexes);
            return token.ToString();
        }
        public static void RemoveSensitiveProperties(JToken token, IEnumerable<Regex> regexes)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (JProperty prop in token.Children<JProperty>().ToList())
                {
                    bool removed = false;
                    foreach (Regex regex in regexes)
                    {
                        if (regex.IsMatch(prop.Name))
                        {
                            prop.Remove();
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        RemoveSensitiveProperties(prop.Value, regexes);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (JToken child in token.Children())
                {
                    RemoveSensitiveProperties(child, regexes);
                }
            }
        }
        public static bool IsNullOrEmpty(IEnumerable source)
        {
            if (source != null)
            {
                foreach (object obj in source)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsNullOrEmpty<T>(IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (T obj in source)
                {
                    return false;
                }
            }
            return true;
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? null : dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static string GetFileExtension(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                base64String = "";
            }
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return ".png";
                case "/9J/4":
                    return ".jpg";
                case "AAAAF":
                    return ".mp4";
                case "JVBER":
                    return ".pdf";
                case "AAABA":
                    return ".ico";
                case "UMFYI":
                    return ".rar";
                case "E1XYD":
                    return ".rtf";
                case "U1PKC":
                    return ".txt";
                case "PD94B":
                    return ".xml";
                case "77U/Q":
                    return ".cshtml";
                case "MO+/V":
                    return ".pfx";
                case "UESDB":
                    return ".xlsx";
                case "77+97":
                    return ".xls";
                case "MQOWM":
                case "77U/M":
                    return ".srt";
                default:
                    return string.Empty;
            }
        }
        public void ConvertFileThemQRCODE(Stream stream, string filePath, string noi_dung, DevExpress.XtraRichEdit.DocumentFormat sourceFormat, DevExpress.XtraRichEdit.DocumentFormat destFormat, ref string outFileName_pdf1)
        {
            string outFileName = Path.ChangeExtension(filePath, "docx");
            RichEditDocumentServer server = new RichEditDocumentServer();
            server.LoadDocument(stream, sourceFormat);
            FileStream fsOut = File.Open(outFileName, FileMode.Create);
            DevExpress.XtraRichEdit.API.Native.Document doc = server.Document;
            doc.BeginUpdate();
            try
            {
                Regex regexServiceWords = new Regex(@"<b>(.*?)</b>", RegexOptions.Compiled);
                DocumentRange[] insertedTextRange = doc.FindAll(regexServiceWords);
                for (int i = 0; i < insertedTextRange.Length; i++)
                {
                    CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                    cp.Bold = true;
                    doc.EndUpdateCharacters(cp);
                }
                regexServiceWords = new Regex(@"<i>(.*?)</i>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                for (int i = 0; i < insertedTextRange.Length; i++)
                {
                    CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                    cp.Italic = true;
                    doc.EndUpdateCharacters(cp);
                }
                regexServiceWords = new Regex(@"<u>(.*?)</u>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                for (int i = 0; i < insertedTextRange.Length; i++)
                {
                    CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                    cp.Underline = UnderlineType.Single;
                    doc.EndUpdateCharacters(cp);
                }
                regexServiceWords = new Regex(@"<strike>(.*?)</strike>", RegexOptions.Compiled);
                insertedTextRange = doc.FindAll(regexServiceWords);
                for (int i = 0; i < insertedTextRange.Length; i++)
                {
                    CharacterProperties cp = doc.BeginUpdateCharacters(insertedTextRange[i]);
                    cp.Strikeout = StrikeoutType.Single;
                    doc.EndUpdateCharacters(cp);
                }
                regexServiceWords = new Regex(@"<b>|</b>|<i>|</i>|<u>|</u>|<strike>|</strike>|<tmp>|</tmp>", RegexOptions.Compiled);
                doc.ReplaceAll(regexServiceWords, "");
            }
            finally
            {

            }

            DocumentRange[] rangeFound = doc.FindAll("QRCODE", SearchOptions.CaseSensitive);
            int temp = rangeFound.Length;
            string outFileName_qrcode = Path.ChangeExtension(filePath, ".png");
            int a = doc.Sections.Count;
            if (a >= 1)
            {
                for (int i = 0; i < (a); i++)
                {
                    Section firstSection = doc.Sections[i];
                    SubDocument myHeader = firstSection.BeginUpdateFooter(HeaderFooterType.Odd);
                    DocumentImage range = myHeader.InsertImage(myHeader.CreatePosition(0), DocumentImageSource.FromFile(outFileName_qrcode));
                    myHeader.Fields.Update();
                    firstSection.EndUpdateHeader(myHeader);
                    firstSection.DifferentFirstPage = false;
                }
            }
            if (temp == 1)
            {
                DocumentPosition pos = doc.Paragraphs[doc.GetParagraph(rangeFound[0].End).Index].Range.Start;
                doc.InsertImage(pos, DocumentImageSource.FromFile(outFileName_qrcode));
            }
            else if (temp == 2)
            {
                if (rangeFound[0].Length != 0)
                {
                    DocumentPosition pos = doc.Paragraphs[doc.GetParagraph(rangeFound[0].End).Index].Range.Start;
                    doc.InsertImage(pos, DocumentImageSource.FromFile(outFileName_qrcode));
                }
                if (rangeFound[1].Length != 0)
                {
                    DocumentPosition pos = doc.Paragraphs[doc.GetParagraph(rangeFound[1].End).Index].Range.Start;
                    doc.InsertImage(pos, DocumentImageSource.FromFile(outFileName_qrcode));
                }
            }
            if (destFormat == DevExpress.XtraRichEdit.DocumentFormat.Rtf)
            {
                server.Options.Export.Rtf.Compatibility.DuplicateObjectAsMetafile = false;
            }

            server.SaveDocument(fsOut, destFormat);
            FileStream fsInQRCODE = File.OpenRead(outFileName_qrcode);
            fsInQRCODE.Flush();
            fsInQRCODE.Close();
            fsOut.Flush();
            fsOut.Close();
            string outFileName_pdf = Path.ChangeExtension(filePath, "pdf");
            using (Stream str = File.Create(outFileName_pdf))
            {
                server.ExportToPdf(str);
            }
            outFileName_pdf1 = Path.GetFileName(outFileName_pdf);

            if (File.Exists(filePath)) File.Delete(filePath);
            if (File.Exists(outFileName)) File.Delete(outFileName);
        }
        private static void AddImage(DevExpress.XtraRichEdit.API.Native.Document doc, string psource, string filePath, int width = 0, int height = 0)
        {
            DocumentRange rangeFound = doc.FindAll(psource, SearchOptions.None).FirstOrDefault();
            if (rangeFound == null)
                return;
            DocumentPosition pos = doc.Paragraphs[doc.GetParagraph(rangeFound.End).Index].Range.Start;
            if (!System.IO.File.Exists(filePath))
            {
                doc.Delete(rangeFound);
                return;
            }
            string ext = Path.GetExtension(filePath);
            if (ext == null || (ext.ToLower() != ".png" && ext.ToLower() != ".jpg" && ext.ToLower() != ".jpeg"))
            {
                doc.Delete(rangeFound);
                return;
            }
            Image img = ResizeImage(filePath, width, height);
            doc.InsertImage(pos, DocumentImageSource.FromImage(img));
            doc.Delete(rangeFound);

        }
        private static void AddImageHoaDon(DevExpress.XtraRichEdit.API.Native.Document doc, string psource, string filePath, int width = 0, int height = 0)
        {
            DocumentRange rangeFound = doc.FindAll(psource, SearchOptions.None).FirstOrDefault();
            if (rangeFound == null)
                return;
            DocumentPosition pos = doc.Paragraphs[doc.GetParagraph(rangeFound.End).Index].Range.Start;
            if (!System.IO.File.Exists(filePath))
            {
                doc.Delete(rangeFound);
                return;
            }
            string ext = Path.GetExtension(filePath);
            if (ext == null || (ext.ToLower() != ".png" && ext.ToLower() != ".jpg" && ext.ToLower() != ".jpeg"))
            {
                doc.Delete(rangeFound);
                return;
            }
            using (Image img = Image.FromFile(filePath))
            {
                var widthImg = img.Width;
                var heighImg = img.Height;
                var widthNew = width;
                var heightNew = widthNew * heighImg / widthImg;
                doc.InsertImage(pos, DocumentImageSource.FromImage(new Bitmap(img, widthNew, heightNew)));
                doc.Delete(rangeFound);
            }

        }
        private static void AddImageByteArray(DevExpress.XtraRichEdit.API.Native.Document doc, string psource, Image img)
        {
            DocumentRange rangeFound = doc.FindAll(psource, SearchOptions.None).FirstOrDefault();
            if (rangeFound == null)
                return;
            DocumentPosition pos = doc.Paragraphs[doc.GetParagraph(rangeFound.End).Index].Range.Start;
            doc.InsertImage(pos, DocumentImageSource.FromImage(img));
            doc.Delete(rangeFound);
        }
        private static void RemoveImageByteArray(DevExpress.XtraRichEdit.API.Native.Document doc, string psource)
        {
            DocumentRange rangeFound = doc.FindAll(psource, SearchOptions.None).FirstOrDefault();
            if (rangeFound == null)
                return;
            //DocumentPosition pos = doc.Paragraphs[doc.GetParagraph(rangeFound.End).Index].Range.Start;
            doc.Delete(rangeFound);
        }
        public static Image ResizeImage(string fileName, int width, int height)
        {
            using (Image image = Image.FromFile(fileName))
            {
                if (width == 0 || height == 0)
                    return new Bitmap(image);
                return new Bitmap(image, width, height);
            }
        }
        public static Image GetImageByPath(string fileName)
        {
            using (Image image = Image.FromFile(fileName))
            {
                return new Bitmap(image);
            }
        }
        public static Image RotateImage(Image img, float degree, int flipx, int flipy)
        {
            if (degree == 90)
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (degree == -90)
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            if (flipx == 1 || flipx == -1)
                img.RotateFlip(RotateFlipType.Rotate180FlipX);
            if (flipy == 1 || flipy == -1)
                img.RotateFlip(RotateFlipType.Rotate180FlipY);
            return img;
        }
        public static void SetImageWatermark(Document document, Image image)
        {
            Section section = document.Sections[0];
            SubDocument subDocument = section.BeginUpdateHeader();
            subDocument.Delete(subDocument.Range);
            Shape shape = subDocument.Shapes.InsertPicture(subDocument.Range.Start, image);

            shape.RotationAngle = -45;
            shape.Offset = new PointF(section.Page.Width / 2 - shape.Size.Width / 2, section.Page.Height / 2 - shape.Size.Height / 2);
            section.EndUpdateHeader(subDocument);
        }
        public static Image DrawLetter(string letter, string urlImage)
        {
            var img = (Bitmap)Image.FromFile(urlImage);
            var bit = new Bitmap(img.Width, 80);
            Graphics g = Graphics.FromImage(bit);

            float width = ((float)bit.Width);
            float height = ((float)bit.Height);

            float emSize = height;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode = InterpolationMode.High;

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Near;
            // Top/Left.
            sf.Alignment = StringAlignment.Center;

            Font font = new Font(new FontFamily("Tahoma"), emSize, FontStyle.Regular);
            Rectangle rect = new Rectangle(0, 0, bit.Width, bit.Height);

            font = FindBestFitFont(g, letter.ToString(), font, bit.Size);

            Color color = System.Drawing.ColorTranslator.FromHtml("#00ac3a");
            SizeF size = g.MeasureString(letter.ToString(), font);
            g.DrawString(letter, font, new SolidBrush(color), rect, sf);

            return MergeTwoImages(img, bit);
        }
        private static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width > secondImage.Width ? firstImage.Width : secondImage.Width;

            int outputImageHeight = firstImage.Height + secondImage.Height + 1;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(0, firstImage.Height + 1), secondImage.Size),
                    new Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);
            }
            return outputImage;
        }
        private static Font FindBestFitFont(Graphics g, String text, Font font, Size proposedSize)
        {
            while (true)
            {
                SizeF size = g.MeasureString(text, font);
                if (size.Height <= proposedSize.Height &&
                     size.Width <= proposedSize.Width) { return font; }
                Font oldFont = font;
                font = new Font(font.Name, (float)(font.Size * .9), font.Style);
                oldFont.Dispose();
            }
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static byte[] ConvertFileToByteArray(string path)
        {
            if (System.IO.File.Exists(path))
                return File.ReadAllBytes(path);
            return null;
        }
        public static bool IsNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            return decimal.TryParse("123", out decimal n);
        }
        public static string NumberToString(string textNumber)
        {
            if (string.IsNullOrEmpty(textNumber))
                return "";
            textNumber = textNumber.Trim();
            try
            {
                string nam = textNumber.Substring(0, 4);
                string thang = textNumber.Substring(4, 2);
                string ngay = textNumber.Substring(6, 2);
                return ngay + "/" + thang + "/" + nam + " 00:00";
            }
            catch
            {
                return "";
            }
        }
        public static string chuanHoaNgayPjico(string textNumber)
        {
            if (string.IsNullOrEmpty(textNumber))
                return "";
            textNumber = textNumber.Trim();
            try
            {
                return textNumber + " 00:00";
            }
            catch
            {
                return "";
            }
        }
    }
}