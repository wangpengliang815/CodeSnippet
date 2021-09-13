namespace RabbitMQPublisher
{
    using CommonLib.RabbitMQ;

    using System;

    /// <summary>
    /// 路由模式(direct),消息会发送到exchange
    /// 所有订阅了当前Exchange并且routingKey完全匹配的Queue都可以收到消息
    /// </summary>
    internal static class DirectPub
    {
        private static readonly string exchangeName = $"test.exchange.direct";

        private static void Main(string[] args)
        {
            using RabbitMQHelper mq = new(new string[] { "192.168.181.191" });
            mq.UserName = "guest";
            mq.Password = "guest";
            mq.Port = 5672;

            while (true)
            {
                Console.WriteLine("消息发布者:模式{direct}=>输入消息内容");
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {               
                    mq.Publish(exchangeName, "direct", message, new ExchangeQueueOptions { Type = RabbitMQExchangeType.Direct });
#if rabbitMQClient
                    ConnectionFactory factory = BasePublisher.CreateRabbitMqConnection();
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();
                    // 声明交换机
                    string exchangeName = $"test.exchange.direct";
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                    // 声明队列
                    string queue1 = "test.direct.queue1";
                    channel.QueueDeclare(queue1, false, false, false, null);

                    string queue2 = "test.direct.queue2";
                    channel.QueueDeclare(queue2, false, false, false, null);

                    //将队列与交换机进行绑定
                    channel.QueueBind(queue: queue1, exchange: exchangeName, routingKey: "fanout");

                    channel.QueueBind(queue: queue2, exchange: exchangeName, routingKey: "");

                    // 只有queue1可以收到消息,因为queue2的routingKey不匹配
                    channel.BasicPublish(exchange: exchangeName, routingKey: "fanout", basicProperties: null, body: Encoding.UTF8.GetBytes(message));
#endif
                }
            }
        }
    }
}
