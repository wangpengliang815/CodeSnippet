namespace CodeSnippet.RabbitMQTests
{
    using global::RabbitMQ.Client;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RabbitMQ.Client.Events;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// 使用RabbitMQClient
    /// </summary>
    /// <seealso cref="CodeSnippet.RabbitMQ.TestBase" />
    [TestCategory("RebbitMQ-Client")]
    [TestClass()]
    public class MQPubSubTests
    {
        private static readonly string queueName = $"test.pubsub.ut.queue";
        private static ConnectionFactory factory;
        private static IConnection connection;
        private static IModel channel;

        private static TestContext _testContext;
        private static readonly Stopwatch sw = new();

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;

            factory = new ConnectionFactory()
            {
                HostName = "192.168.248.191",
                // 用户名
                UserName = "guest",
                // 密码
                Password = "guest",
                // 网络故障自动恢复连接
                AutomaticRecoveryEnabled = true,
                // 心跳处理
                RequestedHeartbeat = new TimeSpan(5000)
            };


        }

        [TestInitialize]
        public void TestCaseInit()
        {
            Console.WriteLine($"TestName: {_testContext.TestName}");
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            sw.Restart();
        }

        /// <summary>
        /// Tests the cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            sw.Stop();
            channel.Dispose();
            connection.Dispose();
            Console.WriteLine($"time：{sw.ElapsedMilliseconds} Milliseconds");
        }

        private static void PubMessage(string queueName, string message)
        {
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: messageBody);
        }

        /// <summary>
        /// Pub:简单队列
        /// </summary>
        [TestMethod]
        public void Pub_Simple()
        {
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = "hello world";
            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            try
            {
                // 消息发送,mandatory=如果发布了一个设置了“强制”标志的消息，但未能送达，则代理将消息返回给发送的客户端（通过basic.return AMQP 0-9-1命令）。 要获得此类通知，客户端可以订阅IModel.BasicReturn事件。 如果没有附加事件的监听器，则返回的消息将被静默地丢弃。
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: messageBody, mandatory: true);

                channel.BasicReturn += (sender, message) =>
                {
                    Console.WriteLine("message send fail：" + Encoding.UTF8.GetString(message.Body.ToArray()));
                };
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        ///Pub:消息确认(tx事务机制)
        /// </summary>
        [TestMethod]
        public void Pub_Transaction()
        {
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = "hello world";
            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            try
            {
                // 开启tx事务机制
                channel.TxSelect();

                // 消息发送
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: messageBody);

                // 事务提交
                channel.TxCommit();
            }
            catch (Exception ex)
            {
                // 事务回滚
                channel.TxRollback();
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Pub:消息确认(Confirm模式)
        /// </summary>
        [TestMethod]
        public void Pub_Confirm()
        {
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = "hello world";
            byte[] messageBody = Encoding.UTF8.GetBytes(message);

            // 开启Confirm模式
            channel.ConfirmSelect();

            // 消息发送
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: messageBody);

            // WaitForConfirms确认消息(可以同时确认多条消息)是否发送成功
            if (channel.WaitForConfirms())
            {
                Console.WriteLine($"message发送成功");
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Pub:消息优先级
        /// </summary>
        [TestMethod]
        public void Pub_Priority()
        {
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false,
                               arguments: new Dictionary<string, object>() {
                               // 队列优先级最高为10，不加x-max-priority的话，消息发布时设置了消息的优先级也不会生效
                               {"x-max-priority",10 }
                               });

            // 测试数据
            string[] msgs = { "vip1", "hello1", "hello2", "hello3", "vip5" };

            // 设置消息优先级
            IBasicProperties props = channel.CreateBasicProperties();
            foreach (string msg in msgs)
            {
                // vip开头的消息，优先级设置为9,其他消息优先级为1
                if (msg.StartsWith("vip"))
                    props.Priority = 9;
                else
                    props.Priority = 1;

                channel.BasicPublish(exchange: "",
                                       routingKey: queueName,
                                       basicProperties: props,
                                       body: Encoding.UTF8.GetBytes(msg));
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Sub:消息优先级
        /// </summary>
        [TestMethod]
        public void Sub_Priority()
        {
            EventingBasicConsumer consumer = new(channel);

            // 绑定消息接收后的事件委托
            consumer.Received += (model, ea) =>
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                channel.BasicAck(
                   deliveryTag: ea.DeliveryTag,
                   multiple: false);
                Console.WriteLine($"Message：{message}");
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Thread.Sleep(3000);
        }

        /// <summary>
        /// Sub:自动确认,自动确认有一个弊端:如果消费端收到了消息,但在后续处理代码中抛出了异常,这条消息将会丢失,因为消费端收到消息因为是自动ack RabbitMQ就会将该消息删除
        /// </summary>
        /// <remarks>
        /// autoAck设置为true
        /// channel会自动在处理完上一条消息之后,接收下一条消息。（同一个channel消息处理是串行的）
        /// 除非关闭channel或者取消订阅,否则客户端将会一直接收队列的消息
        /// </remarks>
        [TestMethod]
        public void Sub_AutoAck()
        {
            EventingBasicConsumer consumer = new(channel);

            // 绑定消息接收后的事件委托
            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message：{message}");
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        /// <summary>
        /// Sub:手动确认
        /// </summary>
        /// <remarks>
        /// autoAck设置为false
        /// 通过BasicAck手动确认消费
        /// <remarks>
        [TestMethod]
        public void Sub_BasicAck()
        {
            for (int i = 0; i < 10; i++)
            {
                PubMessage(queueName, $"test_{i}");
            }

            EventingBasicConsumer consumer = new(channel);

            channel.BasicQos(0, 1, false);

            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message：{message}");
                Assert.IsNotNull(message);

                // 这里可以添加逻辑:处理成功才ack
                channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Thread.Sleep(5000);
        }

        /// <summary>
        /// Sub:消息拒绝(BasicReject:一次只能拒绝一条消息)
        /// </summary>
        /// <remarks>
        /// 把消息塞回的队列(头部不是尾部）
        /// 该测试将会死循环
        /// <remarks>
        [TestMethod]
        public void Sub_BasicReject()
        {
            // for (int i = 0; i < 3; i++)
            // {
            //     PubMessage(queueName, i.ToString());
            // }

            EventingBasicConsumer consumer = new(channel);

            channel.BasicQos(0, 1, false);

            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());

                if (message == "2")
                {
                    Console.WriteLine($"Message：{message}");
                    channel.BasicAck(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false);
                }
                else
                {
                    Console.WriteLine($"拒绝处理");
                    /* BasicReject用于拒绝消息
                       requeue参数指定了拒绝后是否重新放回queue
                       一次只能拒绝一条消息
                       设置为true: 消息会被重新仍回queue中
                       设置为false:消息将被丢弃
                    */
                    channel.BasicReject(
                        deliveryTag: ea.DeliveryTag,
                        requeue: true);
                }
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Thread.Sleep(3000);
        }

        /// <summary>
        /// Sub:消息拒绝(BasicNack:可以一次拒绝N条消息)
        /// </summary>
        /// 把消息塞回的队列(头部不是尾部）
        /// 该测试将会死循环
        /// <remarks>
        [TestMethod]
        public void Sub_BasicNack()
        {

            for (int i = 0; i < 100; i++)
            {

                PubMessage(queueName, i.ToString());
            }

            static void MessageConsumer(BasicDeliverEventArgs ea)
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message:{message}");
                if (message == "50")
                {
                    throw new Exception("error");
                }
            }
            EventingBasicConsumer consumer = new(channel);

            channel.BasicQos(0, 1, false);

            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    /* 消费到某条消息时出错
                     * 导致Broker无法拿到正常回执信息引发后续消息都无法被正常消费
                     * 如果MQ没得到ack响应，这些消息会堆积在Unacked消息里,不会丢弃,直至客户端断开重连时，才变回ready
                     * 如果Consumer客户端不断开连接，这些Unacked消息，永远不会变回ready状态
                     * Unacked消息多了,占用内存越来越大,就会异常
                     */
                    MessageConsumer(ea);
                    channel.BasicAck(
                               deliveryTag: ea.DeliveryTag,
                               multiple: false);
                }
                catch (Exception ex)
                {
                    // 出错了，发nack，并通知MQ把消息塞回的队列头部（不是尾部）
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    Console.WriteLine(ex.Message);
                }
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            Thread.Sleep(50000);
        }

        /// <summary>
        /// BasicGet()消费端主动获取消息
        /// </summary>
        [TestMethod]
        public void BasicGet()
        {
            BasicGetResult result = channel.BasicGet(queue: queueName, autoAck: true);
            Assert.IsNotNull(result.Body.ToArray());
            Console.WriteLine($"接收到消息:{Encoding.UTF8.GetString(result.Body.ToArray())}");
        }

        /// <summary>
        /// Pub:死信队列.DLXs the pub test.
        /// </summary>
        [TestMethod]
        public void Dlx_PubTest()
        {
            string dlxExchangeName = "test.dlx.exchange";
            string dlxQueueName = "test.dlx.queue";
            channel.ExchangeDeclare(dlxExchangeName, type: "topic");
            channel.QueueDeclare(queue: dlxQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            // # 表示只要有消息到达了死信的exchange,都会路由到这个死信队列
            channel.QueueBind(queue: dlxQueueName, exchange: dlxExchangeName, routingKey: "#");

            // 声明队列时添加死信参数
            Dictionary<string, object> agruments = new()
            {
                { "x-dead-letter-exchange", dlxExchangeName }
            };
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: agruments);

            for (int i = 0; i < 10; i++)
            {
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: Encoding.UTF8.GetBytes(i.ToString()));
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Dlx_SubTest()
        {
            EventingBasicConsumer consumer = new(channel);

            channel.BasicQos(0, 1, false);

            consumer.Received += (model, ea) =>
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (message != "8")
                {
                    Console.WriteLine("message:" + message);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Thread.Sleep(3000);
        }
    }
}

