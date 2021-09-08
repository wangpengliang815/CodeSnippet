//#define one
#define two
//#define three
//#define four
namespace CodeSnippet.Redis
{
    using CSRedis;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// 单机部署的Redis测试,使用CSRedis
    /// </summary>
    [TestCategory("StandAloneCsRedisTests")]
    [TestClass()]
    public class StandAloneCsRedisTests
    {
        private static TestContext _testContext;

        private const string redisConnection = "192.168.55.143:6379";

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
            string strKey = Guid.NewGuid().ToString();
            string Strval = Faker.Name.FullName();
            csRedis.Set($"{strKey}", Strval);
            string strResult = csRedis.Get(strKey);
            Assert.AreEqual(Strval, strResult);

            string intKey = Guid.NewGuid().ToString();
            int intVal = Faker.RandomNumber.Next(1, 10);
            csRedis.Set($"{intKey}", intVal);
            int intResult = csRedis.Get<int>(intKey);
            Assert.AreEqual(intVal, intResult);
        }

        /// <summary>
        /// 关于事务的处理
        /// </summary>
        [TestMethod]
        public void Transaction()
        {
            string strKey = Guid.NewGuid().ToString();
            string strval = Faker.Name.FullName();

            string intKey = Guid.NewGuid().ToString();
            int intVal = Faker.RandomNumber.Next(1, 10);

#if one
            // 第一种方式:正常情况下redis命令批处理,返回[true,true]
            object[] result = csRedis.StartPipe()
                .Set(strKey, strval)
                .Set(intKey, intVal)
                .EndPipe();
            Console.WriteLine(JsonConvert.SerializeObject(result));
#endif

#if two
            // 第二种方式：对string类型进行IncrBy操作,会抛出异常但不影响执行,返回[false, false, null]; Redis中多了两条数据
            object[] result2 = csRedis.StartPipe()
                .Set(strKey, strval)
                .Set(intKey, intVal)
                .IncrBy(strKey, 5)
                .EndPipe();
            Console.WriteLine(JsonConvert.SerializeObject(result2));
#endif

#if three
            // 第三种方式：对string类型进行IncrBy操作,错误的命令不被执行,Redis中多了两条数据
            using var rc = csRedis.Nodes.First().Value.Get();
            rc.Value.Multi();
            rc.Value.SAdd(strKey, strval);
            rc.Value.SAdd(intKey, intVal);
            rc.Value.SAdd(strKey, 5);
            // 此时报错：EXEC  ---> CSRedis.RedisException: WRONGTYPE Operation against a key holding the wrong kind of value
            rc.Value.Exec();
#endif
#if four
            // 第四种方式：对string类型进行IncrBy操作,错误的命令不被执行,Redis中多了两条数据
            var tran = csRedis.StartPipe();
            tran.Set(strKey, strval);
            tran.Set(intKey, intVal);
            tran.IncrBy(strKey, 5);
            var b = tran.EndPipe();
#endif
        }
    }
}
