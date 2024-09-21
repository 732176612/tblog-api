using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using TBlog.Common;
using TBlog.IService;
using TBlog.Model;
using TBlog.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using TBlog.Api;
using Microsoft.DotNet.PlatformAbstractions;
using TBlog.IRepository;
using TBlog.Extensions;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TBlog.Repository;

namespace TBlog.Test
{
    public class Init_Test
    {
        public Init_Test()
        {
            var basePath = ApplicationEnvironment.ApplicationBasePath;
            IServiceCollection services = new ServiceCollection().AddLogging();
            services.AddSingleton(new ApiConfig(new ConfigurationBuilder()
               .SetBasePath(basePath)
               .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = false, ReloadOnChange = true })
               .Build(), basePath));
            //services.AddAutoMapper(typeof(Startup));
            services.AddMongoDBSetup();

            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(MongoRepository<>)).As(typeof(IMongoRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(SugarRepository<>)).As(typeof(ISugarRepository<>)).InstancePerDependency();

            var servicesDllFile = Path.Combine(basePath, "TBlog.Service.dll");
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);
            var types = assemblysServices.GetTypes();
            builder.RegisterAssemblyTypes(assemblysServices)
                      .AsImplementedInterfaces()
                      .InstancePerDependency()
                      .PropertiesAutowired()
                      .EnableInterfaceInterceptors();

            var repositoryDllFile = Path.Combine(basePath, "TBlog.Repository.dll");
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository)
                   .AsImplementedInterfaces()
                   .PropertiesAutowired()
                   .InstancePerDependency().InstancePerLifetimeScope();
            builder.Populate(services);
            var ApplicationContainer = builder.Build();

            ContainerHelper.RegisterContainer(ApplicationContainer);

            Assert.True(ApplicationContainer.ComponentRegistry.Registrations.Count() > 0);
        }
    }
}
