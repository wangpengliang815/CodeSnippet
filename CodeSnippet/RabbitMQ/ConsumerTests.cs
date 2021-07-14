#define RabbitMQRuning
namespace codeSnippet.RabbitMQ
{
    using System;
    using System.Text;
    using System.Threading;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("RabbitMQ")]
    [TestClass()]
    public class ConsumerTests : TestBase
    {
        [TestCleanup]
        public void TestCleanup()
        {
            channel.Dispose();
            connection.Dispose();
        }

#if RabbitMQRuning
        /// <summary>
        /// 消费端优先级
        /// </remarks>
        [TestMethod]
        public void ConsumerTest_Priority()
        {
            EventingBasicConsumer consumer =
                new EventingBasicConsumer(channel);

            // 绑定消息接收后的事件委托
            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message：{message}");
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(
                queue: "myqueue",
                autoAck: true,
                consumer: consumer);
        }

        /// <summary>
        /// 消费端自动确认
        /// </summary>
        /// <remarks>
        /// autoAck设置为true
        /// channel会自动在处理完上一条消息之后,接收下一条消息。（同一个channel消息处理是串行的）
        /// 除非关闭channel或者取消订阅,否则客户端将会一直接收队列的消息
        /// </remarks>
        [TestMethod]
        public void ConsumerTest_AutoAck()
        {
            MessagePublisher("hello", "test");

            // 申明队列
            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            EventingBasicConsumer consumer =
                new EventingBasicConsumer(channel);

            // 绑定消息接收后的事件委托
            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message：{message}");
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(
                queue: "hello",
                autoAck: true,
                consumer: consumer);
        }

        /// <summary>
        /// 消费端手动确认
        /// </summary>
        /// <remarks>
        /// autoAck设置为false
        /// 通过BasicAck手动确认消费,自动消费有一个弊端是如果消费端收到了消息,但在后续处理代码中
        /// 抛出了异常,这条消息将会丢失,因为消费端收到消息因为是自动ackRabbitMQ就会将该消息删除
        /// <remarks>
        [TestMethod]
        public void ConsumerTest_BasicAck()
        {
            for (int i = 0; i < 2; i++)
            {
                MessagePublisher("hello", $"test_{i}");
            }

            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            EventingBasicConsumer consumer =
                new EventingBasicConsumer(channel);

            channel.BasicQos(0, 1, false);

            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message：{message}");
                Assert.IsNotNull(message);

                channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            };

            channel.BasicConsume(queue: "hello",
                autoAck: false,
                consumer: consumer);
            Thread.Sleep(5000);
        }

        /// <summary>
        /// 消费端消息拒绝(BasicReject:一次只能拒绝一条消息)
        /// </summary>
        /// <remarks>
        /// 把消息塞回的队列(头部不是尾部）
        /// 该测试将会死循环
        /// <remarks>
        [TestMethod]
        public void ConsumerTest_BasicReject()
        {
            MessagePublisher("hello", $"1");
            MessagePublisher("hello", $"2");
            MessagePublisher("hello", $"3");

            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            EventingBasicConsumer consumer =
                new EventingBasicConsumer(channel);

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

            channel.BasicConsume(queue: "hello",
                autoAck: false,
                consumer: consumer);
            Thread.Sleep(5000);
        }

        /// <summary>
        /// 消费端消息拒绝(BasicNack:可以一次拒绝N条消息)
        /// </summary>
        [TestMethod]
        public void ConsumerTest_BasicNack()
        {
            for (int i = 0; i < 100; i++)
            {
                MessagePublisher("hello", i.ToString());
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

            channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            EventingBasicConsumer consumer =
                new EventingBasicConsumer(channel);

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
                    channel.BasicNack(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true);
                    Console.WriteLine(ex.Message);
                }
                Assert.IsNotNull(message);
            };

            channel.BasicConsume(queue: "hello",
                autoAck: false,
                consumer: consumer);

            Thread.Sleep(50000);
        }

        /// <summary>
        /// BasicGet()消费端主动获取消息
        /// </summary>
        [TestMethod]
        public void ConsumerTest_BasicGet()
        {
            BasicGetResult result = channel.BasicGet(queue: "test", autoAck: true);
            Assert.IsNotNull(result.Body.ToArray());
            Console.WriteLine($"接收到消息{Encoding.UTF8.GetString(result.Body.ToArray())}");
            // 打印exchange和routingKey
            Console.WriteLine($"exchange：{result.Exchange},routingKey:{result.RoutingKey}");
        }
    }
#endif
}
