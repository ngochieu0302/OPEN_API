using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ESCS.BUS;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON;
using ESCS.COMMON.Caches;
using ESCS.COMMON.CallApp;
using ESCS.COMMON.Common;
using ESCS.COMMON.Http;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.SignaturePDF;
using ESCS.DAL.Repository;
using ESCS.API.Hubs;
using ESCS.API.Middlewares;
using ESCS.MODEL.OpenID;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using RazorEngine;
using RazorEngine.Templating;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Net.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using ESCS.COMMON.SMS.MCM;
using System.Reflection;

namespace ESCS.API
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            this.Configuration = builder.Build();
            RSACryptographyKeyGenerator rsa = new RSACryptographyKeyGenerator();
            RSAKeysTypes a = rsa.GenerateKeys(RSAKeySize.Key4096);
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.GetSection("MongoConnection").Get<MongoConnection>());
            services.AddSingleton(Configuration.GetSection("RedisCacheMaster").Get<RedisCacheMaster>());
            services.AddSingleton(Configuration.GetSection("RedisCacheReplication").Get<RedisCacheReplication>());
            services.AddSingleton(Configuration.GetSection("OpenIDConfig").Get<OpenIDConfig>());
            services.AddSingleton(Configuration.GetSection("DataProtectionConfig").Get<DataProtectionConfig>());
            services.AddSingleton(Configuration.GetSection("CallAppConfiguration").Get<CallAppConfiguration>());
            services.AddSingleton(Configuration.GetSection("ApiGatewayConfig").Get<ApiGatewayConfig>());
            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddSingleton(Configuration.GetSection("CoreApiConfig").Get<CoreApiConfig>());
            services.AddSingleton(Configuration.GetSection("MCMConfiguration").Get<MCMConfiguration>());
            services.AddSingleton(Configuration.GetSection("OmiCallConfiguration").Get<OmiCallConfiguration>());
            services.AddSingleton(Configuration.GetSection("SignatureFileConfig").Get<SignatureFileConfig>());
            services.AddSingleton(Configuration.GetSection("ESCSServiceConfig").Get<ESCSServiceConfig>());
            services.AddSingleton(Configuration.GetSection("OCRApiConfig").Get<OCRApiConfig>());
            #region Test Log
            /*Options*/
            services.AddOptions<RequestResponseLoggerOption>().Bind
            (Configuration.GetSection("RequestResponseLogger")).ValidateDataAnnotations();
            /*IOC*/
            services.AddSingleton<IRequestResponseLogger, RequestResponseLogger>();
            services.AddScoped<IRequestResponseLogModelCreator, RequestResponseLogModelCreator>();
            #endregion
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddMvc().AddRazorRuntimeCompilation();
            services.AddControllers().AddNewtonsoftJson(opt =>
                 opt.SerializerSettings.ContractResolver = new LowercaseContractResolver());
            services.AddDataProtection()
             .PersistKeysToFileSystem(new DirectoryInfo(@"bin\debug\configuration"))
             .ProtectKeysWithDpapi()
             .SetDefaultKeyLifetime(TimeSpan.FromDays(DataProtectionConfig.KeyLifetimeDay));

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue; 
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = long.MaxValue;
            });
            services.AddCommonService();
            services.AddCustomService();
            OpenIDConfig.LowercaseContractResolver = new LowercaseContractResolver();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMailService mailService, ILoggerFactory loggerFactory)
        {
            if (AppSettings.UseDeveloperExceptionPage)
            {
                app.UseDeveloperExceptionPage();
            }
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
            /*Middleware*/
            app.UseMiddleware<RequestResponseLoggerMiddleware>();
            //app.UseLog();
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotifyMessageHub>("/notify");
                endpoints.MapHub<PartnerNotifyHub>("/notify-service");
            });
            try
            {
                var googleCredential = Path.Combine(env.ContentRootPath, "App_Data/ESCS/esmart-claim-solution-firebase-adminsdk.json");
                var credential = GoogleCredential.FromFile(googleCredential);
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
