using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Logging;
using Microsoft.Extensions.Configuration;
using TBlog.Common;
namespace TBlog.Extensions.ServiceExtensions
{
    /// <summary>
    /// Apollo配置中心扩展类
    /// </summary>
    public static class ApolloSetup
    {
        /// <summary>
        /// 添加Apollo配置到配置构建器
        /// </summary>
        public static void AddApolloSetUp(this IConfigurationBuilder builder, IConfigurationSection apolloConfig)
        {
            try
            {
                if (apolloConfig.GetValue<string>("MetaServer").IsEmptyOrNull() == false)
                {
                    // 设置Apollo日志级别
                    LogManager.UseConsoleLogging(LogLevel.Info);
                    var apolloBuilder = builder.AddApollo(apolloConfig).AddDefault();
                    Console.WriteLine($"Apollo: {apolloConfig.GetValue<string>("MetaServer")}:{apolloConfig.GetValue<string>("AppId")}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Apollo配置中心添加失败: {ex.Message}");
            }
        }
    }
}