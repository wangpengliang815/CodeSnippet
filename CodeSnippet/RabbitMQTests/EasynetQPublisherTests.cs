#define RabbitMQRuning
namespace CodeSnippet.RabbitMQTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    using CommonLib.RabbitMQ;

    using global::RabbitMQ.Client;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("EasyNetQ")]
    [TestClass()]
    public class EasynetQPublisherTests : TestBase
    {
        private static TestContext _testContext;
        private const string redisConnection = @"host=192.168.31.191;virtualHost=myVirtualHost;username=guest;password=guest";
        private static readonly IServiceCollection services = new ServiceCollection();
        private static RabbitMQHelper mq;

        private static readonly Stopwatch sw = new();

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
            services.AddSingleton(p => new RabbitMQHelper(redisConnection));
            IServiceProvider provider = services.BuildServiceProvider();
            mq = provider.GetService<RabbitMQHelper>();
        }

        [TestInitialize]
        public void TestCaseInit()
        {
            Console.WriteLine($"TestName: {_testContext.TestName}");
            sw.Restart();
            Console.WriteLine($"{nameof(mq)} HashCode: {mq.GetHashCode()}");
        }

        /// <summary>
        /// Tests the cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            sw.Stop();
            mq.Dispose();
            Console.WriteLine($"time：{sw.ElapsedMilliseconds} Milliseconds");
        }

        /// <summary>
        /// 生产端
        /// </summary>
        [TestMethod]
        public void PublisherTest_Basic()
        {
            //mq.PublishAsync("easynet").ConfigureAwait(false);

            // 创建名称为hello的队列
            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string message = "hello world";
            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            try
            {
                // 消息发送
                channel.BasicPublish(
                    exchange: "",
                    routingKey: "hello",
                    basicProperties: null,
                    body: messageBody);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
#if debug
        /// <summary>
        /// 生产端消息确认(tx事务机制)
        /// </summary>
        [TestMethod]
        public void PublisherTest_Transaction()
        {
            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string message = "hello world";
            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            try
            {
                // 开启tx事务机制
                channel.TxSelect();

                // 消息发送
                channel.BasicPublish(
                    exchange: "",
                    routingKey: "hello",
                    basicProperties: null,
                    body: messageBody);

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
        /// 生产端消息确认(Confirm模式)
        /// </summary>
        [TestMethod]
        public void PublisherTest_Confirm()
        {
            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string message = "hello world";
            byte[] messageBody = Encoding.UTF8.GetBytes(message);

            // 开启Confirm模式
            channel.ConfirmSelect();

            // 消息发送
            channel.BasicPublish(
                    exchange: "",
                    routingKey: "hello",
                    basicProperties: null,
                    body: messageBody);

            // WaitForConfirms确认消息(可以同时确认多条消息)是否发送成功
            if (channel.WaitForConfirms())
            {
                Console.WriteLine($"Message发送成功");
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// 消息持久化
        /// </summary>
        [TestMethod]
        public void PublisherTest_Persistent()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    byte[] messageBody = Encoding.UTF8.GetBytes(i.ToString());

                    // 设置消息持久化
                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;

                    // 消息发送
                    channel.BasicPublish(
                        exchange: "TestExchange",
                        routingKey: "",
                        basicProperties: props,
                        body: messageBody);
                }
                Console.WriteLine($"发送到Broke成功");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 消息优先级
        /// </summary>
        [TestMethod]
        public void PublisherTest_Priority()
        {
            // 声明交换机exchang
            channel.ExchangeDeclare(exchange: "myexchange",
                                    type: ExchangeType.Fanout,
                                    durable: true,
                                    autoDelete: false,
                                    arguments: null);
            // 声明队列queue
            channel.QueueDeclare(queue: "myqueue",
                               durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: new Dictionary<string, object>() {
                               //队列优先级最高为10，不加x-max-priority的话，消息发布时设置了消息的优先级也不会生效
                               {"x-max-priority",10 }
                               });
            // 绑定exchange和queue
            channel.QueueBind(queue: "myqueue", exchange: "myexchange", routingKey: "mykey");
            Console.WriteLine("生产者准备就绪....");
            // 测试数据
            string[] msgs = { "vip1", "hello1", "hello2", "hello3", "vip5" };

            // 设置消息优先级
            var props = channel.CreateBasicProperties();
            foreach (string msg in msgs)
            {
                // vip开头的消息，优先级设置为9
                if (msg.StartsWith("vip"))
                {
                    props.Priority = 9;
                    channel.BasicPublish(exchange: "myexchange",
                                         routingKey: "mykey",
                                         basicProperties: props,
                                         body: Encoding.UTF8.GetBytes(msg));
                }
                // 其他消息优先级为1
                else
                {
                    props.Priority = 1;
                    channel.BasicPublish(exchange: "myexchange",
                                         routingKey: "mykey",
                                         basicProperties: props,
                                         body: Encoding.UTF8.GetBytes(msg));
                }
            }
            Assert.IsTrue(true);
        }
#endif
    }
}

