namespace TBlog.Model
{
    /// <summary>
    /// 队列消息模型
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// 消息标识
        /// </summary>
        public ulong DeliveryTag { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string GUID { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 延迟时间
        /// </summary>
        public int DelaySecond { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }
    }
}
