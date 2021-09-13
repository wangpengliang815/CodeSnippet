namespace RabbitMQPublisher
{
    using CommonLib.RabbitMQ;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 点对点：简单队列
    /// 消费者多开时,默认采用轮询(均摊)机制,也就是Work模式
    /// </summary>
    internal static class SimplePub
    {
        private static readonly string queueName = "test.simple.queue";

        private static void Main(string[] args)
        {
            // 消息生产
            using RabbitMQHelper mq = new(new string[] { "192.168.181.191" });
            mq.UserName = "guest";
            mq.Password = "guest";
            mq.Port = 5672;

            while (true)
            {
                Console.WriteLine("消息发布者:模式{简单队列}=>输入消息内容");
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    mq.Publish(queueName, message);
#if rabbitMQClient
                    //                    // RabbitMQ连接工厂
                    //                    ConnectionFactory factory = BasePublisher.CreateRabbitMqConnection();
                    //                    // 建立连接
                    //                    using IConnection connection = factory.CreateConnection();
                    //                    // 创建信道
                    //                    using IModel channel = connection.CreateModel();
                   
                    //                    // 声明队列
                    //                    channel.QueueDeclare(queueName, false, false, false, null);
                
                    //                    // 消息发送
                    //                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: Encoding.UTF8.GetBytes(message), mandatory: true);

                    //                    // 获取没有正常发送到队列中的消息
                    //                    channel.BasicReturn += (sender, message) =>
                    //                    {
                    //                        Console.WriteLine(Encoding.UTF8.GetString(message.Body.ToArray()));
                    //                    };
#endif
                }
            }
        }
    }
}