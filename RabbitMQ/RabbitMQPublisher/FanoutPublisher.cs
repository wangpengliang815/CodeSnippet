namespace RabbitMQPublisher
{
    using CommonLib.RabbitMQ;

    using System;

    /// <summary>
    /// 发布订阅模式(fanout),消息会发送到exchange,所有订阅了exchange的queue都可以收到消息
    /// type=fanout：routingKey不会生效
    /// </summary>
    internal static class FanoutPublisher
    {
        private static readonly string exchangeName = $"test.exchange.fanout";

        private static void Main(string[] args)
        {
            // 消息生产
            using RabbitMQHelper mq = new(new string[] { "192.168.181.191" });
            mq.UserName = "guest";
            mq.Password = "guest";
            mq.Port = 5672;

            while (true)
            {
                Console.WriteLine("消息发布者:模式{fanout}=>输入消息内容");
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    mq.Publish(exchangeName, "", message);

#if rabbitMQClient
                    // RabbitMQ连接工厂
                    ConnectionFactory factory = BasePublisher.CreateRabbitMqConnection();
                    // 建立连接
                    using IConnection connection = factory.CreateConnection();
                    // 创建信道
                    using IModel channel = connection.CreateModel();

                    // 声明交换机
                    string exchangeName = $"test.exchange.fanout";
                    channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");

                    // 声明队列
                    string queue1 = "test.fanout.queue1";
                    channel.QueueDeclare(queue1, false, false, false, null);

                    string queue2 = "test.fanout.queue2";
                    channel.QueueDeclare(queue2, false, false, false, null);

                    // 将队列与交换机进行绑定
                    channel.QueueBind(queue: queue1, exchange: exchangeName, routingKey: "fanout");

                    channel.QueueBind(queue: queue2, exchange: exchangeName, routingKey: "");

                    channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: Encoding.UTF8.GetBytes(message));
#endif
                }
            }
        }
    }
}
