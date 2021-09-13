namespace CommonLib.RabbitMQ
{
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class QueueOptions
    {
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }

    public class ConsumeQueueOptions : QueueOptions
    {
        /// <summary>
        /// 是否自动提交
        /// </summary>
        public bool AutoAck { get; set; } = false;

        /// <summary>
        /// 每次接收消息条数
        /// </summary>
        public ushort? FetchCount { get; set; } = 1;
    }

    public class ExchangeConsumeQueueOptions : ConsumeQueueOptions
    {
        /// <summary>
        /// 路由值
        /// </summary>
        public string[] RoutingKeys { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> BindArguments { get; set; } = new Dictionary<string, object>();
    }

    public class ExchangeQueueOptions : QueueOptions
    {
        /// <summary>
        /// 交换机类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 队列及路由值
        /// </summary>
        public List<Tuple<string, string>> QueueAndRoutingKey { get; set; } = new List<Tuple<string, string>>();

        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> BindArguments { get; set; } = new Dictionary<string, object>();
    }

    public static class RabbitMQExchangeType
    {
        /// <summary>
        /// 普通模式
        /// </summary>
        public const string Common = "";

        /// <summary>
        /// 路由模式
        /// </summary>
        public const string Direct = "direct";

        /// <summary>
        /// 发布/订阅模式
        /// </summary>
        public const string Fanout = "fanout";

        /// <summary>
        /// 匹配订阅模式
        /// </summary>
        public const string Topic = "topic";
    }

    public abstract class RabbitBase : IDisposable
    {
        private readonly List<AmqpTcpEndpoint> amqpList;
        private IConnection connection;

        protected RabbitBase(params string[] hosts)
        {
            if (hosts == null || hosts.Length == 0)
            {
                throw new ArgumentException("invalid hosts！", nameof(hosts));
            }

            amqpList = new List<AmqpTcpEndpoint>();
            amqpList.AddRange(hosts.Select(host => new AmqpTcpEndpoint(host, Port)));
        }

        protected RabbitBase(params (string, int)[] hostAndPorts)
        {
            if (hostAndPorts == null || hostAndPorts.Length == 0)
            {
                throw new ArgumentException("invalid hosts！", nameof(hostAndPorts));
            }

            amqpList = new List<AmqpTcpEndpoint>();
            amqpList.AddRange(hostAndPorts.Select(tuple => new AmqpTcpEndpoint(tuple.Item1, tuple.Item2)));
        }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; } = ConnectionFactory.DefaultUser;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = ConnectionFactory.DefaultPass;

        /// <summary>
        /// 虚拟机
        /// </summary>
        public string VirtualHost { get; set; } = ConnectionFactory.DefaultVHost;

        public virtual void Dispose()
        {
            // connection?.Close();
            // connection?.Dispose();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            connection?.Close();
            connection?.Dispose();
        }

        /// <summary>
        /// 获取rabbitmq的连接
        /// </summary>
        /// <returns></returns>
        protected IModel GetChannel()
        {
            if (connection == null)
            {
                lock (this)
                {
                    if (connection == null)
                    {
                        ConnectionFactory factory = new()
                        {
                            Port = Port,
                            UserName = UserName,
                            VirtualHost = VirtualHost,
                            Password = Password
                        };
                        // 网络故障自动恢复连接
                        factory.AutomaticRecoveryEnabled = true;
                        // 心跳处理
                        factory.RequestedHeartbeat = new TimeSpan(5000);
                        connection = factory.CreateConnection(amqpList);
                    }
                }
            }
            return connection.CreateModel();
        }
    }

    public class RabbitMQHelper : RabbitBase
    {
        public RabbitMQHelper(params string[] hosts) : base(hosts)
        {

        }

        public RabbitMQHelper(params (string, int)[] hostAndPorts) : base(hostAndPorts)
        {

        }

        /// <summary>
        /// 简单队列消息发布
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        public void Publish(string queue, string message, QueueOptions options = null)
        {
            options ??= new QueueOptions();
            IModel channel = GetChannel();
            channel.QueueDeclare(queue, options.Durable, false, options.AutoDelete, options.Arguments ?? new Dictionary<string, object>());
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: null, body: buffer);
            channel.Close();
        }

        /// <summary>
        /// 订阅模式/路由模式/Topic模式
        /// </summary>
        /// <param name="exchange">The exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="message">The message.</param>
        /// <param name="options">The options.</param>
        public void Publish(string exchange, string routingKey, string message, ExchangeQueueOptions options = null)
        {
            options ??= new ExchangeQueueOptions();
            IModel channel = GetChannel();

            channel.ExchangeDeclare(exchange,
                string.IsNullOrEmpty(options.Type) ? RabbitMQExchangeType.Fanout : options.Type,
                options.Durable,
                options.AutoDelete,
                options.Arguments ?? new Dictionary<string, object>());

            if (options.QueueAndRoutingKey.Count > 0)
            {
                foreach (var item in options.QueueAndRoutingKey)
                {
                    if (!string.IsNullOrEmpty(item.Item1))
                    {
                        channel.QueueDeclare(item.Item1, options.Durable, false, options.AutoDelete, options.Arguments);

                        channel.QueueBind(item.Item1,
                            exchange,
                            item.Item2 ?? "",
                            options.BindArguments ?? new Dictionary<string, object>());
                    }
                }
            }
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange, routingKey, null, buffer);
            channel.Close();
        }

        public event Action<RecieveResult> Received;

        /// <summary>
        /// 构造消费者
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private IBasicConsumer ConsumeInternal(IModel channel, ConsumeQueueOptions options)
        {
            EventingBasicConsumer consumer = new(channel);
            consumer.Received += (sender, e) =>
            {
                try
                {
                    CancellationTokenSource cancellationTokenSource = new();
                    if (!options.AutoAck)
                    {
                        cancellationTokenSource.Token.Register(() =>
                        {
                            channel.BasicAck(e.DeliveryTag, false);
                        });
                    }
                    Received?.Invoke(new RecieveResult(e, cancellationTokenSource));
                }
                catch { }
            };
            if (options.FetchCount != null)
            {
                channel.BasicQos(0, options.FetchCount.Value, false);
            }
            return consumer;
        }

        /// <summary>
        /// 消息监听
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public ListenResult Listen(string queue, ConsumeQueueOptions options = null)
        {
            options ??= new ConsumeQueueOptions();
            IModel channel = GetChannel();
            channel.QueueDeclare(queue, options.Durable, false, options.AutoDelete, options.Arguments ?? new Dictionary<string, object>());
            IBasicConsumer consumer = ConsumeInternal(channel, options);
            channel.BasicConsume(queue, options.AutoAck, consumer);
            ListenResult result = new();
            result.Token.Register(() =>
            {
                try
                {
                    channel.Close();
                    channel.Dispose();
                }
                catch { }
            });
            return result;
        }

        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        public ListenResult Listen(string exchange, string queue, ExchangeConsumeQueueOptions options = null)
        {
            options ??= new ExchangeConsumeQueueOptions();
            IModel channel = GetChannel();
            channel.QueueDeclare(queue, options.Durable, false, options.AutoDelete, options.Arguments ?? new Dictionary<string, object>());
            if (options.RoutingKeys != null && !string.IsNullOrEmpty(exchange))
            {
                foreach (string key in options.RoutingKeys)
                {
                    channel.QueueBind(queue, exchange, key, options.BindArguments);
                }
            }
            IBasicConsumer consumer = ConsumeInternal(channel, options);
            channel.BasicConsume(queue, options.AutoAck, consumer);
            ListenResult result = new();
            result.Token.Register(() =>
            {
                try
                {
                    channel.Close();
                    channel.Dispose();
                }
                catch { }
            });
            return result;
        }

        public class RecieveResult
        {
            private CancellationTokenSource cancellationTokenSource;

            public RecieveResult(BasicDeliverEventArgs arg, CancellationTokenSource cancellationTokenSource)
            {
                Body = Encoding.UTF8.GetString(arg.Body.ToArray());
                ConsumerTag = arg.ConsumerTag;
                DeliveryTag = arg.DeliveryTag;
                Exchange = arg.Exchange;
                Redelivered = arg.Redelivered;
                RoutingKey = arg.RoutingKey;
                this.cancellationTokenSource = cancellationTokenSource;
            }

            /// <summary>
            /// 消息体
            /// </summary>
            public string Body { get; private set; }

            /// <summary>
            /// 消费者标签
            /// </summary>
            public string ConsumerTag { get; private set; }

            /// <summary>
            /// Ack标签
            /// </summary>
            public ulong DeliveryTag { get; private set; }

            /// <summary>
            /// 交换机
            /// </summary>
            public string Exchange { get; private set; }

            /// <summary>
            /// 是否Ack
            /// </summary>
            public bool Redelivered { get; private set; }

            /// <summary>
            /// 路由
            /// </summary>
            public string RoutingKey { get; private set; }

            public void Commit()
            {
                if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested) return;

                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }
        public class ListenResult
        {
            private readonly CancellationTokenSource cancellationTokenSource;

            /// <summary>
            /// CancellationToken
            /// </summary>
            public CancellationToken Token { get { return cancellationTokenSource.Token; } }

            /// <summary>
            /// 是否已停止
            /// </summary>
            public bool Stoped { get { return cancellationTokenSource.IsCancellationRequested; } }

            public ListenResult()
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            /// <summary>
            /// 停止监听
            /// </summary>
            public void Stop()
            {
                cancellationTokenSource.Cancel();
            }
        }
    }
}
