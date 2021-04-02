namespace RabbitMQPublisher
{
    using System;
    using System.Text;

    using RabbitMQ.Client;

    /// <summary>
    /// Worker模式:信息以顺序的传输给每个接收者
    /// </summary>
    static class WorkerPublisher
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("消息发布者:模式{Worker}=>输入消息内容");
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
                    string queueName = "test.worker.queue";
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
}
