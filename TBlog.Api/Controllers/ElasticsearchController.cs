using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using TBlog.Extensions;
using System.Reflection;
using System.ComponentModel;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Nest;
using TencentCloud.Cme.V20191029.Models;

namespace TBlog.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ElasticsearchController : ControllerBase
    {
        private readonly IElasticClient _ElasticClient;
        private readonly IActicleService _ActicleServer;
        private readonly ILogger<ElasticsearchController> _logger;

        public ElasticsearchController(ILogger<ElasticsearchController> logger, IActicleService ActicleServer, IElasticClient elasticClient)
        {
            _logger = logger;
            _ActicleServer = ActicleServer;
            _ElasticClient = elasticClient;
        }

        /// <summary>
        /// 初始化 Elasticsearch 索引
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<APIResult> Test()
        {
            var allActicle= await _ActicleServer.GetAll();
            foreach (var acticle in allActicle)
            {
               var test= await _ElasticClient.IndexDocumentAsync(acticle);
            }
            return APIResult.Success();
        }
    }
}
