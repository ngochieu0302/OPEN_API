using ESCS.COMMON.MongoDb;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.API.Middlewares
{
    public class RequestResponseLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestResponseLoggerOption _options;
        private readonly IRequestResponseLogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggerMiddleware
        (RequestDelegate next, IOptions<RequestResponseLoggerOption> options,
         IRequestResponseLogger logger)
        {
            _next = next;
            _options = options.Value;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext httpContext, IRequestResponseLogModelCreator logCreator, ILogMongoService<RequestResponseLogModel> logMongo)
        {
            // Middleware is enabled only when the 
            // EnableRequestResponseLogging config value is set.
            if (_options == null || !_options.IsEnabled || !MongoConnection.UseMongo || !MongoConnection.UseLog)
            {
                httpContext.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await httpContext.Request.Body.CopyToAsync(requestStream);
                requestStream.Dispose();
                httpContext.Request.Body.Position = 0;
                await _next(httpContext);
                httpContext.Request.Body.Dispose();
                httpContext.Response.Body.Dispose();
                GC.SuppressFinalize(this);
            }
            else
            {
                RequestResponseLogModel log = logCreator.LogModel;
                log.RequestDateTimeUtc = DateTime.UtcNow;
                log.RequestDateTimeNumber = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                httpContext.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                HttpRequest request = httpContext.Request;
                #region Log
                log.LogId = Guid.NewGuid().ToString();
                log.TraceId = httpContext.TraceIdentifier;
                var ip = request.HttpContext.Connection.RemoteIpAddress;
                log.ClientIp = ip == null ? null : ip.ToString();
                log.Node = _options.Name;
                #endregion
                #region Log Request
                log.RequestMethod = request.Method;
                log.RequestPath = request.Path;
                log.RequestQuery = request.QueryString.ToString();
                log.RequestQueries = FormatQueries(request.QueryString.ToString());
                log.RequestHeaders = FormatHeaders(request.Headers);
                log.RequestBody = await ReadBodyFromRequest(request);
                log.RequestScheme = request.Scheme;
                log.RequestHost = request.Host.ToString();
                log.RequestContentType = request.ContentType;
                await httpContext.Request.Body.CopyToAsync(requestStream);
                requestStream.Dispose();
                httpContext.Request.Body.Position = 0;
                #endregion
                // Temporarily replace the HttpResponseStream, 
                // which is a write-only stream, with a MemoryStream to capture 
                // its value in-flight.
                HttpResponse response = httpContext.Response;
                var originalResponseBody = response.Body;
                using var newResponseBody = new MemoryStream();
                response.Body = newResponseBody;
                // Call the next middleware in the pipeline
                try
                {
                    await _next(httpContext);
                }
                catch (Exception exception)
                {
                    /*exception: but was not managed at app.UseExceptionHandler() 
                      or by any middleware*/
                    LogError(log, exception);
                }
                newResponseBody.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(response.Body).ReadToEndAsync();
                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);
                #region Log Response
                log.ResponseContentType = response.ContentType;
                log.ResponseStatus = response.StatusCode.ToString();
                log.ResponseHeaders = FormatHeaders(response.Headers);
                log.ResponseBody = responseBodyText;
                log.ResponseDateTimeUtc = DateTime.UtcNow;
                log.ResponseDateTimeNumber = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                #endregion
                #region Log Exception
                /*exception: but was managed at app.UseExceptionHandler() 
                  or by any middleware*/
                var contextFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (contextFeature != null && contextFeature.Error != null)
                {
                    Exception exception = contextFeature.Error;
                    LogError(log, exception);
                }
                logMongo.AddLogAsync(log);
                //var jsonString = logCreator.LogString(); /*log json*/
                //_logger.Log(logCreator);
                #endregion
            }

        }

        private void LogError(RequestResponseLogModel log, Exception exception)
        {
            log.ExceptionMessage = exception.Message;
            log.ExceptionStackTrace = exception.StackTrace;
        }
        private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                pairs.Add(header.Key, header.Value);
            }
            return pairs;
        }
        private List<KeyValuePair<string, string>> FormatQueries(string queryString)
        {
            List<KeyValuePair<string, string>> pairs =
                 new List<KeyValuePair<string, string>>();
            string key, value;
            foreach (var query in queryString.TrimStart('?').Split("&"))
            {
                var items = query.Split("=");
                key = items.Count() >= 1 ? items[0] : string.Empty;
                value = items.Count() >= 2 ? items[1] : string.Empty;
                if (!String.IsNullOrEmpty(key))
                {
                    pairs.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            return pairs;
        }
        private async Task<string> ReadBodyFromRequest(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times 
            // (for the next middlewares in the pipeline).
            request.EnableBuffering();
            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            var requestBody = await streamReader.ReadToEndAsync();
            // Reset the request's body stream position for 
            // next middleware in the pipeline.
            request.Body.Position = 0;
            return requestBody;
        }
    }
    public interface IRequestResponseLogModelCreator
    {
        RequestResponseLogModel LogModel { get; }
        string LogString();
    }
    public interface IRequestResponseLogger
    {
        void Log(IRequestResponseLogModelCreator logCreator);
    }
    public class RequestResponseLogModelCreator : IRequestResponseLogModelCreator
    {
        public RequestResponseLogModel LogModel { get; private set; }
        public RequestResponseLogModelCreator()
        {
            LogModel = new RequestResponseLogModel();
        }
        public string LogString()
        {
            var jsonString = JsonConvert.SerializeObject(LogModel);
            return jsonString;
        }
    }
    public class RequestResponseLogger : IRequestResponseLogger
    {
        private readonly ILogger<RequestResponseLogger> _logger;
        public RequestResponseLogger(ILogger<RequestResponseLogger> logger)
        {
            _logger = logger;
        }
        public void Log(IRequestResponseLogModelCreator logCreator)
        {
            //_logger.LogTrace(logCreator.LogString());
            _logger.LogInformation(logCreator.LogString());
            //_logger.LogWarning(logCreator.LogString());
            //_logger.LogCritical(logCreator.LogString());
        }
    }
}
