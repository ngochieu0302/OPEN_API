using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.DAL;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.BUS
{
    public static class IncludeServiceExtensionMethod
    {
        public static void AddCustomService(this IServiceCollection services)
        {
            services.AddCustomRepository();
            services.AddScoped<IDynamicService, DynamicService>();
            services.AddScoped<IOpenIdService, OpenIdService>();
            services.AddScoped<IErrorCodeService, ErrorCodeService>();
            services.AddScoped<IMailService, MailService>();
        }
    }
}
