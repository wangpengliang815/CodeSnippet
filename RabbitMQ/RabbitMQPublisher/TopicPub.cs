namespace RabbitMQPublisher
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CommonLib.RabbitMQ;

    using RabbitMQ.Client;

    /// <summary>
    /// 路由模式(topic),消息会发送到exchange
    /// topic与direct模式区别在于routingKey可以声明为模糊查询，RabbitMQ拥有两个通配符
    /// #：匹配0-n个字符语句
    /// *：匹配一个字符语句
    /// </summary>
    static class TopicPublisher
    {
        private static readonly string exchangeName = $"test.exchange.topic";

        static void Main(string[] args)
        {
            using RabbitMQHelper mq = new(new string[] { "test.exchange.topic" });
            mq.UserName = "root";
            mq.Password = "wpl19950815";
            mq.Port = 5672;

            while (true)
            {
                Console.WriteLine("消息发布者:模式{topic}=>输入消息内容");
                string message = Console.ReadLine();

                if (!string.IsNullOrEmpty(message))
                {
                    // routingKey = "test.one.one":只有test.topic.queue2可以收到消息,因为#匹配0个或多个单词
                    // routingKey = "test.one"    :两个queue都可以收到消息
                    string routingKey = "test.one.one";

                    mq.Publish(exchangeName, routingKey, message, new ExchangeQueueOptions
                    {
                        Type = RabbitMQExchangeType.Topic,
                        QueueAndRoutingKey = new List<Tuple<string, string>>() {
                              new Tuple<string, string>("test.topic.queue1", "test.*"),
                              new Tuple<string, string>("test.topic.queue2", "test.#")
                        }
                    });

#if rabbitMQClient
                    ConnectionFactory factory = BasePublisher.CreateRabbitMqConnection();
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    // 声明交换机
                    string exchangeName = $"test.exchange.topic";
                    channel.ExchangeDeclare(exchange: exchangeName, type: "topic");

                    // 声明队列
                    string queue1 = "test.topic.queue1";
                    channel.QueueDeclare(queue1, false, false, false, null);

                    string queue2 = "test.topic.queue2";
                    channel.QueueDeclare(queue2, false, false, false, null);

                    //将队列与交换机进行绑定
                    channel.QueueBind(queue: queue1, exchange: exchangeName, routingKey: "topic.*");

                    channel.QueueBind(queue: queue2, exchange: exchangeName, routingKey: "topic.#");

                    // queue1和queue2都可以收到消息
                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: "topic.test",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(message));

                    // 只有queue2可以收到消息,因为.#可以匹配一个或者多个字符语句而.*只能匹配单个
                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: "topic.test.test",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(message));
                }
#endif
                }
            }
        }
    }
}