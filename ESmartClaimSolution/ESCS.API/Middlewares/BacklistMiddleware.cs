using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ESCS.API.Middlewares
{
    public class BacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _safelist;

        public BacklistMiddleware(
            RequestDelegate next,
            string safelist)
        {
            _safelist = safelist;
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != HttpMethod.Get.Method)
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                string[] ip = _safelist.Split(';');
                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;
                foreach (var address in ip)
                {
                    var testIp = IPAddress.Parse(address);
                    if (testIp.GetAddressBytes().SequenceEqual(bytes))
                    {
                        badIp = false;
                        break;
                    }
                }
                if (badIp)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
