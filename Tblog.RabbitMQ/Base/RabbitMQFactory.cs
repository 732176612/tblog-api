using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBlog.Common;

namespace Tblog.RabbitMQ
{
    public class RabbitMQFactory
    {
        public static void Init(IServiceCollection service)
        {
            service.AddSingleton(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQConnection>();
                var logger = sp.GetRequiredService<ILogger<TestQueue>>();
                var retryCount = ApiConfig.RabbitMQ.RetryCount;
                var queue = new TestQueue(connection, logger, retryCount: retryCount);
                return queue;
            });
        }
    }
}