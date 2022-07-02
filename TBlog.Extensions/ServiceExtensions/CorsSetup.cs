using Microsoft.Extensions.DependencyInjection;
using System;
using TBlog.Common;
using System.Linq;

namespace TBlog.Extensions
{
    /// <summary>
    /// Cors 启动服务
    /// </summary>
    public static class CorsSetup
    {
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (ApiConfig.Cors.Enabled)
            {
                services.AddCors(c =>
                {
                    if (ApiConfig.Cors.FilterIPs.Any())
                    {
                        c.AddPolicy(ApiConfig.Cors.PolicyName,
                            policy =>
                            {
                                policy
                                .WithOrigins(ApiConfig.Cors.FilterIPs)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                            });
                    }
                    else
                    {
                        c.AddPolicy(ApiConfig.Cors.PolicyName,
                        policy =>
                        {
                            policy
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                        });
                    }
                });
            }
        }
    }
}