global using TBlog.Extensions;
global using TBlog.Service;
global using TBlog.Model;
global using TBlog.Repository;
global using System;
global using TBlog.Common;
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
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using TBlog.RabbitMQ;
using AspNetCoreRateLimit;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBlog.Api;
using System.Threading.Tasks;
using TBlog.Extensions.ServiceExtensions;
using Com.Ctrip.Framework.Apollo;
var builder = WebApplication.CreateBuilder(args);
builder.Host
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacModuleRegister("TBlog.Api.dll"));
    builder.RegisterModule<AutofacPropertityModuleReg>();
    builder.RegisterBuildCallback(c =>
    {
        ContainerHelper.RegisterContainer(c as IContainer);
    });
})
.ConfigureAppConfiguration((hostContext, config) =>
{
    config.AddApolloSetUp(hostContext.Configuration.GetSection("Apollo"));
})
.ConfigureLogging((hostingContext, builder) =>
{
    builder.AddFilter("System", LogLevel.Error);
    builder.AddFilter("Microsoft", LogLevel.Error);
    builder.SetMinimumLevel(LogLevel.Error);
    builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
});
builder.Services.AddSingleton(new ApiConfig(builder.Configuration, AppDomain.CurrentDomain.BaseDirectory));
builder.Services.AddSingleton(new LogLock(AppDomain.CurrentDomain.BaseDirectory));
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddMemoryCacheSetup();
builder.Services.AddRedisCacheSetup();
builder.Services.AddElasticsearchSetup();
builder.Services.AddMongoDBSetup();
builder.Services.AddCorsSetup();
builder.Services.AddMiniProfilerSetup();
builder.Services.AddSwaggerSetup();
builder.Services.AddHttpContextSetup();
builder.Services.AddSqlSugarSetup();
builder.Services.AddRabbitMQSetup();

// 授权+认证 (jwt or ids4)
builder.Services.AddAuthorizationSetup();
builder.Services.AddAuthentication_Ids4Setup();
builder.Services.AddAuthentication_JWTSetup();

builder.Services.AddIpPolicyRateLimitSetup(builder.Configuration);

builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();

builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
        .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

builder.Services.AddControllers(o =>
{
    //如果是正式环境
    if (builder.Environment.IsProduction())
    {
        o.Filters.Add<GlobalExceptionsFilter>();
    }
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
builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
//支持编码大全 例如:支持 System.Text.Encoding.GetEncoding("GB2312")  System.Text.Encoding.GetEncoding("GB18030") 
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var app = builder.Build();
app.UseIpLimitMildd();

app.UseHttpLog();

app.UseSignalRSendMildd();

app.UseAllServicesMildd(builder.Services);

if (builder.Environment.IsDevelopment())
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
            var html = await File.ReadAllTextAsync(Path.Combine(builder.Environment.WebRootPath, "./view/index.html"));
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
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "./view")),
    RequestPath = "/view"
});

app.UseConsulMildd(app.Lifetime);

app.UseRabbitMQQueue("TBlog.Api.dll");

await SqlSugarDBSeed.SeedAsync();

ChangeToken.OnChange(() => ((IConfiguration)builder.Configuration).GetReloadToken(), () =>
{
    app.Services.GetRequiredService<ApiConfig>().Reload();
});

app.Run();

