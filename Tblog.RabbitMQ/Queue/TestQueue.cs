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
        public TestQueue(IRabbitMQConnection persistentConnection, ILogger<RabbitMQueue<TestQueueModel>> logger, string queueName = "", int parrelTaskCount = 10, int retryCount = 5)
            : base(persistentConnection, logger, queueName, parrelTaskCount, retryCount)
        {

        }

        public override bool DequeueAction(TestQueueModel model)
        {
            _logger.LogInformation("收到了消息:" + model.Msg);
            return true;
        }
    }
}
