namespace RabbitMQPublisher
{
    using System;
    using System.Text;
    using RabbitMQ.Client;

    /// <summary>
    /// 发布订阅模式(fanout),消息会发送到exchange,所有订阅了exchange的queue都可以收到消息
    /// type=fanout:routingKey不会生效
    /// </summary>
    static class Fanout
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("消息发布者:模式{fanout}=>输入消息内容");
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
                    string exchangeName = $"test.exchange.fanout";
                    channel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: "fanout");

                    // 声明队列
                    string queue1 = "test.fanout.queue1";
                    channel.QueueDeclare(queue1, false, false, false, null);

                    string queue2 = "test.fanout.queue2";
                    channel.QueueDeclare(queue2, false, false, false, null);

                    // 将队列与交换机进行绑定
                    channel.QueueBind(
                        queue: queue1,
                        exchange: exchangeName,
                        routingKey: "fanout");

                    channel.QueueBind(
                      queue: queue2,
                      exchange: exchangeName,
                      routingKey: "");

                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: "",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(message));
                }
            }
        }
    }
}
