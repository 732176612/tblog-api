using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
namespace TBlog.Common
{
    public static class HttpHelper
    {
        /// <summary>
        /// 获取请求头数据
        /// </summary>
        public static async Task<string> GetRequestData(this HttpContext context)
        {
            // 1. 检查 Content-Type 是否是 multipart/form-data 为了不保存文件二进制数据
            if (context.Request.ContentType?.StartsWith("multipart/form-data") == true)
            {
                var form = await context.Request.ReadFormAsync();
                var bodyContent = string.Join("&", form.ToDictionary(x => x.Key, x => x.Value.ToString()));
                return $" QueryString:[{context.Request.QueryString}]；Body:[{bodyContent}]；";
            }

            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            return $" QueryString:[{context.Request.QueryString}]；Body:[{body}]；";
        }

        /// <summary>
        /// 获取响应流数据
        /// </summary>
        public static async Task<string> GetResponeData(this HttpContext context, RequestDelegate _next)
        {
            // 1. 保存原始的 Response.Body
            var originalBody = context.Response.Body;
            var responseBody = string.Empty;
            try
            {
                // 2. 替换为可寻址的 MemoryStream
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    // 3. 调用下一个中间件（继续处理请求）
                    await _next(context);

                    // 4. 回退到流开头，读取响应内容
                    memStream.Position = 0;
                    responseBody = await new StreamReader(memStream).ReadToEndAsync();

                    // 6. 把修改后的内容写回原始 Response.Body
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
            }
            finally
            {
                // 7. 恢复原始的 Response.Body
                context.Response.Body = originalBody;
            }
            return responseBody;
        }

        /// <summary>
        /// 获取请求Ip
        /// </summary>
        public static string GetIpAddress(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].ObjToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.MapToIPv4().ObjToString();
            }
            return ip;
        }

        /// <summary>
        /// 根据腾讯地图接口查询IP所属地区
        /// </summary>
        public static async Task<string> GetLocationNameByIp(string ip)
        {
            var respone = await $"https://apis.map.qq.com/ws/location/v1/ip?ip={ip}&key=Z3KBZ-IKUCW-KGSRV-YU6IW-JM6AH-AWBUZ".GetJsonAsync<TencentIpResult>();
            if (respone != null && respone.status == 0)
            {
                return $"{respone.result.ad_info.nation ?? ""}{respone.result.ad_info.province ?? ""}" +
                    $"{respone.result.ad_info.city ?? ""}{respone.result.ad_info.district ?? ""}";
            }
            return "";
        }
    }

    #region 腾讯IP地址接口响应结果
    /// <summary>
    /// 通过终端设备IP地址获取其当前所在地理位置，精确到市级，常用于显示当地城市天气预报、初始化用户城市等非精确定位场景。
    /// </summary>
    public class TencentIpResult
    {
        /// <summary>
        /// 状态状态码，
        /// 0为正常,
        /// 310请求参数信息有误，
        /// 311Key格式错误,
        /// 306请求有护持信息请检查字符串,
        /// 110请求来源未被授权
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 对status的描述
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// IP定位结果
        /// </summary>
        public IpResult result { get; set; }

    }

    /// <summary>
    /// IP定位结果
    /// </summary>
    public class IpResult
    {
        /// <summary>
        /// 用于定位的IP地址
        /// </summary>
        public string ip { get; set; }

        /// <summary>
        /// 定位坐标
        /// </summary>
        public Location location { get; set; }

        /// <summary>
        /// 定位行政区划信息
        /// </summary>
        public Adinfo ad_info { get; set; }
    }

    /// <summary>
    /// 定位坐标
    /// </summary>
    public class Location
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public decimal lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal lng { get; set; }

    }

    /// <summary>
    /// 定位行政区划信息
    /// </summary>
    public class Adinfo
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string nation { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public string district { get; set; }

        /// <summary>
        /// 行政区划代码
        /// </summary>
        public int adcode { get; set; }
    }
    #endregion
}

