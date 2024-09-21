using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.IRepository;

namespace TBlog.Extensions
{
    /// <summary>
    /// 自定义权限授权处理器
    /// </summary>
    public class TBlogAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>
    {
        public IAuthenticationSchemeProvider Schemes { get; set; }
        private readonly IHttpContextAccessor _accessor;
        private readonly IMenuRepository _menuServices;
        private readonly IRoleRepository _roleServices;

        public TBlogAuthorizationHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor accessor, IMenuRepository menuServices, IRoleRepository roleServices)
        {
            _accessor = accessor;
            Schemes = schemes;
            _roleServices = roleServices;
            _menuServices = menuServices;
        }

        // 重写异步处理程序
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
        {
            var httpContext = _accessor.HttpContext;

            if (httpContext != null)
            {
                var rquestUrl = httpContext.Request.Path.Value.ToLower();

                httpContext.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
                {
                    OriginalPath = httpContext.Request.Path,
                    OriginalPathBase = httpContext.Request.PathBase
                });

                // Give any IAuthenticationRequestHandler schemes a chance to handle the request
                // 主要作用是: 判断当前是否需要进行远程验证，如果是就进行远程验证
                var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
                {
                    if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                    {
                        context.Fail();
                        return;
                    }
                }

                //判断请求是否拥有凭据，即有没有登录
                var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);

                    // 是否开启测试环境
                    var isTestCurrent = ApiConfig.BaseSetting.EnabledTest;

                    //result?.Principal不为空即登录成功
                    if (result?.Principal != null || isTestCurrent)
                    {
                        if (!isTestCurrent) httpContext.User = result.Principal;

                        var currentUserRoles = new List<string>();

                        if (ApiConfig.IdentityServer4.Enabled)
                        {
                            currentUserRoles = (from item in httpContext.User.Claims
                                                where item.Type == "role"
                                                select item.Value).ToList();
                        }
                        else
                        {
                            currentUserRoles = (from item in httpContext.User.Claims
                                                where item.Type == requirement.ClaimType
                                                select item.Value).ToList();
                        }

                        var isMatchRole = false;
                        var roleEntities = await _roleServices.GetByNames(currentUserRoles);
                        var menuEntities = _menuServices.GetByRoleIds(roleEntities.Select(c => c.Id));
                        foreach (var item in menuEntities)
                        {
                            try
                            {
                                if (Regex.Match(rquestUrl, item.Url?.ObjToString().ToLower())?.Value == rquestUrl)
                                {
                                    isMatchRole = true;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }

                        //验证权限
                        if (currentUserRoles.Count <= 0 || !isMatchRole)
                        {
                            context.Fail();
                            return;
                        }

                        var isExp = false;

                        if (ApiConfig.IdentityServer4.Enabled)
                        {
                            isExp = (httpContext.User.Claims.SingleOrDefault(s => s.Type == "exp")?.Value) != null && DateTimeHelper.StampToDateTime(httpContext.User.Claims.SingleOrDefault(s => s.Type == "exp")?.Value) >= DateTime.Now;
                        }
                        else
                        {
                            isExp = (httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null && DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now;
                        }

                        if (isExp)
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                        }
                    }
                }
            }
        }
    }
}
