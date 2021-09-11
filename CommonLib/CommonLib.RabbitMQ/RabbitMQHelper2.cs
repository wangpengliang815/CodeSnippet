namespace CommonLib.RabbitMQ
{
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;

    using System;
    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading;

    public class RabbitMQHelper2
    {
        public static string connectionString = "";

        /// <summary>
        /// 默认重试连接次数
        /// </summary>
        public const int DefaultReTryConnectionCount = 1;

        /// <summary>
        /// 默认最大连接数
        /// </summary>
        public static int DefaultMaxConnectionCount = 500;

        /// <summary>
        /// 空闲连接队列
        /// </summary>
        private static readonly ConcurrentQueue<IConnection> FreeConnectionQueue;

        /// <summary>
        /// 活动中的连接队列
        /// </summary>
        private static readonly ConcurrentDictionary<IConnection, bool> BusyConnectionQueue;

        /// <summary>
        /// 信号量,控制同时并发可用线程数
        /// </summary>
        private static readonly Semaphore MQConnectionPoolSemaphore;
        private static readonly object freeConnLock = new(), addConnLock = new();

        static RabbitMQHelper2()
        {
            FreeConnectionQueue = new ConcurrentQueue<IConnection>();
            BusyConnectionQueue = new ConcurrentDictionary<IConnection, bool>();
            // 第一个参数：初始请求数，第二个参数：最大请求数
            MQConnectionPoolSemaphore = new Semaphore(0, DefaultMaxConnectionCount, "MQConnectionPoolSemaphore");
        }

        /// <summary>
        /// 以“;”分割MQconnectionString字符串
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">MQConnectionSetting未配置或配置不正确</exception>
        private static string[] GetMQConnectionSetting(string connectionString)
        {
            string[] mqConnectionSetting = null;

            // MQConnectionSetting=Host IP|;userid;|;password
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                if (connectionString.Contains(";"))
                {
                    mqConnectionSetting = connectionString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            if (mqConnectionSetting == null || mqConnectionSetting.Length < 3)
            {
                throw new Exception("MQConnectionSetting未配置或配置不正确");
            }
            return mqConnectionSetting;
        }

        /// <summary>
        /// 构建IConnection
        /// </summary>
        /// <returns></returns>
        private static IConnection CreateMQConnection()
        {
            ConnectionFactory factory = CrateFactory();
            // 自动重连
            factory.AutomaticRecoveryEnabled = true;
            factory.RequestedHeartbeat = new TimeSpan(5000);
            IConnection connection = factory.CreateConnection();
            return connection;
        }

        /// <summary>
        /// 构建ConnectionFactory建立连接
        /// </summary>
        /// <param name="hostName">服务器地址</param>
        /// <param name="userName">登录账号</param>
        /// <param name="passWord">登录密码</param>
        /// <returns></returns>
        private static ConnectionFactory CrateFactory()
        {
            string[] mqConnectionSetting = GetMQConnectionSetting(connectionString);
            ConnectionFactory connectionfactory = new()
            {
                HostName = mqConnectionSetting[0],
                UserName = mqConnectionSetting[1],
                Password = mqConnectionSetting[2]
            };
            // 增加端口号
            if (mqConnectionSetting.Length > 3)
            {
                connectionfactory.Port = Convert.ToInt32(mqConnectionSetting[3]);
            }
            return connectionfactory;
        }

        /// <summary>
        /// 将MQConnection重置为空闲状态
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void ResetConnectionToFree(IConnection connection)
        {
            Console.WriteLine(connection);
            try
            {
                lock (freeConnLock)
                {
                    // 从活动队列中取出
                    if (BusyConnectionQueue.TryRemove(connection, out bool result))
                    {
                    }
                    else // 若极小概率移除失败，则再重试一次
                    {
                        if (!BusyConnectionQueue.TryRemove(connection, out result)) { }
                    }

                    if (result)
                    {
                        // 如果是OPEN状态加入空闲队列
                        if (connection.IsOpen)
                        {
                            // 加入到空闲队列,以便持续提供连接服务
                            FreeConnectionQueue.Enqueue(connection);
                            if (FreeConnectionQueue.Count + BusyConnectionQueue.Count < DefaultMaxConnectionCount)
                            {
                                //释放一个空闲连接信号
                                MQConnectionPoolSemaphore.Release();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建连接,可能获取到连接池中已存在的以保持复用,尽量减少TCP/IP的连接消耗
        /// </summary>
        /// <returns></returns>
        public static IConnection CreateMQConnectionInPools()
        {
            IConnection mqConnection = null;
            try
            {
                // 如果有可用空闲连接直接返回
                if (FreeConnectionQueue.TryDequeue(out mqConnection))
                {
                    BusyConnectionQueue[mqConnection] = true;
                    return mqConnection;
                }
                else
                {
                    // 如果已有连接数小于最大可用连接数，则直接创建新连接
                    if (FreeConnectionQueue.Count + BusyConnectionQueue.Count < DefaultMaxConnectionCount)
                    {
                        lock (addConnLock)
                        {
                            if (FreeConnectionQueue.Count + BusyConnectionQueue.Count < DefaultMaxConnectionCount)
                            {
                                mqConnection = CreateMQConnection();
                                // 加入到活动连接集合
                                BusyConnectionQueue[mqConnection] = true;
                                return mqConnection;
                            }
                        }
                    }
                }
                return mqConnection;
            }
            catch
            {
                // 如果在创建连接发生错误，则判断当前是否已获得Connection，如果获得则释放连接
                if (mqConnection != null)
                {
                    mqConnection.Close();
                    mqConnection.Dispose();
                }
                throw;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="connection">消息队列连接对象</param>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="queueName">队列名称</param>
        /// <param name="durable">是否持久化</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static string SendMsg(IConnection connection, string queueName, string msg, bool durable = true)
        {
            int reTryCount = 0;
            string sendErrMsg = null;
            bool reTry;
            do
            {
                reTry = false;
                try
                {
                    // 建立通讯信道
                    using (IModel channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queueName, durable, false, false, null);

                        IBasicProperties properties = channel.CreateBasicProperties();

                        if (!durable)
                            properties = null;
                        else
                            // 1:不持久|2.持久化
                            properties.DeliveryMode = 2;

                        byte[] body = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish("", queueName, properties, body);
                    }
                    sendErrMsg = string.Empty;
                }
                catch (Exception ex)
                {
                    // 可重试1次
                    if ((++reTryCount) <= DefaultReTryConnectionCount)
                    {
                        ResetConnectionToFree(connection);
                        connection = CreateMQConnection();
                        reTry = true;
                    }
                    sendErrMsg = ex.ToString();
                }
                finally
                {
                    if (!reTry)
                    {
                        ResetConnectionToFree(connection);
                    }
                }
            } while (reTry);
            return sendErrMsg;
        }

        /// <summary>
        /// 消费消息(持续订阅消费消息,这个其实可以不用连接池也不会有问题，因为它是一个持久订阅并持久消费的过程，不会出现频繁创建连接对象的情况)
        /// </summary>
        /// <param name="connection">消息队列连接对象</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="durable">是否持久化</param>
        /// <param name="messageHandler">消息处理函数</param>
        /// <param name="logHandler">保存日志方法,可选</param>
        public static void ConsumeMsg(IConnection connection, string queueName, bool durable
            , Func<string, ConsumeAction> messageHandler
            , Action<string, Exception> logHandler = null)
        {
            try
            {
                using IModel channel = connection.CreateModel();
                // 建立消费者
                EventingBasicConsumer consumer = new(channel);

                // 获取队列
                //channel.QueueDeclare(queueName, durable, false, false, null);
                channel.BasicQos(0, 1, false);

                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                while (true)
                {
                    ConsumeAction consumeResult = ConsumeAction.RETRY;

                    consumer.Received += (model, ea) =>
                    {
                        string message = null;
                        try
                        {
                            message = Encoding.UTF8.GetString(ea.Body.ToArray());
                            consumeResult = messageHandler(message);
                        }
                        catch (Exception ex)
                        {
                            logHandler?.Invoke(message, ex);
                        }
                        finally
                        {
                            if (consumeResult == ConsumeAction.ACCEPT)
                            {
                                // 消息从队列中删除
                                channel.BasicAck(ea.DeliveryTag, false);
                            }
                            else if (consumeResult == ConsumeAction.RETRY)
                            {
                                // 消息重回队列
                                channel.BasicNack(ea.DeliveryTag, false, true);
                            }
                            else
                            {
                                // 消息直接丢弃
                                channel.BasicNack(ea.DeliveryTag, false, false);
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                logHandler?.Invoke("QueueName:" + queueName, ex);
            }
            //finally
            //{
            //    ResetConnectionToFree(connection);
            //}
        }

        /// <summary>
        /// 依次获取单个消息
        /// </summary>
        /// <param name="connection">消息队列连接对象</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="durable">持久化</param>
        /// <param name="messageHandler">处理消息委托</param>
        public static void ConsumeMsgSingle(IConnection connection, string queueName, bool durable
            , Func<string, ConsumeAction> messageHandler)
        {
            bool reTry = false;
            int reTryCount = 0;
            ConsumeAction consumeResult = ConsumeAction.RETRY;
            IModel channel = null;
            BasicDeliverEventArgs ea = null;
            do
            {
                reTry = false;
                try
                {
                    channel = connection.CreateModel();

                    channel.QueueDeclare(queueName, durable, false, false, null);
                    channel.BasicQos(0, 1, false);

                    uint msgCount = channel.MessageCount(queueName);

                    if (msgCount > 0)
                    {
                        // 建立消费者
                        EventingBasicConsumer consumer = new(channel);
                        channel.BasicConsume(queueName, false, consumer);

                        consumer.Received += (model, ea) =>
                        {
                            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                            consumeResult = messageHandler(message);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        };
                    }
                    else
                    {
                        messageHandler("queue is empty.");
                    }
                }
                catch (Exception ex)
                {
                    if ((++reTryCount) <= DefaultReTryConnectionCount)
                    {
                        if (channel != null) channel.Dispose();

                        ResetConnectionToFree(connection);
                        connection = CreateMQConnection();
                        reTry = true;
                    }
                }
                finally
                {
                    if (!reTry)
                    {
                        if (channel != null && ea != null)
                        {
                            if (consumeResult == ConsumeAction.ACCEPT)
                            {
                                channel.BasicAck(ea.DeliveryTag, false);
                            }
                            else if (consumeResult == ConsumeAction.RETRY)
                            {
                                channel.BasicNack(ea.DeliveryTag, false, true);
                            }
                            else
                            {
                                channel.BasicNack(ea.DeliveryTag, false, false);
                            }
                        }

                        if (channel != null) channel.Dispose();

                        ResetConnectionToFree(connection);
                    }
                }

            } while (reTry);
        }

        /// <summary>
        /// 获取队列消息数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static int GetMessageCount(IConnection connection, string queueName)
        {
            int msgCount = 0;
            int reTryCount = 0;

            bool reTry;
            do
            {
                reTry = false;
                try
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queueName, true, false, false, null);
                        msgCount = (int)channel.MessageCount(queueName);
                    }
                }
                catch (Exception ex)
                {
                    if ((++reTryCount) <= DefaultReTryConnectionCount)//可重试1次
                    {
                        ResetConnectionToFree(connection);
                        connection = CreateMQConnection();
                        reTry = true;
                    }
                    throw ex;
                }
                finally
                {
                    if (!reTry)
                    {
                        ResetConnectionToFree(connection);
                    }
                }

            } while (reTry);

            return msgCount;
        }
    }

    public enum ConsumeAction
    {
        /// <summary>
        /// 消费成功
        /// </summary>
        ACCEPT,
        /// <summary>
        /// 消费失败,可放回队列重新消费
        /// </summary>
        RETRY,
        // 消费失败，直接丢弃
        REJECT,
    }
}

