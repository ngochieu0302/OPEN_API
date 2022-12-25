using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.COMMON;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Common;
using ESCS.COMMON.Http;
using ESCS.COMMON.MongoDb;
using ESCS.OPENID.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ESCS.OPENID
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
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.GetSection("RedisCacheMaster").Get<RedisCacheMaster>());
            services.AddSingleton(Configuration.GetSection("MongoConnection").Get<MongoConnection>());
            services.AddSingleton(Configuration.GetSection("HttpConfiguration").Get<HttpConfiguration>());
            services.AddSingleton(Configuration.GetSection("OpenIDConfig").Get<OpenIDConfig>());
            services.AddSingleton(Configuration.GetSection("HubNotifyConfiguration").Get<HubNotifyConfiguration>());
            services.AddScoped<IHttpService, HttpService>();
            services.AddSignalR();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.Cookie.Name = "escs_session_app";
                options.IdleTimeout = TimeSpan.FromHours(8);  
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddCommonService(false);
            services.AddOpenIDService();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHubContext<NotifyMessageHub> notificationHubContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseExceptionHandler("/Home/Error");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Action}/{action=Index}/{id?}");
                endpoints.MapHub<NotifyMessageHub>("/notify");
            });
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(HubNotifyConfiguration.UrlConnect)
                .Build();
            connection.On<string, string, string>("sendToUser", (partner, title, message) =>
            {
                notificationHubContext.Clients.All.SendAsync("ReceiveNotify", partner, title, message);
            });
            connection.StartAsync();
        }
    }
}
