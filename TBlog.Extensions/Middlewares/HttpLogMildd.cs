﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TBlog.Common;
using System.Linq;
using TBlog.Model;
using TBlog.Repository;

namespace TBlog.Extensions
{
    /// <summary>
    /// 中间件
    /// 记录请求和响应数据
    /// </summary>
    public class HttpLogMildd
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLogMildd> _logger;
        private readonly IClaimUser _user;

        public HttpLogMildd(RequestDelegate next, IClaimUser user, ILogger<HttpLogMildd> logger)
        {
            _next = next;
            _logger = logger;
            _user = user;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ApiConfig.Middleware.HTTPLogMatchPath.Any(c => c.Contains(context.Request.Path.Value)))
            {
                try
                {
                    var requestData = await context.GetRequestData();
                    var startDate = DateTime.UtcNow;
                    await _next(context);
                    var endDate = DateTime.UtcNow;
                    var responeData = await context.GetResponeData();
                    var ip = context.GetIpAddress();
                    var httpLogEntity = new HttpLogEntity()
                    {
                        RequestData = requestData,
                        ResponetData = responeData,
                        IP = ip,
                        Url = context.Request.Path.ObjToString(),
                        RequestMethod = context.Request.Method,
                        Id = _user.ID,
                        UserName = _user.Name,
                        UserAgent = context.Request.Headers["User-Agent"].ObjToString(),
                        StartDate = startDate,
                        EndDate = endDate
                    };
                    await DBHelper.DB.Insertable(httpLogEntity).ExecuteCommandAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Message:[{ex.Message}]；InnerException:[{ex.InnerException}]");
                }
                return;
            }
            await _next(context);
        }
    }
}

