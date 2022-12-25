using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.CallApp;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON
{
    public static class IncludeCommonServiceExtensionMethod
    {
        public static void AddCommonService(this IServiceCollection services, bool isApi = true)
        {
            services.AddScoped<IMongoDBContext, MongoDBContext>();
            services.AddScoped(typeof(IMongoDBService<>), typeof(MongoDBService<>));
            services.AddScoped(typeof(ILogMongoRepository<>), typeof(LogMongoRepository<>));
            services.AddScoped(typeof(ILogMongoService<>), typeof(LogMongoService<>));
            services.AddSingleton<ICacheServer, CacheServer>();
            services.AddSingleton<ICacheClient, CacheClient>();
            services.AddScoped<IUserConnectionManager, UserConnectionManager>();
            services.AddSingleton<IMemoryCacheManager, MemoryCacheManager>();
            if (isApi)
                services.AddScoped<IOpenIdCallApp, OpenIdCallApp>();
        }
    }
}
