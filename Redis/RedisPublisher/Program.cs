namespace RedisPublisher
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

            while (true)
            {
                Console.WriteLine("please input message:");
                string message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("please again input message:");
                }
                if (message.Equals("exit"))
                {
                    return;
                }
                redis.Publish(channel, message);
            }
        }
#endif
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(p => new CSRedisClient(redisConnection));
            IServiceProvider provider = services.BuildServiceProvider();
            CSRedisClient redis = provider.GetService<CSRedisClient>();

            while (true)
            {
                Console.WriteLine("please input message:");
                string message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("please again input message:");
                }
                if (message.Equals("exit"))
                {
                    return;
                }
                redis.Publish(channel, message);
            }
        }
    }
}
