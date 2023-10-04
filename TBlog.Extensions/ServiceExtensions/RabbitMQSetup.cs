using TBlog.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using Tblog.RabbitMQ;
using Autofac;

namespace TBlog.Extensions
{
    /// <summary>
    /// RabbitMQ启动服务
    /// </summary>
    public static class RabbitMQSetup
    {
        public static void AddRabbitMQSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (ApiConfig.RabbitMQ.Enabled)
            {
                services.AddSingleton<IRabbitMQConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = ApiConfig.RabbitMQ.Connection,
                        UserName = ApiConfig.RabbitMQ.UserName,
                        Password = ApiConfig.RabbitMQ.Password,
                        DispatchConsumersAsync = true
                    };

                    return new RabbitMQConnection(factory, logger, ApiConfig.RabbitMQ.RetryCount);
                });

                RabbitMQFactory.Init(services);
            }
        }
    }
}
