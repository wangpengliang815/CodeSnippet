namespace CodeSnippet.RabbitMQTests
{
    using System;
    using System.Text;
    using global::RabbitMQ.Client;

    public class TestBase
    {
        protected readonly ConnectionFactory factory;
        protected readonly IConnection connection;
        protected readonly IModel channel;

        protected TestBase()
        {
            // RabbitMQ连接工厂
            factory = new ConnectionFactory()
            {
                HostName = "192.168.181.191",
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
            connection = factory.CreateConnection();

            // 创建信道
            channel = connection.CreateModel();
        }

        protected void MessagePublisher(
            string queueName, 
            string message)
        {
            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "hello",
                basicProperties: null,
                body: messageBody);
        }
    }
}
