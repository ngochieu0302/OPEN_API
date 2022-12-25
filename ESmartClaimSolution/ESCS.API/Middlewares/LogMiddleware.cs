using ESCS.BUS.Services;
using ESCS.COMMON.Common;
using ESCS.COMMON.ESCSServices;
using ESCS.COMMON.ESCSServices.Request;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESCS.API.Middlewares
{
    /// <summary>
    /// LogMiddleware
    /// </summary>
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        /// <summary>
        /// LogMiddleware
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logRequestService"></param>
        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }
        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="logRequestService"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext, ILogMongoService<LogRequest> logRequestService, ILogMongoService<LogResponse> logResponseService)
        {
            string code = Guid.NewGuid().ToString("N").ToLower();
            await LogRequest(httpContext, logRequestService, code);
            await LogResponse(httpContext, logResponseService, code);
        }
        /// <summary>
        /// Log Reuqest
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task LogRequest(HttpContext httpContext, ILogMongoService<LogRequest> logRequestService, string code)
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                if (MongoConnection.UseMongo && MongoConnection.UseLog)
                {
                    var headerDic = httpContext.Request.Headers;
                    if (headerDic != null)
                        foreach (var item in headerDic)
                            headers.Add(item.Key, item.Value);
                }
                httpContext.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await httpContext.Request.Body.CopyToAsync(requestStream);
                #region Log SQL Server
                //if (ESCSServiceConfig.UseService)
                //{
                //    pht_log_ung_dung_nh_param rq = new pht_log_ung_dung_nh_param();
                //    rq.path = httpContext.Request.Path;
                //    if (!string.IsNullOrEmpty(rq.path) && !rq.path.Contains("/upload-file"))
                //        rq.body_request = ReadStreamInChunks(requestStream);
                //    else
                //        rq.body_request = "";

                //    Task task = new Task(async () =>
                //    {
                //        try
                //        {
                //            Dictionary<string, string> headers = new Dictionary<string, string>();
                //            var headerDic = httpContext.Request.Headers;
                //            if (headerDic != null)
                //                foreach (var item in headerDic)
                //                    headers.Add(item.Key, item.Value);

                //            rq.code = code;
                //            rq.headers = JsonConvert.SerializeObject(headers);
                //            rq.scheme = httpContext.Request.Scheme;
                //            rq.host = httpContext.Request.Host.ToString();
                //            rq.query_string = httpContext.Request.QueryString.ToString();
                //            rq.time_request = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                //            rq.time_response = 0;
                //            await ESCSService.AddLogRequestResponse(rq);
                //        }
                //        catch { }
                //    });
                //    task.Start();
                //}
                #endregion
                requestStream.Dispose();
                #region Log MongoDB
                if (MongoConnection.UseMongo && MongoConnection.UseLog)
                {
                    Task logRQ = new Task(() =>
                    {
                        try
                        {
                            LogRequest rq = new LogRequest();
                            rq.id = code;
                            rq.headers = headers;
                            rq.schema = httpContext.Request.Scheme;
                            rq.host = httpContext.Request.Host.ToString();
                            rq.path = httpContext.Request.Path;
                            rq.query_string = httpContext.Request.QueryString.ToString();
                            if (!string.IsNullOrEmpty(rq.path))
                            {
                                if (!rq.path.Contains("/upload-file"))
                                    rq.body = ReadStreamInChunks(requestStream);
                                else
                                {
                                    string str = ReadStreamInChunks(requestStream);
                                    rq.body = str.Length <= 1000 ? str : str.Substring(0, 1000);
                                }
                            }
                            logRequestService.AddLogAsync(rq);
                        }
                        catch { }
                    });
                    logRQ.Start();
                }
                #endregion
                httpContext.Request.Body.Position = 0;
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Log Reponse
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task LogResponse(HttpContext httpContext, ILogMongoService<LogResponse> logResponseService, string code)
        {
            if (MongoConnection.UseMongo && MongoConnection.UseLog)
            {
                var originalBodyStream = httpContext.Response.Body;
                await using var responseBody = _recyclableMemoryStreamManager.GetStream();
                //httpContext.Response.Body = responseBody;
                await _next(httpContext);
                try
                {
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    var text = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);

                    //Dictionary<string, string> headers = new Dictionary<string, string>();
                    //var headerDic = httpContext.Request.Headers;
                    //if (headerDic != null)
                    //    foreach (var item in headerDic)
                    //        headers.Add(item.Key, item.Value);
                    
                    //LogResponse res = new LogResponse();
                    //res.id = code;
                    //res.headers = headers;
                    //res.schema = httpContext.Request.Scheme;
                    //res.host = httpContext.Request.Host.ToString();
                    //res.path = httpContext.Request.Path;
                    //res.query_string = httpContext.Request.QueryString.ToString();
                    //res.body = text;
                    //if (res.path.Contains("/upload-file") || text.Length > 1000)
                    //    res.body = text.Length <= 1000 ? text : text.Substring(0, 1000);
                    //logResponseService.AddLogAsync(res);
                    
                }
                catch
                {

                }
            }
            else
            {
                await _next(httpContext);
                httpContext.Request.Body.Dispose();
                httpContext.Response.Body.Dispose();
                GC.SuppressFinalize(this);
            }    
            #region Log SQL Server
            //if (ESCSServiceConfig.UseService)
            //{
            //    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            //    var text = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            //    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            //    try
            //    {
            //        Task task = new Task(async () =>
            //        {
            //            try
            //            {
            //                pht_log_ung_dung_nh_param res = new pht_log_ung_dung_nh_param();
            //                res.code = code;
            //                if (text.Length < 10000)
            //                    res.body_response = text;
            //                else
            //                    res.body_response = "Response lớn hơn 10.000 ký tự";
            //                res.time_request = 0;
            //                res.time_response = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            //                await ESCSService.AddLogRequestResponse(res);
            //            }
            //            catch { }
            //        });
            //        task.Start();
            //    }
            //    catch
            //    {
            //    }

            //}
            //await responseBody.CopyToAsync(originalBodyStream);
            #endregion
        }
        /// <summary>
        /// ReadStreamInChunks
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadStreamInChunks(Stream stream)
        {
            try
            {
                const int readChunkBufferLength = 4096;
                stream.Seek(0, SeekOrigin.Begin);

                using var textWriter = new StringWriter();
                using var reader = new StreamReader(stream);

                var readChunk = new char[readChunkBufferLength];
                int readChunkLength;

                do
                {
                    readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                    textWriter.Write(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                return textWriter.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
    /// <summary>
    /// LogMiddlewareExtensions
    /// </summary>
    public static class LogMiddlewareExtensions
    {
        /// <summary>
        /// UseLog
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}
