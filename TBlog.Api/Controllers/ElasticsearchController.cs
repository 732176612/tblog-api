//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System.Diagnostics;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Text.RegularExpressions;
//using System.Linq;
//using TBlog.Extensions;
//using System.Reflection;
//using System.ComponentModel;
//using Elastic.Clients.Elasticsearch;
//using Elastic.Transport;
//using Nest;
//namespace TBlog.Api
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class ElasticsearchController : ControllerBase
//    {
//        private readonly IActicleService _ActicleServer;
//        private readonly ILogger<ElasticsearchController> _logger;

//        public ElasticsearchController(ILogger<ElasticsearchController> logger, IActicleService ActicleServer)
//        {
//            _logger = logger;
//            _ActicleServer = ActicleServer;
//        }

//        [HttpGet]
//        public async Task<APIResult> Test()
//        {

//            return APIResult.Success();
//        }
//    }
//}
