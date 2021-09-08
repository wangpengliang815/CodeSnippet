namespace RedisCustomer
{
    using CommonLib;

    using System;

    class Program
    {
        private const string redisConnection = "192.168.31.143:6379";

        static void Main(string[] args)
        {
            StackExchangeRedisHelper redis = new(0, redisConnection);
            string channel = "Redis_Channel";
            redis.Subscribe(channel, (redisChannel, redisValue) =>
            {
                Console.WriteLine(redisValue);
            });
            Console.ReadLine();
        }
    }
}
