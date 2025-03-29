using Microsoft.Extensions.Logging;
using TBlog.IRepository;
namespace TBlog.RabbitMQ
{
    public class TestQueue : RabbitMQueue<TestQueueModel>
    {
        private readonly IRedisRepository _redis;
        public TestQueue(IRabbitMQConnection persistentConnection, ILogger<RabbitMQueue<TestQueueModel>> logger, IRedisRepository redis)
        : base(redis, persistentConnection, logger, "", 10, 5)
        {
            _redis = redis;
        }

        public override bool DequeueAction(TestQueueModel model)
        {
            if (model.Msg == "1")
            {
                _logger.LogDebug("收到了消息:" + model.Msg);
                return false;
            }
            else
            {
                _logger.LogDebug("收到了消息:" + model.Msg);
                return true;
            }
        }
    }
}
