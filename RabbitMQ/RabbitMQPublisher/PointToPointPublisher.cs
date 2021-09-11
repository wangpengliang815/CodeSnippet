#define mandatory
namespace RabbitMQPublisher
{
    using RabbitMQ.Client;

    using System;
    using System.Text;

    /// <summary>
    /// 点对点：简单队列
    /// 消费者多开时,默认采用轮询(均摊)机制,也就是Work模式
    /// </summary>
    internal static class PointToPointPublisher
    {
        readonly static string queueName = "test.pointToPoint.queue";

        private static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("消息发布者:模式{点对点}=>输入消息内容");
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    // RabbitMQ连接工厂
                    ConnectionFactory factory = BasePublisher.CreateRabbitMqConnection();
                    // 建立连接
                    using IConnection connection = factory.CreateConnection();
                    // 创建信道
                    using IModel channel = connection.CreateModel();
#if mandatory
                    // 声明队列
                    channel.QueueDeclare(queueName, false, false, false, null);
#endif
                    // 消息发送
                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: Encoding.UTF8.GetBytes(message), mandatory: true);

                    // 获取没有正常发送到队列中的消息
                    channel.BasicReturn += (sender, message) =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(message.Body.ToArray()));
                    };
                }
            }
        }
    }
}