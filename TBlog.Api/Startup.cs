global using TBlog.Common;
global using TBlog.Extensions;
global using TBlog.IService;
global using TBlog.Model;
global using TBlog.Repository;
global using TBlog.Tasks;
global using System;
global using TBlog.Redis;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdGen;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TBlog.Api
{
    public class Startup
    {
        private IServiceCollection _services;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new ApiConfig(Configuration, AppDomain.CurrentDomain.BaseDirectory));
            services.AddSingleton(new LogLock(AppDomain.CurrentDomain.BaseDirectory));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddMemoryCacheSetup();
            services.AddRedisCacheSetup();

            services.AddMongoDBSetup();
            services.AddSqlsugarSetup();
            services.AddCorsSetup();
            services.AddMiniProfilerSetup();
            services.AddSwaggerSetup();
            services.AddJobSetup();
            services.AddHttpContextSetup();
            services.AddRedisInitMqSetup();

            services.AddRabbitMQSetup();
            services.AddEventBusSetup();

            // 授权+认证 (jwt or ids4)
            services.AddAuthorizationSetup();
            services.AddAuthentication_Ids4Setup();
            services.AddAuthentication_JWTSetup();

            services.AddIpPolicyRateLimitSetup(Configuration);

            services.AddSignalR().AddNewtonsoftJsonProtocol();

            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            services.AddControllers(o =>
            {
                //o.Filters.Add<GlobalExceptionsFilter>();
                o.InputFormatters.Add(new PlainTextInputFormatter());
                o.InputFormatters.Add(new CustInputFormatter());
            })
            .AddNewtonsoftJson(options =>
             {
                 //忽略循环引用
                 options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                 //不使用驼峰样式的key
                 options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                 //设置本地时间而非UTC时间
                 options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
             });
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            _services = services;
            //支持编码大全 例如:支持 System.Text.Encoding.GetEncoding("GB2312")  System.Text.Encoding.GetEncoding("GB18030") 
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
            builder.RegisterModule<AutofacPropertityModuleReg>();
            builder.RegisterBuildCallback(c =>
            {
                ContainerHelper.RegisterContainer(c as IContainer);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISqlSugarClient sugarClient, IHostApplicationLifetime lifetime)
        {
            app.UseIpLimitMildd();

            app.UseHttpLog();

            app.UseSignalRSendMildd();

            app.UseAllServicesMildd(_services);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSwaggerMildd();

            app.UseCorsMildd();

            app.UseCookiePolicy();

            app.UseStatusCodePages();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiniProfilerMildd();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/api2/chatHub");

                endpoints.MapFallback(async context =>
                {
                    if (context.Request.Path.StartsWithSegments("/view", StringComparison.Ordinal))
                    {
                        var html = await File.ReadAllTextAsync(Path.Combine(env.WebRootPath, "./view/index.html"));
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(html, Encoding.UTF8);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                });
            });

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "./view")),
                RequestPath = "/view"
            });

            SqlSugarDBSeed.SeedAsync(sugarClient);

            app.UseConsulMildd(lifetime);

            app.EventBusMildd();
        }
    }
}
