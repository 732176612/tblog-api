using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;
using MongoDB;
using StackExchange.Profiling;
using MongoDB.Driver;
using TBlog.IRepository;
using TBlog.Repository;
using Microsoft.Extensions.Logging;

namespace TBlog.Extensions
{
    public static class MongoDBSetup
    {
        /// <summary>
        /// MongoDB配置
        /// </summary>
        /// <param name="services"></param>
        public static void AddMongoDBSetup(this IServiceCollection services)
        {
            services.AddScoped<IMongoClient>(o =>
            {
                return new MongoClient(ApiConfig.DBSetting.MongoConnection);
            });
        }
    }
}
