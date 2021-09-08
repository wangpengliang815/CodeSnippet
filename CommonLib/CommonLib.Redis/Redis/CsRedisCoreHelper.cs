namespace CommonLib
{
    using CSRedis;

    /// <summary>
    /// 基于CsRedisCore封装的Helper,使用单例模式
    /// </summary>
    public class CsRedisCoreHelper
    {
        private static readonly object locker = new();

        private static CSRedisClient redisClient;

        public static string RedisConnectionString { get; set; }

        public CsRedisCoreHelper(string redisConnectionString)
        {
            RedisConnectionString = redisConnectionString;
        }

        public static CSRedisClient Instance
        {
            get
            {
                if (redisClient == null)
                {
                    lock (locker)
                    {
                        if (redisClient == null)
                        {
                            redisClient = new CSRedisClient(RedisConnectionString);
                        }
                    }
                }
                return redisClient;
            }
        }
    }
}
