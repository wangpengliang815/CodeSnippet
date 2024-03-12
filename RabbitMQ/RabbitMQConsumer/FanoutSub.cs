namespace RabbitMQConsumer
{
    using CommonLib.RabbitMQ;

    using System;

    internal static class FanoutConsumer
    {
        private static void Main(string[] args)
        {
            using RabbitMQHelper mq = new(new string[] { "47.93.34.29" });
            mq.UserName = "root";
            mq.Password = "wpl19950815";
            mq.Port = 5672;

            Console.WriteLine("input queueName...");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    mq.Received += (result) =>
                    {
                        Console.WriteLine($"message：{result.Body}");
                        result.Commit();
                    };
                    mq.Listen("test.fanout.queue1", new ConsumeQueueOptions { AutoAck = false });
                    Console.ReadLine();
                    break;
                case "2":
                    mq.Received += (result) =>
                    {
                        Console.WriteLine($"message：{result.Body}");
                        result.Commit();
                    };
                    mq.Listen("test.fanout.queue2", new ConsumeQueueOptions { AutoAck = false });
                    Console.ReadLine();
                    break;
            }
#if rabbitMQClient
            ConnectionFactory factory = BaseConsumer.CreateRabbitMqConnection();
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            EventingBasicConsumer consumer = new(channel);
            channel.BasicQos(0, 1, false);
            channel.BasicConsume(queue: "test.fanout.queue1", autoAck: false, consumer: consumer);
            // 绑定消息接收后的事件委托
            consumer.Received += (model, message) =>
            {
                Console.WriteLine($"Message:{Encoding.UTF8.GetString(message.Body.ToArray())}");

                channel.BasicAck(
                    deliveryTag: message.DeliveryTag,
                    // 是否一次性确认多条数据
                    multiple: false);
            };
#endif
        }
    }
}
