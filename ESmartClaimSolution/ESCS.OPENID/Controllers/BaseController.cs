using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESCS.COMMON.Contants;
using ESCS.COMMON.MongoDb;
using ESCS.MODEL.OpenID.ModelView;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ESCS.OPENID.Controllers
{
    public class BaseController : Controller
    {
        private readonly IMongoDBContext _context;
        public BaseController()
        {
            _context = new MongoDBContext();
        }
        public sys_partner_cache GetUser()
        {
            string session = HttpContext.Session.GetString(OpenIDConstants.SESSSION_KEY_OPENID_CMS);
            sys_partner_cache partner = JsonConvert.DeserializeObject<sys_partner_cache>(session);
            return partner;
        }
        public virtual IMongoDBService<B> GetMongoService<B>() where B :class
        {
            try
            {
                return (MongoDBService<B>)typeof(MongoDBService<B>).GetConstructor(new Type[] { typeof(MongoDBContext) }).Invoke(new object[] { this._context });
            }
            catch
            {
                return default(MongoDBService<B>);
            }
        }
        public void SetCookies(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);
            Response.Cookies.Append(key, value, option);
        }
        public string GetCookies(string key)
        {
            return Request.Cookies[key];
        }
        public void RemoveCookies(string key)
        {
            Response.Cookies.Delete(key);
        }
    }
}