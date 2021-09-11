namespace RabbitMQConsumer
{
    using CommonLib.RabbitMQ;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    using System;
    using System.Text;

    static class PointToPointConsumer
    {
        static readonly string queueName = "test.pointToPoint.queue";

        private static void Main(string[] args)
        {
            //Console.WriteLine($"{nameof(PointToPointConsumer)}:");

            //// RabbitMQ连接工厂
            //ConnectionFactory factory = BaseConsumer.CreateRabbitMqConnection();

            //// 建立连接
            //using IConnection connection = factory.CreateConnection();

            //// 创建信道
            //using IModel channel = connection.CreateModel();

            //EventingBasicConsumer consumer = new(channel);

            //// 每次只能向消费者发送一条信息,在消费者未确认之前,不再向它发送信息
            //channel.BasicQos(0, 1, false);

            //channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            //// 绑定消息接收后的事件委托
            //consumer.Received += (model, message) =>
            //{
            //    Console.WriteLine($"Message:{Encoding.UTF8.GetString(message.Body.ToArray())}");

            //    channel.BasicAck(
            //        deliveryTag: message.DeliveryTag,
            //        // 是否一次性确认多条数据
            //        multiple: false);
            //};
            //RabbitMQHelper2.connectionString = "192.168.31.191;guest;guest";
            //IConnection conn = RabbitMQHelper2.CreateMQConnectionInPools();
            ////for (int i = 0; i < 1000; i++)
            ////{
            ////    string result = RabbitMQHelper.SendMsg(conn, "test.direct.queue1", Faker.Address.City(), false);
            ////}
            //RabbitMQHelper2.ConsumeMsg(conn, "test.direct.queue1", false, (msg) =>
            //{
            //    Console.WriteLine(msg);
            //    return ConsumeAction.ACCEPT;
            //}, (a, b) =>
            //{
            //    Console.WriteLine($"{a},{b}");
            //});
            string[] hosts = new string[] { "192.168.31.191"};
            int port = 5672;
            string userName = "guest";
            string password = "guest";
            string virtualHost = "/";
            RabbitMQHelper helper = new RabbitMQHelper(hosts);
            using (RabbitMQHelper.RabbitMQConsumer consumer = new RabbitMQHelper.RabbitMQConsumer(hosts))
            {
                consumer.UserName = userName;
                consumer.Password = password;
                consumer.Port = port;
                consumer.VirtualHost = virtualHost;

                consumer.Received += result =>
                {
                    Console.WriteLine($"消费者1接收到数据：{result.Body}");
                    result.Commit();//提交
                };
                consumer.Listen("test.direct.queue1", options =>
                {
                    options.AutoAck = false;
                    options.Durable = false;
                });
            }

            Console.ReadLine();
        }
    }
}
