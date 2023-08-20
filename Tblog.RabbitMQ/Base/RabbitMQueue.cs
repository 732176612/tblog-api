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

namespace Tblog.RabbitMQ
{
    public abstract class RabbitMQueue<T> : IDisposable where T : MessageModel
    {
        private readonly IRabbitMQConnection _persistentConnection;
        protected readonly ILogger<RabbitMQueue<T>> _logger;
        private readonly int _retryCount;
        private IModel _channel;
        private string _queueName;
        private BlockingCollection<T> _blockCollection;

        /// <summary>
        /// RabbitMQ事件总线
        /// </summary>
        public RabbitMQueue(IRabbitMQConnection persistentConnection, ILogger<RabbitMQueue<T>> logger, string queueName = "", int parrelTaskCount = 1, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blockCollection = new BlockingCollection<T>(parrelTaskCount);
            _retryCount = retryCount;

            if (string.IsNullOrEmpty(_queueName))
            {
                _queueName = $"{typeof(T).Name.Replace("Model", "")}";
            }
            else
            {
                _queueName = queueName;
            }

            for (int i = 0; i < parrelTaskCount; i++)
            {
                Task.Factory.StartNew(async () =>
                {
                    while (_blockCollection.IsCompleted == false)
                    {
                        var isTry = _blockCollection.TryTake(out T message, 100);
                        if (isTry && message != null)
                        {
                            bool result = false;
                            var watch = new Stopwatch();
                            watch.Start();
                            try
                            {
                                result = DequeueAction(message);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "处理消息失败");
                            }

                            try
                            {
                                while (_channel == null) { await Task.Yield(); }
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
                                logger.LogError(ex, "处理消息失败");
                            }

                            watch.Stop();
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }

        }

        /// <summary>
        /// 定义队列
        /// </summary>
        public void Start()
        {
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
            _channel.QueueDeclare(delayQueueName, true, false, false, null);
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
        /// 发布入队
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
                if (msg.DelaySecond != 0)
                {
                    properties.Expiration = (msg.DelaySecond * 1000).ToString();//延迟时间
                }
                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "", _queueName, basicProperties: properties, body: body);
                var flag = channel.WaitForConfirms();
                if (!flag)
                {
                    throw new Exception("Publish Fail");
                }
            });
        }

        public abstract bool DequeueAction(T model);

        /// <summary>
        /// 消费者消费消息
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
                model.DeliveryTag = eventArgs.DeliveryTag;
                while (_blockCollection.TryAdd(model) == false)
                {
                    await Task.Yield();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }

            while (_blockCollection.Count != 0)
            {
                Thread.Sleep(100);
            }
        Retry:
            try
            {
                _blockCollection.CompleteAdding();
            }
            catch
            {
                Thread.Sleep(100);
                goto Retry;
            }
        }
    }
}
