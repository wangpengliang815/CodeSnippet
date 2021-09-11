namespace CodeSnippet.RabbitMQTests
{
    using System;
    using System.Text;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("RabbitMQ")]
    [TestClass()]
    public class PolicyTests : TestBase
    {
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
