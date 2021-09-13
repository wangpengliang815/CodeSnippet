﻿#define RabbitMQRuning
namespace CodeSnippet.RabbitMQTests
{
    using global::RabbitMQ.Client;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RabbitMQ.Client.Events;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// 使用RabbitMQClient
    /// </summary>
    /// <seealso cref="CodeSnippet.RabbitMQ.TestBase" />
    [TestCategory("RebbitMQ-Client")]
    [TestClass()]
    public class RabbitMqClientPublisherTests : TestBase
    {
        private static readonly string queueName = $"test.pub.ut.queue";

        private static TestContext _testContext;
        private static readonly Stopwatch sw = new();

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestInitialize]
        public void TestCaseInit()
        {
            Console.WriteLine($"TestName: {_testContext.TestName}");
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

        /// <summary>
        /// 向queue中发布一条消息,如果queue不存在则会创建
        /// </summary>
        [TestMethod]
        public void PublisherTest_Simple()
        {
            // 创建名称为hello的队列
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
        /// 生产端消息确认(tx事务机制)
        /// </summary>
        [TestMethod]
        public void PublisherTest_Transaction()
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
        /// 生产端消息确认(Confirm模式)
        /// </summary>
        [TestMethod]
        public void PublisherTest_Confirm()
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
                Console.WriteLine($"Message发送成功");
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// 消息优先级
        /// </summary>
        [TestMethod]
        public void PublisherTest_Priority()
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

        [TestMethod]
        public void Dlx_PublisherTest()
        {
            for (int i = 0; i < 100; i++)
            {
                byte[] messageBody = Encoding.UTF8.GetBytes(i.ToString());

                // 设置消息持久化
                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                // 消息发送
                channel.BasicPublish(
                    exchange: "exchange",
                    routingKey: "#",
                    basicProperties: props,
                    body: messageBody);
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Dlx_ConsumerTest()
        {
            static void MessageConsumer(BasicDeliverEventArgs ea)
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message:{message}");
                if (message == "50")
                {
                    throw new Exception("error");
                }
            }

            EventingBasicConsumer consumer =
               new EventingBasicConsumer(channel);

            channel.BasicQos(0, 1, false);

            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    MessageConsumer(ea);
                    channel.BasicAck(
                               deliveryTag: ea.DeliveryTag,
                               multiple: false);
                }
                catch (Exception ex)
                {
                    // 出错了，发nack
                    channel.BasicNack(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false);
                    Console.WriteLine(ex.Message);
                }
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(queue: "queue",
                autoAck: false,
                consumer: consumer);
            Assert.IsTrue(true);
        }
    }
}
