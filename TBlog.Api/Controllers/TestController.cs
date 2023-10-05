using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using TBlog.IRepository;
using Tblog.RabbitMQ;

namespace TBlog.Api
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : TblogController
    {
        private readonly IMenuService _testServer;
        private static ILogger<TestController> _logger;
        private readonly ISugarRepository<RoleEntity> _role;
        private readonly IRedisRepository _redis;
        private readonly TestQueue _testQueue;

        public TestController(IMenuService testServer, IRedisRepository redis, ILogger<TestController> logger, ISugarRepository<RoleEntity> role, TestQueue testQueue)
        {
            this._testServer = testServer;
            _logger = logger;
            _role = role;
            _testQueue = testQueue;
            _redis = redis;
        }

        /// <summary>
        /// 测试
        /// </summary>
        [HttpGet]
        public APIResult Test()
        {
            var test = _role.AddEntity(new RoleEntity
            {
                Id = 30000,
                Name = "test"
            });
            return APIResult.Success();
        }

        /// <summary>
        /// 测试
        /// </summary>
        [HttpGet]
        public APIResult TestRedis(string msg)
        {
            _redis.ListLeftPush("TestQueue", msg);
            return APIResult.Success();
        }

        /// <summary>
        /// 测试队列入队
        /// </summary>
        [HttpGet]
        public APIResult TestQueue(string msg)
        {
            _testQueue.Enqueue(new TestQueueModel
            {
                Msg = msg
            });
            return APIResult.Success();
        }


        /// <summary>
        /// 测试延迟队列入队
        /// </summary>
        [HttpGet]
        public APIResult TestDelayQueue(string msg)
        {
            _testQueue.Enqueue(new TestQueueModel
            {
                Msg = msg,
                DelaySecond = 10
            });
            return APIResult.Success();
        }

        /// <summary>
        /// 测试延迟队列入队
        /// </summary>
        [HttpGet]
        public APIResult TestBatchDelayQueue(int count)
        {
            Parallel.For(0, count, (i) =>
            {
                _testQueue.Enqueue(new TestQueueModel
                {
                    Msg = (i + 1).ToString(),
                    DelaySecond = 10
                });
            });
            return APIResult.Success();
        }
    }
}
