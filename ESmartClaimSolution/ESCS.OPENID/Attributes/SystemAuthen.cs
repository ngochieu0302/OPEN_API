using ESCS.COMMON.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.OPENID.Attributes
{
    public class SystemAuthen : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            string json = session.GetString(OpenIDConstants.SESSSION_KEY_OPENID_CMS);
            if (string.IsNullOrEmpty(json))
            {
                context.Result = new RedirectResult("/home/login");
                return;
            }
            base.OnActionExecuting(context);    
        }
    }
}
