using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tblog.RabbitMQ
{
    public class TestQueue : RabbitMQueue<TestQueueModel>
    {
        public TestQueue(IRabbitMQConnection persistentConnection, ILogger<RabbitMQueue<TestQueueModel>> logger, string queueName = "", int parrelTaskCount = 1, int retryCount = 5) : base(persistentConnection, logger, queueName, parrelTaskCount, retryCount)
        {

        }

        public override void DequeueAction(TestQueueModel model)
        {
            _logger.LogDebug("收到了消息:"+model.Msg);
        }
    }
}
