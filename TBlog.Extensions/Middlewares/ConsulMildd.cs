using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using TBlog.Common;
namespace TBlog.Extensions
{
    public static class ConsulMildd
    {
        /// <summary>
        ///  微服务框架服务注册
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsulMildd(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            if (ApiConfig.Consul.Enabled)
            {
                var consulClient = new ConsulClient(c =>
                {
                    c.Address = new Uri(ApiConfig.Consul.ConsulAddress);
                });

                var registration = new AgentServiceRegistration()
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = ApiConfig.Consul.ServiceName,
                    Address = ApiConfig.Consul.ServiceIP,
                    Port = int.Parse(ApiConfig.Consul.ServicePort),
                    Check = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(ApiConfig.Consul.RegisterDealyTime),
                        Interval = TimeSpan.FromSeconds(ApiConfig.Consul.CheckHelpInterval),
                        HTTP = ApiConfig.Consul.HealthURL,
                        Timeout = TimeSpan.FromSeconds(ApiConfig.Consul.Timeout)
                    }
                };

                consulClient.Agent.ServiceRegister(registration).Wait();//服务注册

                lifetime.ApplicationStopping.Register(() =>//应用程序终止时，取消注册
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }
            return app;
        }
    }
}
