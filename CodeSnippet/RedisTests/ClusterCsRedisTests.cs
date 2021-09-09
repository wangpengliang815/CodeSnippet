namespace CodeSnippet.Redis
{
    using CSRedis;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Cluster集群模式部署的Redis测试,使用CSRedis
    /// </summary>
    [TestCategory("StandAloneCsRedisTests")]
    [TestClass()]
    public class ClusterCsRedisTests
    {
        private static TestContext _testContext;

        private const string redisConnection = "192.168.68.143:6379";

        private static readonly IServiceCollection services = new ServiceCollection();
        private static CSRedisClient csRedis;

        private static readonly Stopwatch sw = new();

        /// <summary>
        /// Classes the initialize.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
            services.AddSingleton(p => new CSRedisClient(redisConnection));
            IServiceProvider provider = services.BuildServiceProvider();
            csRedis = provider.GetService<CSRedisClient>();
        }

        /// <summary>
        /// Tests the case initialize.
        /// </summary>
        [TestInitialize]
        public void TestCaseInit()
        {
            Console.WriteLine($"TestName: {_testContext.TestName}");
            sw.Restart();
            // 这里为了测试注入时的声明周期
            Console.WriteLine($"{nameof(csRedis)} HashCode: {csRedis.GetHashCode()}");
        }

        /// <summary>
        /// Tests the cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            sw.Stop();
            Console.WriteLine($"time：{sw.ElapsedMilliseconds} Milliseconds");
        }

        [TestMethod]
        public void SetTest()
        {
            Parallel.For(0, 10, j =>
            {
                csRedis.Set("name", Faker.Name.FullName());
                //csRedis.Set("name2", "wangpengliang2");
                string strResult1 = csRedis.Get("name");
            });
            //string strResult2 = csRedis.Get("name2");
        }
    }
}
