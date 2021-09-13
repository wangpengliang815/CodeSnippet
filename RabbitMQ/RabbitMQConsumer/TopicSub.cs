namespace RabbitMQConsumer
{
    using CommonLib.RabbitMQ;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    using System;
    using System.Text;

    internal static class TopicConsumer
    {
        private static readonly string exchangeName = $"test.exchange.topic";

        private static void Main(string[] args)
        {
            using RabbitMQHelper mq = new(new string[] { "192.168.181.191" });
            mq.UserName = "guest";
            mq.Password = "guest";
            mq.Port = 5672;

            mq.Received += (result) =>
            {
                Console.WriteLine($"message：{result.Body}");
                result.Commit();
            };
            mq.Listen(exchangeName, "test.topic.queue2", new ExchangeConsumeQueueOptions { AutoAck = false });
#if rabbitMQClient
            Console.WriteLine($"{nameof(TopicConsumer)}:");
            // RabbitMQ连接工厂
            ConnectionFactory factory = BaseConsumer.CreateRabbitMqConnection();
            // 建立连接
            using IConnection connection = factory.CreateConnection();
            // 创建信道
            using IModel channel = connection.CreateModel();

            string exchangeName = $"testExchange_topic";

            string routeKey = "testExchange_routeKey.*";

            //声明交换机并指定类型
            channel.ExchangeDeclare(exchange: exchangeName, type: "topic");

            string queueName = $"{exchangeName}_{nameof(TopicConsumer)}";
            // 声明队列
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // 将队列与交换机进行绑定
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routeKey);

            EventingBasicConsumer consumer = new(channel);

            channel.BasicQos(0, 1, false);

            consumer.Received += (model, ea) =>
            {
                string message =
                       Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Message:{message}");

                channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    // 是否一次性确认多条数据
                    multiple: false);
            };
            channel.BasicConsume(queue: queueName,
                  autoAck: false,
                  consumer: consumer);
#endif
            Console.ReadLine();
        }
    }
}