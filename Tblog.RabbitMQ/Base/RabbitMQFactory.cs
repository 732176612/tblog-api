using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBlog.Common;
using TBlog.IRepository;

namespace TBlog.RabbitMQ
{
    public class RabbitMQFactory
    {
        public static void Init(IServiceCollection service)
        {
            service.AddSingleton(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQConnection>();
                var redis = sp.GetRequiredService<IRedisRepository>();
                var logger = sp.GetRequiredService<ILogger<TestQueue>>();
                var retryCount = ApiConfig.RabbitMQ.RetryCount;
                var queue = new TestQueue(connection, logger, redis, retryCount: retryCount);
                return queue;
            });
        }
    }
}