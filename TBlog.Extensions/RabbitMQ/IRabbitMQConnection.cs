using RabbitMQ.Client;
namespace TBlog.Extensions
{
    /// <summary>
    /// RabbitMQ持久连接接口
    /// </summary>
    public interface IRabbitMQConnection : IDisposable
    {
        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 尝试连接
        /// </summary>
        Task<bool> TryConnect();

        /// <summary>
        /// 创建渠道
        /// </summary>
        Task<IChannel> CreateIChannel();
    }
}
