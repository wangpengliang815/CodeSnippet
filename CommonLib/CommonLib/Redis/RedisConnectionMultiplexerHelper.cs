namespace RedisHelp
{
    using StackExchange.Redis;

    using System;

    public static class RedisConnectionMultiplexerHelper
    {
        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public static string RedisConnectionString { get; set; }

        private static readonly object locker = new();

        /// <summary>
        /// ConnectionMultiplexer对象
        /// </summary>
        private static ConnectionMultiplexer connectionMultiplexerInstance;

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (connectionMultiplexerInstance == null)
                {
                    lock (locker)
                    {
                        if (connectionMultiplexerInstance == null || !connectionMultiplexerInstance.IsConnected)
                        {
                            connectionMultiplexerInstance = GetConnectionMultiplexer();
                        }
                    }
                }
                return connectionMultiplexerInstance;
            }
        }

        private static ConnectionMultiplexer GetConnectionMultiplexer()
        {
            if (string.IsNullOrWhiteSpace(RedisConnectionString))
            {
                throw new Exception("RedisConnectionString IsNullOrWhiteSpace!");
            }
            var connect = ConnectionMultiplexer.Connect(RedisConnectionString);

            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            return connect;
        }

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine($"Configuration changed: {e.EndPoint}");
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine($"ErrorMessage:{e.Message}");
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($"ConnectionRestored:{e.EndPoint}");
        }

        /// <summary>
        /// 连接失败,如果重新连接成功将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($"重新连接：Endpoint failed:{e.EndPoint},{e.FailureType},{e.Exception.Message}");
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine($"HashSlotMoved:NewEndPoint={e.NewEndPoint},OldEndPoint={ e.OldEndPoint}");
        }

        /// <summary>
        /// Redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine($"InternalError:Message={e.Exception.Message}");
        }
    }
}