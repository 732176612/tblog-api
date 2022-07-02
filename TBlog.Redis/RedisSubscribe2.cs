using InitQ.Abstractions;
using InitQ.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;

namespace TBlog.Redis
{
    public class RedisSubscribe2 : IRedisSubscribe
    {
        [Subscribe("Loging")]
        private async Task SubRedisLoging(string msg)
        {
            Console.WriteLine($"订阅者 2 从 队列Loging消费到/接受到 消息:{msg}");

            await Task.CompletedTask;
        }
    }
}
