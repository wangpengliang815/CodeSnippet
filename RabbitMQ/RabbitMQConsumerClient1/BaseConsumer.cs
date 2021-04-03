﻿namespace RabbitMQConsumerClient1
{
    using System;

    using RabbitMQ.Client;

    public static class BaseConsumer
    {
        public static ConnectionFactory CreateRabbitMqConnection()
        {
            // RabbitMQ连接工厂
            return new ConnectionFactory()
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
        }
    }
}
