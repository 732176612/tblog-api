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
                Task.Factory.StartNew(() =>
                {
                    while (_blockCollection.IsCompleted == false)
                    {
                        var isTry = _blockCollection.TryTake(out T message, 100);
                        if (isTry && message != null)
                        {
                            var watch = new Stopwatch();
                            watch.Start();
                            try
                            {
                                DequeueAction(message);
                            }
                            catch (Exception ex)
                            {

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

            var channel = _persistentConnection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += Consumer_Received;

            channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");
                _channel.Dispose();
                Start();
            };

            _channel = channel;
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
                properties.DeliveryMode = 2; // persistent
                channel.BasicPublish(exchange: "", _queueName, basicProperties: properties, body: body);
            });
        }

        public abstract void DequeueAction(T model);

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

                while (_blockCollection.TryAdd(model) == false)
                {
                    await Task.Yield();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
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
