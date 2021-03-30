namespace RabbitMQPublisher
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    /// <summary>
    /// Exchange-发布订阅模式(fanout)
    /// </summary>
    static class Fanout
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Publisher(fanout):Input Message Content:");
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

                    string exchangeName = $"test.rabbitMq.worker";
                    // 声明交换机
                    channel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: "fanout");

                    byte[] body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: "",
                        basicProperties: null,
                        body: body);
                }
            }
        }
    }
}
