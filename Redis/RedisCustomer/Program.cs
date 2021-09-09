namespace RedisCustomer
{
    using CommonLib;

    using CSRedis;

    using Microsoft.Extensions.DependencyInjection;

    using System;

    class Program
    {
        private const string redisConnection = "192.168.48.143:6379";
        private const string channel = "Redis_Channel";

#if stackRedis
        static void Main(string[] args)
        {
            StackRedisHelper redis = new(0, redisConnection);
            string channel = "Redis_Channel";
            redis.Subscribe(channel, (redisChannel, redisValue) =>
            {
                Console.WriteLine(redisValue);
            });
            Console.ReadLine();
        }
#endif
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(p => new CSRedisClient(redisConnection));
            IServiceProvider provider = services.BuildServiceProvider();
            CSRedisClient redis = provider.GetService<CSRedisClient>();

            redis.Subscribe(
                 (channel, msg => Console.WriteLine(msg.Body)));
            Console.ReadLine();
        }
    }
}
