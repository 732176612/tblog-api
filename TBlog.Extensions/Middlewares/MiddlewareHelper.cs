using AspNetCoreRateLimit;
using Autofac.Extensions.DependencyInjection;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TBlog.Common;
using TBlog.EventBus;

namespace TBlog.Extensions
{
    public static class MiddlewareHelper
    {
        /// <summary>
        /// 请求响应中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void UseHttpLog(this IApplicationBuilder app)
        {
            try
            {
                if (ApiConfig.Middleware.HTTPLogMatchPath!=null&& ApiConfig.Middleware.HTTPLogMatchPath.Any())
                {
                    app.UseMiddleware<HttpLogMildd>();
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MiddlewareHelper)).Error($"HttpLogMildd_Error:[{e.Message}]");
                throw;
            }
        }

        /// <summary>
        /// SignalR中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void UseSignalRSendMildd(this IApplicationBuilder app)
        {
            try
            {
                if (ApiConfig.Middleware.SignalR)
                {
                    app.UseMiddleware<SignalRSendMildd>();
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MiddlewareHelper)).Error($"SignalRSendMildd_Error:[{e.Message}]");
                throw;
            }
        }

        /// <summary>
        /// IP限流
        /// </summary>
        /// <param name="app"></param>
        public static void UseIpLimitMildd(this IApplicationBuilder app)
        {
            try
            {
                if (ApiConfig.Middleware.IpRateLimit)
                {
                    app.UseIpRateLimiting();
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MiddlewareHelper)).Error($"IpRateLimit_Error:[{e.Message}]");
                throw;
            }
        }

        /// <summary>
        /// 性能分析
        /// </summary>
        /// <param name="app"></param>
        public static void UseMiniProfilerMildd(this IApplicationBuilder app)
        {
            try
            {
                if (ApiConfig.BaseSetting.MiniProfiler)
                {
                    app.UseMiniProfiler();
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MiddlewareHelper)).Error($"MiniProfiler_Error:[{e.Message}]");
                throw;
            }
        }

        /// <summary>
        /// 接口文档中间件
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggerMildd(this IApplicationBuilder app)
        {
            try
            {
                app.UseSwagger().UseSwaggerUI(c =>
                {
                    typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                    {
                        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiConfig.BaseSetting.ApiName} {version}");
                    });
                    c.SwaggerEndpoint($"https://petstore.swagger.io/v2/swagger.json", $"{ApiConfig.BaseSetting.ApiName} pet");
                    c.RoutePrefix = "swagger";
                });
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MiddlewareHelper)).Error($"Swagger_Error:[{e.Message}]");
                throw;
            }
        }   

        /// <summary>
        /// 跨域中间件
        /// </summary>
        /// <param name="app"></param>
        public static void UseCorsMildd(this IApplicationBuilder app)
        {
            try
            {
                if (ApiConfig.Cors.Enabled)
                {
                    app.UseCors(ApiConfig.Cors.PolicyName);
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MiddlewareHelper)).Error($"Cors_Error:[{e.Message}]");
                throw;
            }
        }

        /// <summary>
        /// 事件订阅中间件
        /// </summary>
        /// <param name="app"></param>
        public static void EventBusMildd(this IApplicationBuilder app)
        {
            try
            {
                if (ApiConfig.RabbitMQ.Enabled)
                {
                    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
                    //eventBus.Subscribe<TBlogDeletedIntegrationEvent, TBlogDeletedIntegrationEventHandler>();
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MiddlewareHelper)).Error($"EventBus_Error:[{e.Message}]");
                throw;
            }
        }

        /// <summary>
        /// 显示所有服务详情信息
        /// </summary>
        /// <param name="app"></param>
        /// <param name="_services"></param>
        public static void UseAllServicesMildd(this IApplicationBuilder app, IServiceCollection _services)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            var autofacContaniers = (app.ApplicationServices.GetAutofacRoot())?.ComponentRegistry?.Registrations;


            app.Map("/allservices", builder => builder.Run(async context =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("<style>.table2_1 table {width:100%;margin:15px 0}.table2_1 th {background-color:#93DAFF;color:#000000}.table2_1,.table2_1 th,.table2_1 td{font-size:0.95em;text-align:center;padding:4px;border:1px solid #c1e9fe;border-collapse:collapse}.table2_1 tr:nth-child(odd){background-color:#dbf2fe;}.table2_1 tr:nth-child(even){background-color:#fdfdfd;}</style>");

                await context.Response.WriteAsync($"<h3>所有服务{_services.Count}个</h3><table class='table2_1'><thead><tr><th>类型</th><th>生命周期</th><th>Instance</th></tr></thead><tbody>");

                foreach (var svc in _services)
                {
                    await context.Response.WriteAsync("<tr>");
                    await context.Response.WriteAsync($"<td>{svc.ServiceType.FullName}</td>");
                    await context.Response.WriteAsync($"<td>{svc.Lifetime}</td>");
                    await context.Response.WriteAsync($"<td>{svc.ImplementationType?.Name}</td>");
                    await context.Response.WriteAsync("</tr>");
                }
                foreach (var item in autofacContaniers.ToList())
                {
                    var interfaceType = item.Services;
                    foreach (var typeArray in interfaceType)
                    {
                        await context.Response.WriteAsync("<tr>");
                        await context.Response.WriteAsync($"<td>{typeArray?.Description}</td>");
                        await context.Response.WriteAsync($"<td>{item.Lifetime}</td>");
                        await context.Response.WriteAsync($"<td>{item?.Target.Activator.ToString().Replace("(ReflectionActivator)", "")}</td>");
                        await context.Response.WriteAsync("</tr>");
                    }
                }
                await context.Response.WriteAsync("</tbody></table>");
            }));
        }
    }
}
