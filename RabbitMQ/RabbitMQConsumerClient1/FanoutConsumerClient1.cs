namespace RabbitMQConsumerClient1
{
    using System;
    using System.Text;

    using EasyNetQ;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    static class FanoutConsumerClient1
    {
        static void Main(string[] args)
        {
#if debug
            Console.WriteLine($"{nameof(FanoutConsumerClient1)}:");
            // RabbitMQ连接工厂
            var factory = BaseConsumer.CreateRabbitMqConnection();
            // 建立连接
            using var connection = factory.CreateConnection();
            // 创建信道
            using var channel = connection.CreateModel();

            string exchangeName = $"test.exchange.fanout";
            //声明交换机并指定类型
            channel.ExchangeDeclare(
                exchange: exchangeName,
                type: "fanout");

            string queueName = $"test.fanout.queue1";
            //声明队列
            channel.QueueDeclare(queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            //将队列与交换机进行绑定
            channel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "fanout");

            EventingBasicConsumer consumer =
                new EventingBasicConsumer(channel);

            // 每次只能向消费者发送一条信息,在消费者未确认之前,不再向它发送信息
            channel.BasicQos(0, 1, false);
            // 绑定消息接收后的事件委托
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

            Console.ReadLine();
        }
#endif
            var connStr = "host=192.168.252.191;username=guest;password=guest";

            using (var bus = RabbitHutch.CreateBus(connStr))
            {
                bus.PubSub.Subscribe<string>("easynet", (message) =>
                {
                    Console.WriteLine(message);
                });
                Console.ReadLine();
            }
        }

    }
}
