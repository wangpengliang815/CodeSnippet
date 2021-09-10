namespace CommonLib.RabbitMQ
{
    using EasyNetQ;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class RabbitMQHelper
    {
        private readonly IBus bus;

        public RabbitMQHelper(string connectionString)
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
    }
}

