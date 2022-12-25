using System;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.ExceptionHandlers;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Response;
using ESCS.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

/// <summary>
/// Không có kết nối ra server bên ngoài
/// </summary>
namespace ESCS.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorHandlerController : Controller
    {
        private readonly ILogMongoService<LogException> _logRequestService;
        public readonly IErrorCodeService _errorCodeService;
        public readonly ICacheServer _cacheServer;
        private readonly IHubContext<NotifyMessageHub> _notificationHubContext;
        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorHandlerController(
            ILogMongoService<LogException> logRequestService,
            IErrorCodeService errorCodeService,
            ICacheServer cacheServer,
            IHubContext<NotifyMessageHub> notificationHubContext
         )
        {
            _logRequestService = logRequestService;
            _errorCodeService = errorCodeService;
            _cacheServer = cacheServer;
            _notificationHubContext = notificationHubContext;
        }
        /// <summary>
        /// All exception will come here
        /// </summary>
        /// <returns></returns>
        [Route("error")]
        [AllowAnonymous]
        public async Task<IActionResult> Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Exception exception = context?.Error;
            //_logRequestService.AddLogAsync(new LogException("code", exception.ToString()));
            BaseResponse<object> response = new BaseResponse<object>();
            response.state_info.status = "NotOK";
            response.state_info.message_code = "500";
            response.state_info.message_body = exception.Message;
            if (exception is OracleDbException)
            {
                var ex = exception as OracleDbException;
                response.state_info.message_body = ex.Message;
                if (ex.Message=="INVALID_AUTH")
                {
                    response.state_info.message_body = "Lỗi xác thực tài khoản";
                    response.state_info.message_code = "INVALID_AUTH";
                }
                if (ex.Message == "INVALID_DEVICE")
                {
                    response.state_info.message_body = "Tài khoản đang được đăng nhập ở thiết bị khác.";
                    response.state_info.message_code = "INVALID_DEVICE";
                }
            }
            else
            {
                await _notificationHubContext.Clients.All.SendAsync("ReceiveNotify", "Error system", "Lỗi ứng dụng", exception.ToString());
            }
            return Ok(response);
        }
    }
}
