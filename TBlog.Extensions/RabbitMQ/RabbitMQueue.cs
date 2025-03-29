using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;
using System.Net.Sockets;
using TBlog.Common;
using TBlog.IRepository;
using TBlog.Model;
namespace TBlog.Extensions
{
    public abstract class RabbitMQueue<T> : IDisposable where T : MessageModel, new()
    {
        private readonly IRabbitMQConnection _persistentConnection;
        protected readonly ILogger<RabbitMQueue<T>> _logger;
        protected readonly int _retryCount;
        private IChannel? _channel;
        private string _queueName;
        private BlockingCollection<T> _blockCollection;
        private bool IsDisposeing;
        private IRedisRepository _redis;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parrelTaskCount">并行任务数</param>
        /// <param name="retryCount">重试次数</param>
        public RabbitMQueue(IRedisRepository redis, IRabbitMQConnection persistentConnection, ILogger<RabbitMQueue<T>> logger, string queueName = "", int parrelTaskCount = 1, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blockCollection = new BlockingCollection<T>(parrelTaskCount);
            _retryCount = retryCount;
            _queueName = string.IsNullOrEmpty(queueName) ? $"{typeof(T).Name.Replace("Model", "")}" : queueName;
            _redis = redis;
            InitTask();
        }

        /// <summary>
        /// 初始化多线程
        /// </summary>
        public void InitTask()
        {
            for (int i = 0; i < _blockCollection.BoundedCapacity; i++)
            {
                Task.Factory.StartNew(async () =>
                {
                    while (_blockCollection.IsCompleted == false)
                    {
                        var isTry = _blockCollection.TryTake(out T? message, 10);
                        if (isTry && message != null)
                        {
                            bool result = false;
                            try
                            {

                                result = DequeueAction(message);//确认消费成功
                                while (_channel == null) { await Task.Delay(10); }
                                if (result)
                                {
                                    await _channel.BasicAckAsync(message.DeliveryTag, multiple: false);//标记消息已消费
                                }
                                else
                                {
                                    var retryKey = (await _redis._db.HashGetAsync(_queueName, message.GUID));
                                    var retryCount = retryKey.HasValue ? retryKey.ToInt(0) : message.RetryCount;
                                    await _redis._db.HashSetAsync(_queueName, message.GUID, --retryCount);
                                    if (retryCount <= 0)
                                    {
                                        await _channel.BasicAckAsync(message.DeliveryTag, multiple: false);//标记消息已消费
                                        await _redis._db.HashDeleteAsync(_queueName, message.GUID);
                                    }
                                    else
                                    {
                                        await _channel.BasicNackAsync(message.DeliveryTag, false, true);//标记消息未消费
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"{_queueName} HandleMessage Exception");
                            }
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// 初始化队列信息
        /// </summary>
        public async Task Start()
        {
            if (IsDisposeing) return;
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnect();
            }

            _channel = await _persistentConnection.CreateIChannel();

            var delayQueueName = _queueName + "Delay";
            var delayExchageName = _queueName + "Exchange";

            //设置普通队列
            Dictionary<string, object> args = new Dictionary<string, object>() {
                { "x-dead-letter-exchange", delayExchageName },
                { "x-dead-letter-routing-key", delayQueueName }
            };
            await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);

            //设置死信队列
            await _channel.ExchangeDeclareAsync(delayExchageName, "direct", true, false, null);
            await _channel.QueueDeclareAsync(delayQueueName, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(delayQueueName, delayExchageName, delayQueueName, null);
            var delayConsumer = new AsyncEventingBasicConsumer(_channel);
            delayConsumer.ReceivedAsync += Consumer_Received;
            await _channel.BasicConsumeAsync(
                queue: delayQueueName,
                autoAck: false,
                consumer: delayConsumer);

            _channel.CallbackExceptionAsync += async (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception.Message, "Recreating RabbitMQ consumer channel");
                _channel.Dispose();
                await Start();
            };
        }

        /// <summary>
        /// 入队
        /// </summary>
        public async Task Enqueue(T msg)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not Enqueue:{Timeout}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                });

            using var channel = await _persistentConnection.CreateIChannel();
            var message = JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(message);
            await policy.Execute(async () =>
            {
                var properties = new BasicProperties();
                properties.ContentType = "text/plain";
                properties.DeliveryMode = DeliveryModes.Persistent;
                properties.Expiration = msg.DelaySecond == 0 ? "0" : (msg.DelaySecond * 1000).ToString();//延迟时间
                await channel.BasicPublishAsync(exchange: "", _queueName, mandatory: true, basicProperties: properties, body: body);
            });
        }

        /// <summary>
        /// 出队
        /// </summary>
        public abstract bool DequeueAction(T model);

        /// <summary>
        /// 接收信息
        /// </summary>
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                var model = JsonConvert.DeserializeObject<T>(message);
                if (model == null) model = new T();
                model.DeliveryTag = eventArgs.DeliveryTag;
                while (_blockCollection.TryAdd(model) == false)
                {
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public async void Dispose()
        {
            IsDisposeing = true;
            if (_channel != null) _channel.Dispose();
            while (_blockCollection.Count != 0) await Task.Yield();
        }
    }
}
