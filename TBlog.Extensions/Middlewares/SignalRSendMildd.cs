using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;

namespace TBlog.Extensions
{
    /// <summary>
    /// 中间件
    /// SignalR发送数据
    /// </summary>
    public class SignalRSendMildd
    {
        private readonly RequestDelegate _next;
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRSendMildd(RequestDelegate next, IHubContext<ChatHub> hubContext)
        {
            _next = next;
            _hubContext = hubContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData());
            await _next(context);
        }

    }
}
