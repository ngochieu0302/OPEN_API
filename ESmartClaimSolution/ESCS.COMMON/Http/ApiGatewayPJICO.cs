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
    public class ApiGatewayPJICO
    {
        public static string TOKEN = string.Empty;
        public static DateTime? TIMELIVE = null;
        public const double TIME_EXPIRE = 86399;
        public static void GetTokenPJICO(string ma_doi_tac_nsd)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
            if (!string.IsNullOrEmpty(config.Domain) && config.EndPoint != null && !string.IsNullOrEmpty(config.EndPoint.Partner)
                && !string.IsNullOrEmpty(config.EndPoint.Password)
                && (ApiGatewayPJICO.TIMELIVE == null || ApiGatewayPJICO.TIMELIVE >= DateTime.Now || string.IsNullOrEmpty(ApiGatewayPJICO.TOKEN)))
            {
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    using (HttpClient httpClient = new HttpClient(clientHandler))
                    {
                        httpClient.BaseAddress = new Uri(config.Domain);
                        httpClient.DefaultRequestHeaders.Clear();
                        var data = new[]
                        {
                            new KeyValuePair<string, string>("username", config.EndPoint.Partner),
                            new KeyValuePair<string, string>("password", config.EndPoint.Password),
                            new KeyValuePair<string, string>("grant_type", "password"),
                        };
                        var response = httpClient.PostAsync("/token", new FormUrlEncodedContent(data)).Result;
                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        PjicoToken tokenMessage = JsonConvert.DeserializeObject<PjicoToken>(jsonString);

                        if (string.IsNullOrEmpty(tokenMessage.error))
                        {
                            ApiGatewayPJICO.TOKEN = tokenMessage.access_token;
                            if (tokenMessage.expires_in > 600)
                                ApiGatewayPJICO.TIMELIVE = DateTime.Now.AddSeconds(tokenMessage.expires_in - 600);
                            else
                                ApiGatewayPJICO.TIMELIVE = DateTime.Now.AddSeconds(tokenMessage.expires_in);
                        }
                        else
                        {
                            ApiGatewayPJICO.TOKEN = string.Empty;
                            ApiGatewayPJICO.TIMELIVE = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ApiGatewayPJICO.TOKEN = string.Empty;
                    ApiGatewayPJICO.TIMELIVE = null;
                }
            }
            else
            {
                ApiGatewayPJICO.TOKEN = string.Empty;
                ApiGatewayPJICO.TIMELIVE = null;
            }
        }

        public static IDictionary<string, object> RequestApiTruyVanXe(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("DIVN_CODE", data.GetString("ma_chi_nhanh"));
            dataRequest.AddWithExists("POL_NO", data.GetString("so_gcn"));
            dataRequest.AddWithExists("CERT_NO", data.GetString("so_hdong"));
            dataRequest.AddWithExists("REGN_NO", data.GetString("bien_so_xe"));
            dataRequest.AddWithExists("CHASSIS_NO", data.GetString("so_khung"));
            dataRequest.AddWithExists("ENGINE_NO", data.GetString("so_may"));
            dataRequest.AddWithExists("CUST_ID", data.GetString("cmt_kh"));
            dataRequest.AddWithExists("CUST_TAX", data.GetString("mst_kh"));
            dataRequest.AddWithExists("DAME_DT", Utilities.chuanHoaNgayPjico(data.GetString("ngay_xr")));
            return dataRequest;
        }
        public async static Task<string> CallApiTruyVanXe(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || config.EndPoint == null || string.IsNullOrEmpty(config.EndPoint.Partner) 
                    || string.IsNullOrEmpty(config.EndPoint.Partner))
                    return null;

                GetTokenPJICO(ma_doi_tac_nsd);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient client = new HttpClient(clientHandler);
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiGatewayPJICO.TOKEN);

                    var response = await httpClient.PostAsJsonAsync("/api/policy/bt/get", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static IDictionary<string, object> RequestApiThongTinGCN(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("POL_DIVN_CODE", data.GetString("ma_chi_nhanh"));
            dataRequest.AddWithExists("POL_SYS_ID", data.GetString("so_id_gcn"));
            return dataRequest;
        }
        public async static Task<string> CallApiThongTinGCN(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || config.EndPoint == null || string.IsNullOrEmpty(config.EndPoint.Partner)
                    || string.IsNullOrEmpty(config.EndPoint.Partner))
                    return null;
                GetTokenPJICO(ma_doi_tac_nsd);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient client = new HttpClient(clientHandler);
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiGatewayPJICO.TOKEN);
                    var response = await httpClient.PostAsJsonAsync("/api/policy/bt/get_detail", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static IDictionary<string, object> RequestApiTruyVanNguoi(IDictionary<string, object> data)
        {
            var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(config.Domain))
                return null;
            IDictionary<string, object> dataRequest = new Dictionary<string, object>();
            dataRequest.AddWithExists("DIVN_CODE", data.GetString("ma_chi_nhanh"));
            dataRequest.AddWithExists("POL_NO", data.GetString("gcn"));
            dataRequest.AddWithExists("CERT_NO", data.GetString("so_hd"));
            dataRequest.AddWithExists("REGN_NO", data.GetString("ten_kh"));
            dataRequest.AddWithExists("CHASSIS_NO", data.GetString("ngay_sinh"));
            dataRequest.AddWithExists("ENGINE_NO", data.GetString("lhnv"));
            dataRequest.AddWithExists("CUST_ID", data.GetString("so_cmt"));
            dataRequest.AddWithExists("CUST_TAX", "");
            dataRequest.AddWithExists("DAME_DT", Utilities.chuanHoaNgayPjico(data.GetString("ngay_vv")));
            return dataRequest;
        }
        public async static Task<string> CallApiTruyVanNguoi(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        {
            try
            {
                var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
                if (string.IsNullOrEmpty(config.Domain) || config.EndPoint == null || string.IsNullOrEmpty(config.EndPoint.Partner)
                    || string.IsNullOrEmpty(config.EndPoint.Partner))
                    return null;

                GetTokenPJICO(ma_doi_tac_nsd);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient client = new HttpClient(clientHandler);
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(config.Domain);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiGatewayPJICO.TOKEN);

                    var response = await httpClient.PostAsJsonAsync("/api/policy/bt/get", dataRequest);
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return jsonString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }




        #region Chưa làm đến
        //public static IDictionary<string, object> RequestApiTruyVanNguoi(IDictionary<string, object> data)
        //{
        //    var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
        //    if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //        return null;
        //    var checksum = CheckSumCommon.MICTruyVanNguoi(config.Secret, config.EndPoint.Partner, data.GetString("ma_chi_nhanh"), data.GetString("gcn"),
        //                                                data.GetString("ngay_sinh"), data.GetString("nd_tim"), data.GetString("lhnv"));
        //    IDictionary<string, object> dataRequest = new Dictionary<string, object>();
        //    dataRequest.AddWithExists("ma_dvi", config.EndPoint.Partner);
        //    dataRequest.AddWithExists("pas", config.EndPoint.Password);
        //    dataRequest.AddWithExists("checksum", checksum);
        //    dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
        //    dataRequest.AddWithExists("so_gcn", data.GetString("gcn"));
        //    dataRequest.AddWithExists("so_hdong", data.GetString("so_hd"));
        //    dataRequest.AddWithExists("ten_ndbh", data.GetString("ten_kh"));
        //    dataRequest.AddWithExists("ngay_sinh", data.GetString("ngay_sinh"));
        //    dataRequest.AddWithExists("cmt_ndbh", data.GetString("nd_tim"));
        //    dataRequest.AddWithExists("ctrinh", data.GetString("lhnv"));
        //    return dataRequest;
        //}
        //public async static Task<string> CallApiTruyVanNguoi(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/truyvan_connguoi", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        //public static IDictionary<string, object> RequestApiThongTinGCNNguoi(IDictionary<string, object> data)
        //{
        //    var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
        //    if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //        return null;
        //    var checksum = CheckSumCommon.MICThongTinGCNNguoi(config.Secret, config.EndPoint.Partner, data.GetString("ma_chi_nhanh"), data.GetString("so_id_hd"),
        //                                                data.GetString("so_id_gcn"));
        //    IDictionary<string, object> dataRequest = new Dictionary<string, object>();
        //    dataRequest.AddWithExists("ma_dvi", config.EndPoint.Partner);
        //    dataRequest.AddWithExists("pas", config.EndPoint.Password);
        //    dataRequest.AddWithExists("checksum", checksum);
        //    dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
        //    dataRequest.AddWithExists("so_id_hdong", data.GetString("so_id_hd"));
        //    dataRequest.AddWithExists("so_id_gcn", data.GetString("so_id_gcn"));
        //    return dataRequest;
        //}
        //public async static Task<string> CallApiThongTinGCNNguoi(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/xem_ttin_connguoi", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        //public static IDictionary<string, object> RequestApiThanhToanPhi(IDictionary<string, object> data)
        //{
        //    var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
        //    if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //        return null;
        //    var checksum = CheckSumCommon.MICThongTinThanhToanPhi(config.Secret, config.EndPoint.Partner, data.GetString("so_id_hd"));
        //    IDictionary<string, object> dataRequest = new Dictionary<string, object>();
        //    dataRequest.AddWithExists("ma_dvi", config.EndPoint.Partner);
        //    dataRequest.AddWithExists("pas", config.EndPoint.Password);
        //    dataRequest.AddWithExists("checksum", checksum);
        //    dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh_ql"));
        //    dataRequest.AddWithExists("so_id_hd", data.GetString("so_id_hd"));
        //    return dataRequest;
        //}
        //public async static Task<string> CallApiThanhToanPhi(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/truyvan_tt", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        //public static IDictionary<string, object> RequestApiDanhSachFile(IDictionary<string, object> data)
        //{
        //    var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
        //    if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //        return null;
        //    IDictionary<string, object> dataRequest = new Dictionary<string, object>();
        //    var checksum = CheckSumCommon.MICDanhSachFile(config.Secret, config.EndPoint.Partner, data.GetString("ma_chi_nhanh"), data.GetString("so_id"),
        //                                               data.GetString("so_id_dt"));
        //    dataRequest.AddWithExists("ma_dvi", config.EndPoint.Partner);
        //    dataRequest.AddWithExists("pas", config.EndPoint.Password);
        //    dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
        //    dataRequest.AddWithExists("so_id_hdong", data.GetString("so_id"));
        //    dataRequest.AddWithExists("so_id_gcn", data.GetString("so_id_dt"));
        //    dataRequest.AddWithExists("checksum", checksum);
        //    return dataRequest;
        //}
        //public async static Task<string> CallApiDanhSachFile(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/dsach_file", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        //public static IDictionary<string, object> RequestApiLayFile(IDictionary<string, object> data)
        //{
        //    var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
        //    if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //        return null;
        //    IDictionary<string, object> dataRequest = new Dictionary<string, object>();
        //    var checksum = CheckSumCommon.MICLayFile(config.Secret, config.EndPoint.Partner, data.GetString("ma_chi_nhanh"), data.GetString("so_id"),
        //                                               data.GetString("ma_file"));
        //    dataRequest.AddWithExists("ma_dvi", config.EndPoint.Partner);
        //    dataRequest.AddWithExists("pas", config.EndPoint.Password);
        //    dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
        //    dataRequest.AddWithExists("so_id_hd", data.GetString("so_id"));
        //    dataRequest.AddWithExists("ma_file", data.GetString("ma_file"));
        //    dataRequest.AddWithExists("checksum", checksum);
        //    return dataRequest;
        //}
        //public async static Task<string> CallApiLayFile(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/xem_file", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        //public async static Task<string> CallApiChuyenDLBoiThuong(string ma_doi_tac_nsd, DuLieuBoiThuong data)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        var checksum = CheckSumCommon.MICChuyenDLBoiThuong(config.Secret, config.EndPoint.Partner, data.hs.ma_chi_nhanh, data.hs.so_id_hs);
        //        data.ma_dvi = config.EndPoint.Partner;
        //        data.pas = config.EndPoint.Password;
        //        data.checksum = checksum;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/du_lieu_boi_thuong", data);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }catch(Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //}
        //public async static Task<string> CallApiChuyenDLBoiThuongConNguoiCu(string ma_doi_tac_nsd, DuLieuBoiThuongConNguoi_CU_TICH_HOP data)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        var checksum = CheckSumCommon.MICChuyenDLBoiThuong(config.Secret, config.EndPoint.Partner, data.hs.ma_chi_nhanh, data.hs.so_id_hs);
        //        data.ma_dvi = config.EndPoint.Partner;
        //        data.pas = config.EndPoint.Password;
        //        data.checksum = checksum;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/du_lieu_boi_thuong", data);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //}
        //public async static Task<string> CallApiChuyenDLBoiThuongConNguoi(string ma_doi_tac_nsd, DuLieuBoiThuongConNguoi data)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.Domain) || string.IsNullOrEmpty(config.Secret))
        //            return null;
        //        var checksum = CheckSumCommon.MICChuyenDLBoiThuong(config.Secret, config.EndPoint.Partner, data.hs.ma_chi_nhanh, data.hs.so_id_hs);
        //        data.ma_dvi = config.EndPoint.Partner;
        //        data.pas = config.EndPoint.Password;
        //        data.checksum = checksum;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.Domain);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/du_lieu_boi_thuong_cnguoi", data);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }

        //}
        //public static IDictionary<string, object> RequestApiSendSMS(bh_bt_gui_sms data)
        //{
        //    var config = CoreApiConfig.Items.Where(n => n.Partner == data.ma_doi_tac_nsd.ToString()).FirstOrDefault();
        //    if (string.IsNullOrEmpty(config.DomainMailSMS) || string.IsNullOrEmpty(config.SecretMailSMS))
        //        return null;
        //    var checksum = CheckSumCommon.MICTichHopEmailSMS(config.SecretMailSMS, config.EndPoint.Partner);
        //    IDictionary<string, object> dataRequest = new Dictionary<string, object>();
        //    dataRequest.AddWithExists("ma_dvi", config.EndPoint.Partner);
        //    dataRequest.AddWithExists("pas", config.PassSMS);
        //    dataRequest.AddWithExists("nsd", config.NsdSMS);
        //    dataRequest.AddWithExists("checksum", checksum);
        //    dataRequest.AddWithExists("loai", "S");
        //    dataRequest.AddWithExists("from_mail", "");
        //    dataRequest.AddWithExists("toi", data.sdt_nhan);
        //    dataRequest.AddWithExists("title", "");
        //    dataRequest.AddWithExists("nd", data.noi_dung);
        //    return dataRequest;
        //}
        //public async static Task<string> CallApiSendSMS(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.DomainMailSMS) || string.IsNullOrEmpty(config.SecretMailSMS))
        //            return null;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.DomainMailSMS);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/mic_sms_email", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        //public static IDictionary<string, object> RequestApiSendEmail(IDictionary<string, object> data)
        //{
        //    var config = CoreApiConfig.Items.Where(n => n.Partner == data["ma_doi_tac_nsd"].ToString()).FirstOrDefault();
        //    if (string.IsNullOrEmpty(config.DomainMailSMS) || string.IsNullOrEmpty(config.SecretMailSMS))
        //        return null;
        //    var checksum = CheckSumCommon.MICSendSMS(config.Secret, config.EndPoint.Partner, data.GetString("ma_chi_nhanh"), data.GetString("so_gcn"),
        //                                                data.GetString("so_hdong"), data.GetString("bien_so_xe"), data.GetString("so_khung"), data.GetString("so_may"), data.GetString("cmt_kh"),
        //                                                data.GetString("mst_kh"));
        //    IDictionary<string, object> dataRequest = new Dictionary<string, object>();
        //    dataRequest.AddWithExists("ma_dvi", config.EndPoint.Partner);
        //    dataRequest.AddWithExists("pas", config.PassSMS);
        //    dataRequest.AddWithExists("checksum", checksum);
        //    dataRequest.AddWithExists("ma_chi_nhanh", data.GetString("ma_chi_nhanh"));
        //    dataRequest.AddWithExists("so_gcn", data.GetString("so_gcn"));
        //    dataRequest.AddWithExists("so_hdong", data.GetString("so_hdong"));
        //    dataRequest.AddWithExists("bien_so_xe", data.GetString("bien_so_xe"));
        //    dataRequest.AddWithExists("so_khung", data.GetString("so_khung"));
        //    dataRequest.AddWithExists("so_may", data.GetString("so_may"));
        //    dataRequest.AddWithExists("cmt_kh", data.GetString("cmt_kh"));
        //    dataRequest.AddWithExists("mst_kh", data.GetString("mst_kh"));
        //    dataRequest.AddWithExists("ngay_xr", data.GetString("ngay_xr"));
        //    return dataRequest;
        //}
        //public async static Task<string> CallApiSendEmail(string ma_doi_tac_nsd, IDictionary<string, object> dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.DomainMailSMS) || string.IsNullOrEmpty(config.SecretMailSMS))
        //            return null;
        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);

        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.DomainMailSMS);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/mic_sms_email", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        //public async static Task<string> CallApiSendNotify(string ma_doi_tac_nsd, SendNotifyDataMIC dataRequest)
        //{
        //    try
        //    {
        //        var config = CoreApiConfig.Items.Where(n => n.Partner == ma_doi_tac_nsd).FirstOrDefault();
        //        if (string.IsNullOrEmpty(config.DomainNotify) || string.IsNullOrEmpty(config.AuthenNotify.Token))
        //            return null;

        //        HttpClientHandler clientHandler = new HttpClientHandler();
        //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //        HttpClient client = new HttpClient(clientHandler);
        //        using (HttpClient httpClient = new HttpClient(clientHandler))
        //        {
        //            httpClient.BaseAddress = new Uri(config.DomainNotify);
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Add("Authority", config.AuthenNotify.Token);
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var response = await httpClient.PostAsJsonAsync("/api/notifications", dataRequest);
        //            var jsonString = response.Content.ReadAsStringAsync().Result;
        //            return jsonString;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        #endregion
    }
    public class PjicoToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public double expires_in { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }
}
