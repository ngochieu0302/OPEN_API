using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.COMMON.Auth;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExceptionHandlers;
using ESCS.COMMON.Http;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Response;
using ESCS.MODEL.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using ESCS.OPENID.Attributes;
using ESCS.OPENID.Hubs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ESCS.OPENID.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        public readonly IErrorCodeService _errorCodeService;
        public readonly ICacheServer _cacheServer;
        private readonly IHubContext<NotifyMessageHub> _notificationHubContext;
        public HomeController(
            IAuthenticationService authenticationService,
            IErrorCodeService errorCodeService,
            ICacheServer cacheServer,
            IHubContext<NotifyMessageHub> notificationHubContext
         )
        {
            _authenticationService = authenticationService;
            _errorCodeService = errorCodeService;
            _cacheServer = cacheServer;
            _notificationHubContext = notificationHubContext;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Action");
        }
        /// <summary>
        /// Error
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Exception exception = context?.Error;
            BaseResponse<object> response = new BaseResponse<object>();
            response.state_info.status = "NotOK";
            response.state_info.message_code = "500";
            response.state_info.message_body = exception.Message;
            if (exception is OracleDbException)
            {
                var ex = exception as OracleDbException;
                string errorCache = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, "ERROCODE:" + ex.Message, RedisCacheMaster.DatabaseIndex);
                response.state_info.message_body = ex.Message;
                sys_error_code error = new sys_error_code();
                if (!string.IsNullOrEmpty(errorCache))
                {
                    error = JsonConvert.DeserializeObject<sys_error_code>(errorCache);
                    response.state_info.message_body = error.error_message;
                }
                else
                {
                    error = await _errorCodeService.Get(new sys_error_code() { server_error_code = ex.Message });
                    if (error!=null)
                    {
                        response.state_info.message_body = error.error_message;
                        _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, "ERROCODE:" + ex.Message, JsonConvert.SerializeObject(error), DateTime.Now.AddYears(1) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                    }
                }    
            }
            return Json(response);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(user_login model)
        {
            string captchaToken = GetCookies("SESSIONID.CAPTCHA");
            if (string.IsNullOrEmpty(captchaToken))
            {
                ModelState.AddModelError("captcha", "Không tìm thấy captcha");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.captcha))
            {
                ModelState.AddModelError("captcha", "Bạn chưa nhập captcha");
            }
            string captcha = Utilities.DecryptByKey(captchaToken, "OPENID_TOKEN_KEY" + DateTime.Now.ToString("yyyyMMdd"));
            if (model.captcha != captcha)
            {
                ModelState.AddModelError("captcha", "Mã captcha chưa chính xác");
            }
            if (ModelState.IsValid)
            {
                var login = await _authenticationService.Login(new account() { partner_code = HttpConfiguration.PartnerCode, username = model.username, password = model.password });
                if (login.data_info != null)
                {
                    HttpContext.Session.SetString(OpenIDConstants.SESSSION_KEY_OPENID_CMS, JsonConvert.SerializeObject(login.data_info));
                    return RedirectToAction("Index","Action");
                }
                ModelState.AddModelError("username", "Tài khoản hoặc mật khẩu chưa chính xác");
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(OpenIDConstants.SESSSION_KEY_OPENID_CMS);
            return RedirectToAction("Login");
        }
        public IActionResult ThanhTestException()
        {
            _notificationHubContext.Clients.All.SendAsync("ReceiveNotify", "thanh test exception", "thanh test exception", "thanh test exception");
            throw new Exception("test");
        }
        public IActionResult Captcha(string prefix, bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("{0} + {1} = ?", a, b);
            var cookie_value = Utilities.EncryptByKey((a + b).ToString(), "OPENID_TOKEN_KEY" + DateTime.Now.ToString("yyyyMMdd"));
            SetCookies("SESSIONID.CAPTCHA", cookie_value, 10);
            FileContentResult img = null;
            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(System.Drawing.Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = System.Drawing.Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, x - r, y - r, r, r);
                    }
                }
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }
            return img;
        }
    }
}
