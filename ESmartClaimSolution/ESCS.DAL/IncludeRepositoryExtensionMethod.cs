using ESCS.DAL.OpenID;
using ESCS.DAL.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.DAL
{
    public static class IncludeRepositoryExtensionMethod
    {
        public static void AddCustomRepository(this IServiceCollection services)
        {
            
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IOpenIdRepository, OpenIdRepository>();
            services.AddScoped<IErrorCodeRepository, ErrorCodeRepository>();
            services.AddScoped<IMailRepository, MailRepository>();
        }
    }
}
