namespace RabbitMQPublisher
{
    using System;
    using System.Text;

    using RabbitMQ.Client;

    /// <summary>Worker模式:信息以顺序的传输给每个接收者</summary>
    static class Worker
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

                    string queueName = "test.rabbitMq.worker.queue";
                    // 创建队列
                    channel.QueueDeclare(queueName, false, false, false, null);
                    // 构建消息数据包
                    byte[] body = Encoding.UTF8.GetBytes(message);

                    // 消息发送
                    channel.BasicPublish(
                        exchange: "",
                        // Worker模式下routingKey不写无法将消息发送到queue
                        routingKey: queueName,
                        basicProperties: null,
                        body: body);
                }
            }
        }
    }
}
