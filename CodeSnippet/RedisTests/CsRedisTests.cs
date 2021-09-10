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
    using System.Threading;

    /// <summary>
    /// 使用CSRedis
    /// </summary>
    [TestCategory("StandAloneCsRedisTests")]
    [TestClass()]
    public class CsRedisTests
    {
        private static TestContext _testContext;

        private const string redisConnection = "192.168.48.143:6379";

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
        /// 事务处理
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

        /// <summary>
        /// Key过期
        /// </summary>
        [TestMethod]
        public void KeyExpire()
        {
            string key = "name";
            string value = "wangpengliang";
            csRedis.Set(key, value);

            csRedis.Expire(key, 3);
            string result = csRedis.Get(key);
            Assert.AreEqual(value, result);
            Thread.Sleep(4000);
            result = csRedis.Get(key);
            Assert.AreEqual(null, result);
        }

        /// <summary>
        /// 缓存壳
        /// </summary>
        [TestMethod]
        public void CacheShell()
        {
            string value = "wangpengliang";

#if debug
            // 一般的缓存代码，如不封装比较繁琐
            var cacheValue = csRedis.Get("name");
            // 如果已被缓存
            if (!string.IsNullOrEmpty(cacheValue))
            {
                try
                {
                    // 
                }
                catch
                {
                    //出错时删除key
                    csRedis.Del("name");
                    throw;
                }
            }
            else
            {
                csRedis.Set("name", value, 10);
            }
#endif
            // 判断key=name是否已存在,存在返回value,不存在设置
            string t1 = csRedis.CacheShell("name", 10, () => value);
            Assert.AreEqual(value, t1);
            string t2 = csRedis.CacheShell("name", 10, () => "wangpengliang2");
            Assert.AreEqual(value, t2);
            string t3 = csRedis.CacheShell("name2", 10, () => "wangpengliang2");
            Assert.AreEqual("wangpengliang2", t3);
        }

        /// <summary>
        /// 多数据库,使用多个CSRedisClient实现
        /// </summary>
        [TestMethod]
        public void MultiDatabase()
        {
            // 实际使用必须要单例
            CSRedisClient[] redis = new CSRedisClient[14];
            for (int i = 0; i < redis.Length; i++)
            {
                redis[i] = new CSRedisClient($"{redisConnection},defaultDatabase={i}");
            }

            redis[0].Set("db0", "db0");
            string t1 = redis[0].Get("db0");
            Assert.AreEqual("db0", t1);


            redis[1].Set("db1", "db1");
            string t2 = redis[1].Get("db1");
            Assert.AreEqual("db1", t2);

            redis[2].Set("db2", "db2");
            string t3 = redis[2].Get("db2");
            Assert.AreEqual("db2", t3);
        }
    }
}
