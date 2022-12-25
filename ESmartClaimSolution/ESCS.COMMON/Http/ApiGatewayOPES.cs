using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Oracle;
using ESCS.COMMON.Request;
using ESCS.COMMON.Request.ApiGateway;
using ESCS.COMMON.Request.ApiGateway.MIC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.Http
{
    public class ApiGatewayOPES
    {
        public static string TOKEN = string.Empty;
        public static DateTime? TIMELIVE = null;
        public const double TIME_EXPIRE = 3600;

        public static string TOKEN_SIGN = string.Empty;
        public static DateTime? TIMELIVE_SIGN = null;
        public const double TIME_EXPIRE_SIGN = 3600;

        public static void GetTokenOpes(string ma_doi_tac_nsd)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (!string.IsNullOrEmpty(config.Domain) && !string.IsNullOrEmpty(config.EndPoint.Partner) && !string.IsNullOrEmpty(config.EndPoint.Password) 
                && (ApiGatewayOPES.TIMELIVE==null || ApiGatewayOPES.TIMELIVE >= DateTime.Now || string.IsNullOrEmpty(ApiGatewayOPES.TOKEN)))
            {
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    using (HttpClient httpClient = new HttpClient(clientHandler))
                    {
                        httpClient.BaseAddress = new Uri(config.Domain);
                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.EndPoint.Partner}:{config.EndPoint.Password}"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                        var response =  httpClient.PostAsJsonAsync("/token", new { }).Result;
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        OpesToken tokenMessage = JsonConvert.DeserializeObject<OpesToken>(jsonString);
                        if (tokenMessage.statusCode==0)
                        {
                            ApiGatewayOPES.TOKEN = tokenMessage.content.token;
                            if (tokenMessage.content.ttl > 900)
                                ApiGatewayOPES.TIMELIVE = DateTime.Now.AddSeconds(tokenMessage.content.ttl - 900);
                            else
                                ApiGatewayOPES.TIMELIVE = DateTime.Now.AddSeconds(tokenMessage.content.ttl);
                        }
                        else
                        {
                            ApiGatewayOPES.TOKEN = string.Empty;
                            ApiGatewayOPES.TIMELIVE = null;
                        }    
                    }
                }
                catch (Exception ex)
                {
                    ApiGatewayOPES.TOKEN = string.Empty;
                    ApiGatewayOPES.TIMELIVE = null;
                }
            }  
            else
            {
                ApiGatewayOPES.TOKEN = string.Empty;
                ApiGatewayOPES.TIMELIVE = null;
            }    
        }
        public static string GetTokenOpesString(string ma_doi_tac_nsd)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (!string.IsNullOrEmpty(config.Domain) && !string.IsNullOrEmpty(config.EndPoint.Partner) && !string.IsNullOrEmpty(config.EndPoint.Password))
            {
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    using (HttpClient httpClient = new HttpClient(clientHandler))
                    {
                        httpClient.BaseAddress = new Uri(config.Domain);
                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.EndPoint.Partner}:{config.EndPoint.Password}"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                        var response = httpClient.PostAsJsonAsync("/token", new { }).Result;
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        OpesToken tokenMessage = JsonConvert.DeserializeObject<OpesToken>(jsonString);
                        return tokenMessage.content.token;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public static void GetTokenOpesSignature(string ma_doi_tac_nsd)
        {
            if (ma_doi_tac_nsd == SignatureFileConfig.Partner && SignatureFileConfig.Enable && SignatureFileConfig.Online && !string.IsNullOrEmpty(SignatureFileConfig.Partner) && !string.IsNullOrEmpty(SignatureFileConfig.UrlSignature)
                && !string.IsNullOrEmpty(SignatureFileConfig.UrlToken) && !string.IsNullOrEmpty(SignatureFileConfig.UsernameToken) && !string.IsNullOrEmpty(SignatureFileConfig.PassToken)
                && (ApiGatewayOPES.TIMELIVE_SIGN == null || ApiGatewayOPES.TIMELIVE_SIGN >= DateTime.Now || string.IsNullOrEmpty(ApiGatewayOPES.TOKEN_SIGN)))
            {
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    using (HttpClient httpClient = new HttpClient(clientHandler))
                    {
                        httpClient.BaseAddress = new Uri(SignatureFileConfig.UrlToken);
                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = httpClient.PostAsJsonAsync("/api/auth/login", new { login = SignatureFileConfig.UsernameToken, password = SignatureFileConfig.PassToken }).Result;
                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        OpesTokenSignature tokenMessage = JsonConvert.DeserializeObject<OpesTokenSignature>(jsonString);
                        if (tokenMessage.result)
                        {
                            ApiGatewayOPES.TOKEN_SIGN = tokenMessage.data.token;
                            if (tokenMessage.data.ttl > 900)
                                ApiGatewayOPES.TIMELIVE_SIGN = DateTime.Now.AddSeconds(tokenMessage.data.ttl - 900);
                            else
                                ApiGatewayOPES.TIMELIVE_SIGN = DateTime.Now.AddSeconds(tokenMessage.data.ttl);
                        }
                        else
                        {
                            ApiGatewayOPES.TOKEN_SIGN = string.Empty;
                            ApiGatewayOPES.TIMELIVE_SIGN = null;
                        }
                    }

                }
                catch (Exception ex)
                {
                    ApiGatewayOPES.TOKEN_SIGN = string.Empty;
                    ApiGatewayOPES.TIMELIVE_SIGN = null;
                }
            }
            else
            {
                ApiGatewayOPES.TOKEN_SIGN = string.Empty;
                ApiGatewayOPES.TIMELIVE = null;
            }
        }
        public static string GetTokenOpesSignatureString(string ma_doi_tac_nsd)
        {
            if (ma_doi_tac_nsd == SignatureFileConfig.Partner && SignatureFileConfig.Enable && SignatureFileConfig.Online && !string.IsNullOrEmpty(SignatureFileConfig.Partner) && !string.IsNullOrEmpty(SignatureFileConfig.UrlSignature)
                && !string.IsNullOrEmpty(SignatureFileConfig.UrlToken) && !string.IsNullOrEmpty(SignatureFileConfig.UsernameToken) && !string.IsNullOrEmpty(SignatureFileConfig.PassToken))
            {
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    using (HttpClient httpClient = new HttpClient(clientHandler))
                    {
                        httpClient.BaseAddress = new Uri(SignatureFileConfig.UrlToken);
                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = httpClient.PostAsJsonAsync("/api/auth/login", new { login = SignatureFileConfig.UsernameToken, password = SignatureFileConfig.PassToken }).Result;
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        OpesTokenSignature tokenMessage = JsonConvert.DeserializeObject<OpesTokenSignature>(jsonString);
                        return tokenMessage.data.token;
                    }

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }



        public async static Task<byte[]> KySoFile(string ma_doi_tac_nsd, byte[] file, decimal? page, decimal? x, decimal? y, decimal? width, decimal? height, string signer)
        {
            try
            {
                if (string.IsNullOrEmpty(SignatureFileConfig.UrlSignature))
                    return null;
                string token = GetTokenOpesSignatureString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    using (MultipartFormDataContent form = new MultipartFormDataContent())
                    {
                        httpClient.BaseAddress = new Uri(SignatureFileConfig.UrlSignature);
                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        form.Add(new ByteArrayContent(file), "pdf_file", "pdf_file.pdf");
                        form.Add(new StringContent(page.Value.ToString()), "page");
                        form.Add(new StringContent(x.Value.ToString()), "x");
                        form.Add(new StringContent(y.Value.ToString()), "y");
                        form.Add(new StringContent(width.Value.ToString()), "width");
                        form.Add(new StringContent(height.Value.ToString()), "height");
                        form.Add(new StringContent(signer??""), "signer");
                        form.Add(new StringContent("{\"isShowSignTime\":true}"), "options");

                        var response = await httpClient.PostAsync("/api/signing/tracking", form);
                        var file_result = response.Content.ReadAsByteArrayAsync().Result;
                        return file_result;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IDictionary<string, object> RequestApiTruyVanXe(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain)|| string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
            dataRequest.AddWithExists("so_gcn", data.GetString("so_gcn"));
            dataRequest.AddWithExists("so_hdong", data.GetString("so_hdong"));
            dataRequest.AddWithExists("bien_so_xe", data.GetString("bien_so_xe"));
            dataRequest.AddWithExists("so_khung", data.GetString("so_khung"));
            dataRequest.AddWithExists("so_may", data.GetString("so_may"));
            dataRequest.AddWithExists("cmt_kh", data.GetString("cmt_kh"));
            dataRequest.AddWithExists("mst_kh", data.GetString("mst_kh"));
            dataRequest.AddWithExists("ngay_xr", data.GetString("ngay_xr"));
            return dataRequest;
        }
        public async static Task<string> CallApiTruyVanXe(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain)|| string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PostAsJsonAsync("/api/v1/insurance-certificate-information", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public static IDictionary<string, object> RequestApiThongTinGCN(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                return null;
            
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
            dataRequest.AddWithExists("so_id_hdong", data.GetString("so_id_hd"));
            dataRequest.AddWithExists("so_id_gcn", data.GetString("so_id_gcn"));
            return dataRequest;
        }
        public async static Task<string> CallApiThongTinGCN(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PostAsJsonAsync("/api/v1/insurance-certificate-information/integration", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public static IDictionary<string, object> RequestApiThanhToanPhi(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh_ql"));
            dataRequest.AddWithExists("so_id_hdong", data.GetString("so_id_hd"));
            return dataRequest;
        }
        public async static Task<string> CallApiThanhToanPhi(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain)  || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await httpClient.PostAsJsonAsync("/api/v1/insurance-certificate-information/payment", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public static IDictionary<string, object> RequestApiDanhSachFile(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
            dataRequest.AddWithExists("so_id_hdong", data.GetString("so_id"));
            dataRequest.AddWithExists("so_id_gcn", data.GetString("so_id_dt"));
            return dataRequest;
        }
        public async static Task<string> CallApiDanhSachFile(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await httpClient.PostAsJsonAsync("/api/v1/insurance-information/document", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public static IDictionary<string, object> RequestApiLayFile(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
            dataRequest.AddWithExists("so_id_hdong", data.GetString("so_id"));
            dataRequest.AddWithExists("ma_file", data.GetString("ma_file"));
            return dataRequest;
        }
        public async static Task<string> CallApiLayFile(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await httpClient.PostAsJsonAsync("/api/v1/insurance-information/document/detail", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public async static Task<string> CallApiChuyenDLBoiThuong(string ma_doi_tac_nsd, DuLieuBoiThuongOpes data)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain)|| string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PostAsJsonAsync("/api/v1/claim/synchronize-data/v2", data);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }catch(Exception ex)
            {
                return ex.Message;
            }
            
        }
        public static IDictionary<string, object> RequestApiSendSMS(bh_bt_gui_sms data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data.ma_doi_tac_nsd).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("sdt_nhan", data.sdt_nhan);
            dataRequest.AddWithExists("noi_dung", data.noi_dung);
            return dataRequest;
        }
        public async static Task<string> CallApiSendSMS(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;

                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PostAsJsonAsync("/api/v1/sms", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async static Task<string> AddConnect(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                IDictionary<string, object> dataRequestSend = new Dictionary<string, object>();
                dataRequestSend.AddWithExists("nd", dataRequest.GetString("ma_doi_tac_nsd")+"/"+ dataRequest.GetString("nsd"));
                dataRequestSend.AddWithExists("tb", dataRequest.GetString("id_ket_noi"));
                dataRequestSend.AddWithExists("token", dataRequest.GetString("token"));
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PostAsJsonAsync("/api/m/v1/app-user-device", dataRequestSend);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async static Task<string> RemoveConnect(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;
                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var user_id = dataRequest.GetString("ma_doi_tac_nsd") + "/" + dataRequest.GetString("nsd");
                    var device_id = dataRequest.GetString("id_ket_noi");
                    var response = await httpClient.PostAsJsonAsync($"/api/m/v1/app-user-device/del?user={user_id}&device={device_id}", new { });
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async static Task<string> SendNotify(string ma_doi_tac_nsd, SendNotifyDataOpesDataInfo dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;

                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PostAsJsonAsync("/api/m/v1/notification?type=USER", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    string logResponse = config.Domain2 + "/api/m/v1/notification?type=USER"+ Environment.NewLine;
                    logResponse+= ApiGatewayOPES.TOKEN + Environment.NewLine;
                    logResponse += jsonString;
                    return logResponse;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async static Task<string> YeuCauBSCT(string ma_doi_tac_nsd, opes_yc_bshs tai_lieu)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;

                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await httpClient.PostAsJsonAsync("/api/v1/additional-document", tai_lieu);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return "YC_BSCT_ERROR: " + ex.Message;
            }
        }
        public async static Task<string> KhachHangPhanHoiBSCT(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Domain2) || string.IsNullOrEmpty(config.EndPoint.Partner) || string.IsNullOrEmpty(config.EndPoint.Password))
                    return null;

                string token = GetTokenOpesString(ma_doi_tac_nsd);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("[OPES] - Không lấy được thông tin token");

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain2);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var so_id = dataRequest.GetString("so_id");
                    var response =  await httpClient.PostAsJsonAsync($"/api/v1/additional-document/{so_id}/uploading/successful", new { });
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return "KH_BSCT_ERROR: " + ex.Message;
            }
        }
    }
    public class OpesToken
    {
        public decimal? statusCode { get; set; }
        public string message { get; set; }
        public OpesTokenContent content { get; set; }
    }
    public class OpesTokenSignature
    {
        public bool result { get; set; }
        public decimal? numberCode { get; set; }
        public string stringCode { get; set; }
        public string message { get; set; }
        public OpesTokenContent data { get; set; }
    }
    public class OpesTokenContent
    {
        public string token { get; set; }
        public double ttl { get; set; }
    }
}
