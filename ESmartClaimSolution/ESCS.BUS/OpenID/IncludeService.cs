using ESCS.DAL.OpenID;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.BUS.OpenID
{
    public static class IncludeService
    {
        public static void AddOpenIDService(this IServiceCollection services)
        {
            services.AddOpenIDRepository();
            services.AddScoped<IOpenIDCommonService, OpenIDCommonService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IActionService, ActionService>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddScoped<IErrorCodeService, ErrorCodeService>();
            services.AddScoped<IMailService, MailService>();
        }
    }
}
