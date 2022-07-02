using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;
using System;
using TBlog.Common;
using TBlog.Model;

namespace TBlog.Api
{
    /// <summary>
    /// 全局异常错误日志
    /// </summary>
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionsFilter> _loggerHelper;
        private readonly IConfiguration _configuration;
        public GlobalExceptionsFilter(ILogger<GlobalExceptionsFilter> loggerHelper, IConfiguration configuration)
        {
            _loggerHelper = loggerHelper;
            this._configuration = configuration;
        }

        public void OnException(ExceptionContext context)
        {
            bool isApiError = context.Exception.Source == "TBlogApiException";
            IActionResult result = null;
            if (isApiError)
            {
                result = new ObjectResult(APIResult.Fail(context.Exception.Message));
            }
            else
            {
                string errMsg = $"\r\n【错误信息】：{context.Exception.Message} \r\n【异常类型】：{context.Exception.GetType().Name} \r\n【堆栈调用】：{context.Exception.StackTrace}";
                if (_configuration["ASPNETCORE_ENVIRONMENT"] == "Production")
                {
                    errMsg = "服务器错误";
                }
                else
                {
                    _loggerHelper.LogError(errMsg);
                }
                var badRequest = new BadRequestObjectResult(errMsg);
                badRequest.StatusCode = 500;
                result = badRequest;
            }
            context.Result = result;
        }
    }
}