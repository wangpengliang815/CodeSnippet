namespace RabbitMQConsumer
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    using System;
    using System.Text;

    internal static class FanoutConsumer
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("input queueName...");
            var input = Console.ReadLine();
            ConnectionFactory factory = BaseConsumer.CreateRabbitMqConnection();
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            EventingBasicConsumer consumer = new(channel);
            channel.BasicQos(0, 1, false);
            switch (input)
            {
                case "1":
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
                    Console.ReadLine();
                    break;
                case "2":
                    channel.BasicConsume(queue: "test.fanout.queue2", autoAck: false, consumer: consumer);
                    // 绑定消息接收后的事件委托
                    consumer.Received += (model, message) =>
                    {
                        Console.WriteLine($"Message:{Encoding.UTF8.GetString(message.Body.ToArray())}");

                        channel.BasicAck(
                            deliveryTag: message.DeliveryTag,
                            // 是否一次性确认多条数据
                            multiple: false);
                    };
                    Console.ReadLine();
                    break;
            }
        }
    }
}
