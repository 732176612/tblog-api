﻿using Microsoft.Extensions.DependencyInjection;
using Nest;
using TBlog.Common;
namespace TBlog.Extensions
{
    public static class ElasticsearchSetup
    {
        public static void AddElasticsearchSetup(this IServiceCollection services)
        {
            services.AddScoped<IElasticClient>(o =>
            {
                var url = new Uri(ApiConfig.Elasticsearch.Url);
                var settings = new ConnectionSettings(url)
                .BasicAuthentication(ApiConfig.Elasticsearch.UserName, ApiConfig.Elasticsearch.Password)
                .DefaultIndex(ApiConfig.Elasticsearch.DefaultIndex);
                var client = new ElasticClient(settings);
                return client;
            });
        }
    }
}
