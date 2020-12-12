namespace RabbitMQPublisher
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    static class WorkerPublisher
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Publisher(Worker):Input Message Content:");
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
                    // 创建名称为hello的队列
                    channel.QueueDeclare("hello", false, false, false, null);
                    // 构建消息数据包
                    byte[] body = Encoding.UTF8.GetBytes(message);

                    // 消息发送
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "hello",
                        basicProperties: null,
                        body: body);
                }
            }
        }
    }
}
