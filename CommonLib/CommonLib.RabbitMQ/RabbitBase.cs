namespace CommonLib.RabbitMQ
{
    using global::RabbitMQ.Client;

    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            //connection?.Close();
            //connection?.Dispose();
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
                        connection = factory.CreateConnection(amqpList);
                    }
                }
            }
            return connection.CreateModel();
        }
    }
}
