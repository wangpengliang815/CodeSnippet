namespace RabbitMQPublisher
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    /// <summary>
    /// w路由模式(direct),消息会发送到exchange
    /// 所有订阅了当前Exchange并且routingKey完全匹配的Queue都可以收到消息
    /// </summary>
    static class Direct
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("消息发布者:模式{direct}=>输入消息内容");
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

                    // 声明交换机
                    string exchangeName = $"test.exchange.direct";
                    channel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: "direct");

                    // 声明队列
                    string queue1 = "test.direct.queue1";
                    channel.QueueDeclare(queue1, false, false, false, null);

                    string queue2 = "test.direct.queue2";
                    channel.QueueDeclare(queue2, false, false, false, null);

                    //将队列与交换机进行绑定
                    channel.QueueBind(
                        queue: queue1,
                        exchange: exchangeName,
                        routingKey: "fanout");

                    channel.QueueBind(
                      queue: queue2,
                      exchange: exchangeName,
                      routingKey: "");

                    // 只有queue1可以收到消息,因为queue2的routingKey不匹配
                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: "fanout",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(message));
                }
            }
        }
    }
}
