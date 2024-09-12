using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.RabbitMQ
{
    public class TestQueueModel : MessageModel
    {
        public string Msg { get; set; }
    }
}
