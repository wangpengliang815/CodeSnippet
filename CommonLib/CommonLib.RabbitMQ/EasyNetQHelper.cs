namespace CommonLib.RabbitMQ
{
    using EasyNetQ;

    using global::RabbitMQ.Client;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class EasyNetQHelper
    {
        private readonly IBus bus;

        public EasyNetQHelper(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public async Task PublishAsync<TMessage>(TMessage message, string topic = null)
            where TMessage : class
        {
            if (string.IsNullOrWhiteSpace(topic))
                await bus.PubSub.PublishAsync(message);
            else
                await bus.PubSub.PublishAsync(message, x => x.WithTopic(topic));
        }

        public async Task PublishAsync<TMessage>(List<TMessage> messages, string topic)
            where TMessage : class
        {
            foreach (var message in messages)
            {
                if (string.IsNullOrWhiteSpace(topic))
                    await bus.PubSub.PublishAsync(message);
                else
                    await bus.PubSub.PublishAsync(message, x => x.WithTopic(topic));
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 给指定队列发送一条信息
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="message">消息</param>
        public async Task SendAsync<TMessage>(string queue, TMessage message)
            where TMessage : class
        {
            await bus.SendReceive.SendAsync(queue, message);
        }

        /// <summary>
        /// 给指定队列批量发送信息
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="messages">消息</param>
        public async Task SendManyAsync<TMessage>(string queue, IList<TMessage> messages)
            where TMessage : class
        {
            foreach (var message in messages)
            {
                await bus.SendReceive.SendAsync(queue, message);
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 从指定队列接收一天信息，并做相关处理。
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="process">
        /// 消息处理委托方法
        /// <para>
        /// <example>
        /// 例如：
        /// <code>
        /// message=>Task.Factory.StartNew(()=>{
        ///     Console.WriteLine(message);
        /// })
        /// </code>
        /// </example>
        /// </para>
        /// </param>
        public async Task ReceiveAsync<TMessage>(string queue, Func<TMessage, Task> handler)
            where TMessage : class
        {
            await bus.SendReceive.ReceiveAsync(queue, handler);
        }

        /// <summary>
        /// 消息订阅
        /// </summary>
        /// <param name="subscriptionId">消息订阅标识</param>
        /// <param name="process">
        /// 消息处理委托方法
        /// <para>
        /// <example>
        /// 例如：
        /// <code>
        /// message=>Task.Factory.StartNew(()=>{
        ///     Console.WriteLine(message);
        /// })
        /// </code>
        /// </example>
        /// </para>
        /// </param>
        public async Task SubscribeAsync<TMessage>(string subscriptionId, Func<TMessage, Task> handler)
            where TMessage : class
        {
            await bus.PubSub.SubscribeAsync<TMessage>(subscriptionId, message => handler(message));
        }

        /// <summary>
        /// 消息订阅
        /// </summary>
        /// <param name="subscriptionId">消息订阅标识</param>
        /// <param name="process">
        /// 消息处理委托方法
        /// <para>
        /// <example>
        /// 例如：
        /// <code>
        /// message=>Task.Factory.StartNew(()=>{
        ///     Console.WriteLine(message);
        /// })
        /// </code>
        /// </example>
        /// </para>
        /// </param>
        /// <param name="topic">topic</param>
        public async Task SubscribeWithTopicAsync<TMessage>(string subscriptionId, Func<TMessage, Task> process, string topic)
            where TMessage : class
        {
            await bus.PubSub.SubscribeAsync<TMessage>(subscriptionId, message => process(message), x => x.WithTopic(topic));
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            if (bus != null) bus.Dispose();
        }

        /// <summary>
        /// 交换器声明
        /// </summary>
        /// <param name="model"></param>
        /// <param name="exchangeName">交换器</param>
        /// <param name="type">交换器类型：
        /// 1、Direct Exchange – 处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全
        /// 匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “dog”，则只有被标记为“dog”的
        /// 消息才被转发，不会转发dog.puppy，也不会转发dog.guard，只会转发dog
        /// 2、Fanout Exchange – 不处理路由键。你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都
        /// 会被转发到与该交换机绑定的所有队列上。很像子网广播，每台子网内的主机都获得了一份复制的消息。Fanout
        /// 交换机转发消息是最快的。
        /// 3、Topic Exchange – 将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。符号“#”匹配一个或多
        /// 个词，符号“*”匹配不多不少一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*”
        /// 只会匹配到“audit.irs”。</param>
        /// <param name="durable">持久化</param>
        /// <param name="autoDelete">自动删除</param>
        /// <param name="arguments">参数</param>
        private static void ExchangeDeclare(IModel model, string exchangeName
            , string type
            , bool durable = true
            , bool autoDelete = false
            , IDictionary<string, object> arguments = null)
        {
            model.ExchangeDeclare(exchangeName, type, durable, autoDelete, arguments);
        }

        ///// <summary>
        ///// 队列声明
        ///// </summary>
        ///// <param name="channel"></param>
        ///// <param name="queue">队列</param>
        ///// <param name="durable">持久化</param>
        ///// <param name="exclusive">排他队列，如果一个队列被声明为排他队列，该队列仅对首次声明它的连接可见，
        ///// 并在连接断开时自动删除。这里需要注意三点：其一，排他队列是基于连接可见的，同一连接的不同信道是可
        ///// 以同时访问同一个连接创建的排他队列的。其二，“首次”，如果一个连接已经声明了一个排他队列，其他连
        ///// 接是不允许建立同名的排他队列的，这个与普通队列不同。其三，即使该队列是持久化的，一旦连接关闭或者
        ///// 客户端退出，该排他队列都会被自动删除的。这种队列适用于只限于一个客户端发送读取消息的应用场景。</param>
        ///// <param name="autoDelete">自动删除</param>
        ///// <param name="arguments">参数</param>
        //private static void QueueDeclare(IModel channel, string queue, bool durable = true, bool exclusive = false,
        //    bool autoDelete = false, IDictionary<string, object> arguments = null)
        //{
        //    queue = queue.IsNullOrWhiteSpace() ? "UndefinedQueueName" : queue.Trim();
        //    channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        //}

        ///// <summary>
        ///// 获取Model
        ///// </summary>
        ///// <param name="exchange">交换机名称</param>
        ///// <param name="queue">队列名称</param>
        ///// <param name="routingKey"></param>
        ///// <param name="isProperties">是否持久化</param>
        ///// <returns></returns>
        //private static IModel GetModel(string exchange, string queue, string routingKey, bool isProperties = false)
        //{
        //    return ModelDic.GetOrAdd(queue, key =>
        //    {
        //        var model = _conn.CreateModel();
        //        ExchangeDeclare(model, exchange, ExchangeType.Fanout, isProperties);
        //        QueueDeclare(model, queue, isProperties);
        //        model.QueueBind(queue, exchange, routingKey);
        //        ModelDic[queue] = model;
        //        return model;
        //    });
        //}

        ///// <summary>
        ///// 发布消息
        ///// </summary>
        ///// <param name="routingKey">路由键</param>
        ///// <param name="body">队列信息</param>
        ///// <param name="exchange">交换机名称</param>
        ///// <param name="queue">队列名</param>
        ///// <param name="isProperties">是否持久化</param>
        ///// <returns></returns>
        //public void Publish(string exchange, string queue, string routingKey, string body, bool isProperties = false)
        //{
        //    var channel = GetModel(exchange, queue, routingKey, isProperties);

        //    try
        //    {
        //        channel.BasicPublish(exchange, routingKey, null, body.SerializeUtf8());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.GetInnestException();
        //    }
        //}
    }
}

