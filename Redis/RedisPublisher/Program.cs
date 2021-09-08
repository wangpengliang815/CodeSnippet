namespace RedisPublisher
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
