using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar.IOC;
using TBlog.Common;
using TBlog.IRepository;
using TBlog.Model;
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
        private readonly IMongoRepository<HttpLogEntity> _httpLogRepository;

        public HttpLogMildd(RequestDelegate next, IClaimUser user,IMongoRepository<HttpLogEntity> httpLogRepository, ILogger<HttpLogMildd> logger)
        {
            _next = next;
            _logger = logger;
            _user = user;
            _httpLogRepository = httpLogRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ApiConfig.Middleware.HTTPLogMatchPath.Any(context.Request.Path.Value.Contains))
            {
                try
                {
                    var requestData = await context.GetRequestData();
                    var startDate = DateTime.UtcNow;
                    var endDate = DateTime.UtcNow;
                    var responeData = await context.GetResponeData(_next);
                    var ip = context.GetIpAddress();
                    var httpLogEntity = new HttpLogEntity()
                    {
                        RequestData = requestData,
                        ResponetData = responeData,
                        IP = ip,
                        Url = context.Request.Path.ObjToString(),
                        RequestMethod = context.Request.Method,
                        UserName = _user.Name,
                        UserAgent = context.Request.Headers["User-Agent"].ObjToString(),
                        StartDate = startDate,
                        EndDate = endDate,
                        //IpAddress = await HttpHelper.GetLocationNameByIp(ip)
                    };
                    await _httpLogRepository.AddEntity(httpLogEntity);
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

