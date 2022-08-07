using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TBlog.Common;

namespace TBlog.Extensions
{
    /// <summary>
    /// 系统 授权服务 配置
    /// </summary>
    public static class AuthorizationSetup
    {
        public static void AddAuthorizationSetup(this IServiceCollection services)
        {
            if (ApiConfig.IdentityServer4.Enabled || ApiConfig.JwtBearer.Enabled)
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("User", policy => policy.RequireRole("User").Build());
                    options.AddPolicy("System", policy => policy.RequireRole("System").Build());
                    options.AddPolicy("System_User", policy => policy.RequireRole("User", "System"));
                });

                //读取配置文件
                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ApiConfig.JwtBearer.Secret));
                var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                // 角色与接口的权限要求参数
                var permissionRequirement = new AuthorizationRequirement(
                    ClaimTypes.Role,//基于角色的授权
                    ApiConfig.JwtBearer.Issuer,//发行人
                    ApiConfig.JwtBearer.Audience,//听众
                    signingCredentials,//签名凭据
                    TimeSpan.FromSeconds(60 * 60)//接口的过期时间
                    );

                // 3、自定义复杂的策略授权
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(ConstHelper.SystemRole, policy => policy.Requirements.Add(permissionRequirement));
                });
                // 注入权限处理器
                services.AddScoped<IAuthorizationHandler, TBlogAuthorizationHandler>();
                services.AddSingleton(permissionRequirement);
            }
        }
    }
}
