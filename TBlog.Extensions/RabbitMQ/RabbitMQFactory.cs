using Microsoft.AspNetCore.Builder;
using System.IO;
using System.Reflection;
using TBlog.Common;
namespace TBlog.Extensions
{
    public static class RabbitMQFactory
    {
        /// <summary>
        /// RabbitMQ队列初始化
        /// </summary>
        public static void UseRabbitMQQueue(this IApplicationBuilder app,string dllPath)
        {
            if (ApiConfig.RabbitMQ.Enabled)
            {
                var apiDllFile = Path.Combine(AppContext.BaseDirectory, dllPath);
                var queueTypes = Assembly.LoadFrom(apiDllFile).GetTypes().Where(c => c.BaseType.Name.Contains("RabbitMQueue")).ToArray();
                foreach (var item in queueTypes)
                {
                    var instance = app.ApplicationServices.GetService(item);
                    MethodInfo method = item.GetMethod("Start");
                    if (method != null)
                    {
                        method.Invoke(instance, null);
                    }
                }
            }
        }
    }
}