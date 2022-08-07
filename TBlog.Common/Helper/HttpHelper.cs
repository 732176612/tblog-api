using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Common
{
    public static class HttpHelper
    {
        /// <summary>
        /// 获取请求头数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<string> GetRequestData(this HttpContext context)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            return $" QueryString:[{ context.Request.QueryString}]；Body:[{ body}]；";
        }

        /// <summary>
        /// 获取响应流数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<string> GetResponeData(this HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responeBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return responeBody;
        }

        /// <summary>
        /// 获取请求Ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIP(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].ObjToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ObjToString();
            }
            return ip;
        }
    }
}
