namespace RabbitMQPublisher
{
    using System;
    using System.Text;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// 点对点:最简单的工作模式
    /// </summary>
    static class PointToPointPublisher
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("消息发布者:模式{点对点}=>输入消息内容");
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    // RabbitMQ连接工厂
                    var factory = new ConnectionFactory()
                    {
                        HostName = "localhost",
                        // 用户名
                        UserName = "guest",
                        // 密码
                        Password = "guest",
                        // 网络故障自动恢复连接
                        AutomaticRecoveryEnabled = true,
                        // 心跳处理
                        RequestedHeartbeat = new TimeSpan(5000)
                    };
                    // 建立连接
                    using var connection = factory.CreateConnection();
                    // 创建信道
                    using var channel = connection.CreateModel();

                    // 声明队列
                    string queueName = "test.pointToPoint.queue";
                    channel.QueueDeclare(queueName, false, false, false, null);

                    // 消息发送
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(message));
                }
            }
        }
    }

    static class PointToPointConsumer
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{nameof(PointToPointConsumer)}:");
            // RabbitMQ连接工厂
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                // 用户名
                UserName = "guest",
                // 密码
                Password = "guest",
                // 网络故障自动恢复连接
                AutomaticRecoveryEnabled = true,
                // 心跳处理
                RequestedHeartbeat = new TimeSpan(5000)
            };
            // 建立连接
            using var connection = factory.CreateConnection();
            // 创建信道
            using var channel = connection.CreateModel();

            string queueName = "test.pointToPoint.queue";
            // 申明队列
            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

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
    }
}
