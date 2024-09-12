using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Polly;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace TBlog.RabbitMQ
{
    public abstract class RabbitMQueue<T> : IDisposable where T : MessageModel, new()
    {
        private readonly IRabbitMQConnection _persistentConnection;
        protected readonly ILogger<RabbitMQueue<T>> _logger;
        private readonly int _retryCount;
        private IModel? _channel;
        private string _queueName;
        private BlockingCollection<T> _blockCollection;
        private bool IsDisposeing;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parrelTaskCount">并行任务数</param>
        /// <param name="retryCount">重试次数</param>
        public RabbitMQueue(IRabbitMQConnection persistentConnection, ILogger<RabbitMQueue<T>> logger, string queueName = "", int parrelTaskCount = 1, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blockCollection = new BlockingCollection<T>(parrelTaskCount);
            _retryCount = retryCount;
            _queueName = string.IsNullOrEmpty(queueName) ? $"{typeof(T).Name.Replace("Model", "")}" : queueName;
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
                                result = DequeueAction(message);
                                while (_channel == null) { await Task.Delay(10); }
                                if (result)
                                {
                                    _channel.BasicAck(message.DeliveryTag, multiple: false);
                                }
                                else
                                {
                                    _channel.BasicNack(message.DeliveryTag, multiple: false, requeue: true);
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
        public void Start()
        {
            if (IsDisposeing) return;
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _channel = _persistentConnection.CreateModel();

            var delayQueueName = _queueName + "Delay";
            var delayExchageName = _queueName + "Exchange";

            //设置普通队列
            Dictionary<string, object> args = new Dictionary<string, object>() { { "x-dead-letter-exchange", delayExchageName }, { "x-dead-letter-routing-key", delayQueueName } };
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);

            //设置死信队列
            _channel.ExchangeDeclare(delayExchageName, "direct", true, false, null);
            _channel.QueueDeclare(delayQueueName, durable: true, exclusive: false, autoDelete: false, null);
            _channel.QueueBind(delayQueueName, delayExchageName, delayQueueName, null);
            var delayConsumer = new AsyncEventingBasicConsumer(_channel);
            delayConsumer.Received += Consumer_Received;
            _channel.BasicConsume(
                queue: delayQueueName,
                autoAck: false,
                consumer: delayConsumer);

            _channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception.Message, "Recreating RabbitMQ consumer channel");
                _channel.Dispose();
                Start();
            };
        }

        /// <summary>
        /// 入队
        /// </summary>
        public void Enqueue(T msg)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not Enqueue:{Timeout}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                });

            using var channel = _persistentConnection.CreateModel();
            var message = JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(message);
            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // 持久化
                properties.Expiration = msg.DelaySecond == 0 ? "0" : (msg.DelaySecond * 1000).ToString();//延迟时间
                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "", _queueName, basicProperties: properties, body: body);
                var flag = channel.WaitForConfirms();
                if (!flag)
                {
                    throw new Exception("Publish Fail");
                }
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
