namespace RabbitMQPublisher
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    static class ExchangeDirectPublisher
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Publisher(direct):Input Message Content:");
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
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
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    string exchangeName = $"testExchange_direct";

                    string routeKeyName = "testExchange_routeKey";

                    // 声明交换机并设置类型为direct
                    channel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: "direct");

                    byte[] body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: routeKeyName,
                        basicProperties: null,
                        body: body);
                }
            }
        }
    }
}
