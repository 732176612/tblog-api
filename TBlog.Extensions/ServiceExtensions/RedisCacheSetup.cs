using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using TBlog.Common;
using TBlog.Redis;

namespace TBlog.Extensions
{
    public static class RedisCacheSetup
    {
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IRedisRepository, RedisRepository>();
            services.AddSingleton(_ =>
            {
                string redisConfiguration = ApiConfig.Redis.Connection;
                var configuration = ConfigurationOptions.Parse(redisConfiguration, true);
                configuration.ResolveDns = true;
                configuration.Password = ApiConfig.Redis.PassWord;
                return ConnectionMultiplexer.Connect(configuration);
            });
        }
    }
}
