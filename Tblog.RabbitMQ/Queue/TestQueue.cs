using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.IRepository;

namespace Tblog.RabbitMQ
{
    public class TestQueue : RabbitMQueue<TestQueueModel>
    {
        private readonly IRedisRepository _redis;
        public TestQueue(IRabbitMQConnection persistentConnection, ILogger<RabbitMQueue<TestQueueModel>> logger,
            IRedisRepository redis,
            string queueName = "", int parrelTaskCount = 10, int retryCount = 5)
            : base(persistentConnection, logger, queueName, parrelTaskCount, retryCount)
        {
            _redis = redis;
        }

        public override bool DequeueAction(TestQueueModel model)
        {
            _logger.LogInformation("收到了消息:" + model.Msg);
            return _redis.ListLeftPush("TestQueue", model.Msg).Result > 0;
        }
    }
}
