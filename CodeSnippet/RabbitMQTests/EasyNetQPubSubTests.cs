namespace CodeSnippet.RabbitMQTests
{
    using EasyNetQ;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    [TestCategory("EasyNetQ")]
    [TestClass()]
    public class EasyNetQPubSubTests
    {
        private static TestContext _testContext;
        private const string redisConnection = @"host=192.168.178.191;username=guest;password=guest";
        private static readonly IServiceCollection services = new ServiceCollection();
        private static IBus mq;

        private static readonly Stopwatch sw = new();

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
            services.AddSingleton(RabbitHutch.CreateBus(redisConnection));
            IServiceProvider provider = services.BuildServiceProvider();
            mq = provider.GetService<IBus>();
        }

        [TestInitialize]
        public void TestCaseInit()
        {
            Console.WriteLine($"TestName: {_testContext.TestName}");
            sw.Restart();
            Console.WriteLine($"{nameof(mq)} HashCode: {mq.GetHashCode()}");
        }

        /// <summary>
        /// Tests the cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            sw.Stop();
            mq.Dispose();
            Console.WriteLine($"time：{sw.ElapsedMilliseconds} Milliseconds");
        }

        /// <summary>
        /// Pub
        /// </summary>
        [TestMethod]
        public void PubSub_Pub()
        {
            for (int i = 0; i < 10; i++)
            {
                mq.PubSub.Publish(new MyMessage { Content = Faker.Name.FullName() });
            }
        }

        [TestMethod]
        public void PubSub_Sub()
        {
            mq.PubSub.Subscribe<MyMessage>("", msg =>
            {
                Console.WriteLine(msg.Content);
            });
            Thread.Sleep(3000);
        }
    }

    public class MyMessage
    {
        public string Content { get; set; }
    }
}

