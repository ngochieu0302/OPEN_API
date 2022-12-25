using ESCS.COMMON.Common;
using ESCS.COMMON.ESCSServices.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.COMMON.ESCSServices
{
    public class ESCSService
    {
        public static async Task AddLogTichHop(string ma_doi_tac, string nsd, string ma_api, string data_request, string data_response)
        {
            try
            {
                if (!ESCSServiceConfig.UseService)
                    return;

                pbh_tich_hop_api_log_nh_param param = new pbh_tich_hop_api_log_nh_param();
                param.service_ma_doi_tac = ESCSServiceConfig.Partner;
                param.service_token = ESCSServiceConfig.Token;
                param.service_nsd = ESCSServiceConfig.User;
                param.service_pas = ESCSServiceConfig.Pas;

                param.ma_doi_tac = ma_doi_tac;
                param.ma_api = ma_api;
                param.data_request = data_request;
                param.data_response = data_response;
                param.nsd = nsd;

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient client = new HttpClient(clientHandler);
                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    httpClient.BaseAddress = new Uri(ESCSServiceConfig.BaseUrl);
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    await httpClient.PostAsJsonAsync("/api/log/add", param);
                }
            }
            catch (Exception ex)
            {
               
            }
        }
        public static async Task AddLogRequestResponse(pht_log_ung_dung_nh_param log)
        {
            if (!ESCSServiceConfig.UseService)
                return;
            log.service_ma_doi_tac = ESCSServiceConfig.Partner;
            log.service_token = ESCSServiceConfig.Token;
            log.service_nsd = ESCSServiceConfig.User;
            log.service_pas = ESCSServiceConfig.Pas;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            using (HttpClient httpClient = new HttpClient(clientHandler))
            {
                httpClient.BaseAddress = new Uri(ESCSServiceConfig.BaseUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await httpClient.PostAsJsonAsync("/api/log/request", log);
            }
            //Task task = new Task(async () =>
            //{
            //    try
            //    {
            //        log.service_ma_doi_tac = ESCSServiceConfig.Partner;
            //        log.service_token = ESCSServiceConfig.Token;
            //        log.service_nsd = ESCSServiceConfig.User;
            //        log.service_pas = ESCSServiceConfig.Pas;

            //        HttpClientHandler clientHandler = new HttpClientHandler();
            //        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            //        HttpClient client = new HttpClient(clientHandler);
            //        using (HttpClient httpClient = new HttpClient(clientHandler))
            //        {
            //            httpClient.BaseAddress = new Uri(ESCSServiceConfig.BaseUrl);
            //            httpClient.DefaultRequestHeaders.Clear();
            //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //            await httpClient.PostAsJsonAsync("/api/log/request", log);
            //        }
            //    }
            //    catch
            //    {

            //    }
            //});
            //task.Start();
        }
    }
}
