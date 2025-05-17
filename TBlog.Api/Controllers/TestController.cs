using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using TBlog.IRepository;
using TBlog.RabbitMQ;
using DotNetCore.CAP;
using Microsoft.Data.SqlClient;
using System.Data;

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
        private readonly ICapPublisher _capBus;
        private readonly ISugarRepository<UserEntity> _baseRepository;

        public TestController(IMenuService testServer, IRedisRepository redis, ILogger<TestController> logger,
            ISugarRepository<RoleEntity> role, TestQueue testQueue, ICapPublisher capPublisher, ISugarRepository<UserEntity> baseRepository)
        {
            this._testServer = testServer;
            _logger = logger;
            _role = role;
            _testQueue = testQueue;
            _redis = redis;
            _capBus = capPublisher;
            _baseRepository = baseRepository;
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
        public async Task<APIResult> TestQueue(string msg)
        {
            await _testQueue.Enqueue(new TestQueueModel
            {
                Msg = msg,
                RetryCount = 3
            });
            return APIResult.Success();
        }


        /// <summary>
        /// 测试延迟队列入队
        /// </summary>
        [HttpGet]
        public async Task<APIResult> TestDelayQueue(string msg)
        {
            await _testQueue.Enqueue(new TestQueueModel
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
        public async Task<APIResult> TestBatchDelayQueue(int count)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < count; i++)
            {
                tasks.Add(_testQueue.Enqueue(new TestQueueModel
                {
                    Msg = (i + 1).ToString(),
                    DelaySecond = 100
                }));
            }
            await Task.WhenAll(tasks);
            return APIResult.Success();
        }

        /// <summary>
        /// 测试CAP发送分布式事务
        /// </summary>
        [HttpGet]
        public APIResult TestCAPPulish()
        {
            DbScoped.SugarScope.Ado.Connection.Open();
            DbScoped.SugarScope.CurrentConnectionConfig.IsAutoCloseConnection = false;
            using (var connection = (SqlConnection)DbScoped.SugarScope.Ado.Connection)
            {
                using (var transaction = connection.BeginTransaction(_capBus, autoCommit: false))
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        DbScoped.SugarScope.Ado.Transaction = (IDbTransaction)transaction;
                        DbScoped.SugarScope.Insertable<UserEntity>(new UserEntity()
                        {
                            BlogName = DateTime.Now.ToString()
                        }).ExecuteCommand();
                        _capBus.Publish("xxx.services.show.time", DateTime.Now);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
            return APIResult.Success();
        }

        /// <summary>
        /// 测试CAP分布式事务
        /// </summary>
        [HttpGet]
        [CapSubscribe("xxx.services.show.time")]
        public APIResult TestCAPSubscribe()
        {
            Console.WriteLine("xxx.services.show.time" + DateTime.Now);
            DbScoped.SugarScope.Ado.Connection.Open();
            DbScoped.SugarScope.CurrentConnectionConfig.IsAutoCloseConnection = false;
            using (var connection = (SqlConnection)DbScoped.SugarScope.Ado.Connection)
            {
                using (var transaction = connection.BeginTransaction(_capBus, autoCommit: false))
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open) connection.Open();
                        DbScoped.SugarScope.Ado.Transaction = (IDbTransaction)transaction;
                        DbScoped.SugarScope.Insertable<UserEntity>(new UserEntity()
                        {
                            BlogName = DateTime.Now.ToString()
                        }).ExecuteCommand();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
            return APIResult.Success();
        }
    }
}
