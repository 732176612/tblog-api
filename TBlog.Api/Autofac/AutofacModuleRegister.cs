using Autofac;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TBlog.IRepository;
using TBlog.Common;
using Autofac.Extras.DynamicProxy;
using TBlog.Extensions;
using TBlog.Repository;
using System.Linq;

namespace TBlog.Api
{
    /// <summary>
    /// 依赖注入
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
    {
        /// <summary>
        /// APIDLL路径
        /// </summary>
        public string APIDLLPath { get; set; }
        public AutofacModuleRegister(string aPIDLLPath)
        {
            APIDLLPath = aPIDLLPath;
        }

        /// <summary>
        /// 依赖注入
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            var cacheTypes = new List<Type>();
            if (ApiConfig.AOPSetting.RedisCacheAOP)
            {
                builder.RegisterType<RedisCacheAOP>();
                cacheTypes.Add(typeof(RedisCacheAOP));
            }
            if (ApiConfig.AOPSetting.MemoryCachingAOP)
            {
                builder.RegisterType<MemoryCachingAOP>();
                cacheTypes.Add(typeof(MemoryCachingAOP));
            }
            if (ApiConfig.AOPSetting.TransactionProcessAOP)
            {
                builder.RegisterType<TransactionAOP>();
                cacheTypes.Add(typeof(TransactionAOP));
            }
            if (ApiConfig.AOPSetting.ServerLogAOP)
            {
                builder.RegisterType<ServerLogAOP>();
                cacheTypes.Add(typeof(ServerLogAOP));
            }

            builder.RegisterGeneric(typeof(MongoRepository<>)).As(typeof(IMongoRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(SugarRepository<>)).As(typeof(ISugarRepository<>)).InstancePerDependency();

            var servicesDllFile = Path.Combine(AppContext.BaseDirectory, "TBlog.Service.dll");
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);
            var types = assemblysServices.GetTypes();
            builder.RegisterTypes(assemblysServices.GetTypes())
                      .AsImplementedInterfaces()
                      .InstancePerDependency()
                      .PropertiesAutowired()
                      .EnableInterfaceInterceptors()
                      .InterceptedBy(cacheTypes.ToArray());

            var repositoryDllFile = Path.Combine(AppContext.BaseDirectory, "TBlog.Repository.dll");
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterTypes(assemblysRepository.GetTypes())
                   .AsImplementedInterfaces()
                   .PropertiesAutowired()
                   .InstancePerDependency().InstancePerLifetimeScope();

            var apiDllFile = Path.Combine(AppContext.BaseDirectory, APIDLLPath);
            var queueTypes = Assembly.LoadFrom(apiDllFile).GetTypes().Where(c => c.BaseType.Name.Contains("RabbitMQueue")).ToArray();
            builder.RegisterTypes(queueTypes)
                   .PropertiesAutowired()
                   .SingleInstance();
        }
    }
}
