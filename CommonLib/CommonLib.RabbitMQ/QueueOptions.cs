namespace CommonLib.RabbitMQ
{
    using System.Collections.Generic;

    public class QueueOptions
    {
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }

    public class ConsumeQueueOptions : QueueOptions
    {
        /// <summary>
        /// 是否自动提交
        /// </summary>
        public bool AutoAck { get; set; } = false;

        /// <summary>
        /// 每次发送消息条数
        /// </summary>
        public ushort? FetchCount { get; set; }
    }

    public class ExchangeConsumeQueueOptions : ConsumeQueueOptions
    {
        /// <summary>
        /// 路由值
        /// </summary>
        public string[] RoutingKeys { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> BindArguments { get; set; } = new Dictionary<string, object>();
    }

    public class ExchangeQueueOptions : QueueOptions
    {
        /// <summary>
        /// 交换机类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 队列及路由值
        /// </summary>
        public (string, string)[] QueueAndRoutingKey { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public IDictionary<string, object> BindArguments { get; set; } = new Dictionary<string, object>();
    }

    public static class RabbitMQExchangeType
    {
        /// <summary>
        /// 普通模式
        /// </summary>
        public const string Common = "";

        /// <summary>
        /// 路由模式
        /// </summary>
        public const string Direct = "direct";

        /// <summary>
        /// 发布/订阅模式
        /// </summary>
        public const string Fanout = "fanout";

        /// <summary>
        /// 匹配订阅模式
        /// </summary>
        public const string Topic = "topic";
    }
}
