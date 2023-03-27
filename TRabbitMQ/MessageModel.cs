using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRabbitMQ
{
    public class MessageModel
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; } =string.Empty;

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 重试次数
        /// </summary>
        public int TryCount { get; set; }
    }
}
