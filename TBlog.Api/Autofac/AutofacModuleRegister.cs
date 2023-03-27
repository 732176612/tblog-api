﻿using Autofac;
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

namespace TBlog.Api
{
    /// <summary>
    /// 依赖注入
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
    {
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            var cacheType = new List<Type>();
            if (ApiConfig.AOPSetting.RedisCacheAOP)
            {
                builder.RegisterType<RedisCacheAOP>();
                cacheType.Add(typeof(RedisCacheAOP));
            }
            if (ApiConfig.AOPSetting.MemoryCachingAOP)
            {
                builder.RegisterType<MemoryCachingAOP>();
                cacheType.Add(typeof(MemoryCachingAOP));
            }
            if (ApiConfig.AOPSetting.TransactionProcessAOP)
            {
                builder.RegisterType<TransactionAOP>();
                cacheType.Add(typeof(TransactionAOP));
            }
            if (ApiConfig.AOPSetting.ServerLogAOP)
            {
                builder.RegisterType<ServerLogAOP>();
                cacheType.Add(typeof(ServerLogAOP));
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
                      .InterceptedBy(cacheType.ToArray());

            var repositoryDllFile = Path.Combine(AppContext.BaseDirectory, "TBlog.Repository.dll");
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterTypes(assemblysRepository.GetTypes())
                   .AsImplementedInterfaces()
                   .PropertiesAutowired()
                   .InstancePerDependency().InstancePerLifetimeScope();
        }
    }
}
