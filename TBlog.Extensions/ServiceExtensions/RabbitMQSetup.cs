using TBlog.Common;
using TBlog.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace TBlog.Extensions
{
    /// <summary>
    /// Db 启动服务
    /// </summary>
    public static class RabbitMQSetup
    {
        public static void AddRabbitMQSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (ApiConfig.RabbitMQ.Enabled)
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = ApiConfig.RabbitMQ.Connection,
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(ApiConfig.RabbitMQ.UserName))
                    {
                        factory.UserName = ApiConfig.RabbitMQ.UserName;
                    }

                    if (!string.IsNullOrEmpty(ApiConfig.RabbitMQ.Password))
                    {
                        factory.Password = ApiConfig.RabbitMQ.Password;
                    }

                    return new RabbitMQPersistentConnection(factory, logger, ApiConfig.RabbitMQ.RetryCount);
                });
            }
        }
    }
}
