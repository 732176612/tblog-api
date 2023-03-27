namespace Tblog.RabbitMQ
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
    }
}
