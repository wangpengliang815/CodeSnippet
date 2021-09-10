namespace CodeSnippet.Redis
{
    using CommonLib;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using StackExchange.Redis;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// 单机部署的Redis测试,使用StackExchange.Redis
    /// </summary>
    [TestCategory("StandAloneStackRedisTests")]
    [TestClass()]
    public class StackRedisTests
    {
        private static TestContext _testContext;

        private const string redisConnection = "192.168.31.143:6379";
#if nonuseIoc
        private readonly StackExchangeRedisHelper stackRedis = new(0, redisConnection);
#endif

        private static readonly IServiceCollection services = new ServiceCollection();
        private static StackRedisHelper stackRedis;

        private static readonly Stopwatch sw = new();

        /// <summary>
        /// Classes the initialize.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
            services.AddSingleton(p => new StackRedisHelper(0, redisConnection));
            IServiceProvider provider = services.BuildServiceProvider();
            stackRedis = provider.GetService<StackRedisHelper>();
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
            Console.WriteLine($"{nameof(stackRedis)} HashCode: {stackRedis.GetHashCode()}");
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
        public void SetString()
        {
            bool result = stackRedis.StringSet($"{Guid.NewGuid()}", Faker.Name.FullName());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SetStringList()
        {
            Console.WriteLine(stackRedis.GetHashCode());
            List<KeyValuePair<RedisKey, RedisValue>> keyValues = new();
            for (int i = 0; i < 5; i++)
            {
                keyValues.Add(new KeyValuePair<RedisKey, RedisValue>($"{Guid.NewGuid()}", Faker.Name.FullName()));
            }
            bool result = stackRedis.StringSet(keyValues);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SetObject()
        {
            bool result = stackRedis.StringSet("user", new
            {
                name = "wangpengliang",
                age = 25,
                address = "BEIJING"
            });
            Assert.IsTrue(result);

            bool result1 = stackRedis.StringSet("user1", 100);
            Assert.IsTrue(result1);

            bool result2 = stackRedis.StringSet("user2", "100");
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void GetString()
        {
            string key = "name";
            string value = "wangpengliang";
            stackRedis.StringSet(key, value);
            string result = stackRedis.StringGet(key);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void GetStringList()
        {
            int count = 5;
            List<string> listKeys = new();
            RedisValue[] redisValues = new RedisValue[count];
            List<KeyValuePair<RedisKey, RedisValue>> keyValues = new();

            for (int i = 0; i < count; i++)
            {
                string key = $"k_{i}";
                string value = $"v_{i}";

                listKeys.Add(key);
                redisValues[i] = value;

                keyValues.Add(new KeyValuePair<RedisKey, RedisValue>(key, value));
            }
            stackRedis.StringSet(keyValues);

            RedisValue[] result = stackRedis.StringGet(listKeys);

            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(result[i], redisValues[i]);
            }
        }

        [TestMethod]
        public void StringIncrement()
        {
            string key = Guid.NewGuid().ToString();
            double result = stackRedis.StringIncrement(key);
            Assert.AreEqual(1, result);
            result = stackRedis.StringIncrement(key, 10);
            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void StringDecrement()
        {
            string key = Guid.NewGuid().ToString();
            double result = stackRedis.StringDecrement(key);
            Assert.AreEqual(-1, result);
            result = stackRedis.StringDecrement(key);
            Assert.AreEqual(-2, result);
        }

        [TestMethod]
        public void HashSet()
        {
            string key = Guid.NewGuid().ToString();
            bool result = stackRedis.HashSet(key, "name", "wangpengliang");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HashExists()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.HashSet(key, "name", "wangpengliang");
            Assert.IsTrue(stackRedis.HashExists(key, "name"));
            Assert.IsFalse(stackRedis.HashExists(key, "age"));
        }

        [TestMethod]
        public void HashDelete()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.HashSet(key, "name", "wangpengliang");
            bool result = stackRedis.HashDelete(key, "name");
            Assert.IsTrue(result);
            Assert.IsFalse(stackRedis.HashExists(key, "name"));
        }

        [TestMethod]
        public void HashDeleteList()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.HashSet(key, "name", "wangpengliang");
            stackRedis.HashSet(key, "age", "25");

            List<RedisValue> hashFieldNames = new()
            {
                new RedisValue("name"),
                new RedisValue("age")
            };

            stackRedis.HashDelete(key, hashFieldNames);

            Assert.IsFalse(stackRedis.HashExists(key, "name"));
            Assert.IsFalse(stackRedis.HashExists(key, "age"));
        }

        [TestMethod]
        public void HashGet()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.HashSet(key, "name", "wangpengliang");
            stackRedis.HashSet(key, "age", "25");

            string result = stackRedis.HashGet<string>(key, "name");
            Assert.AreEqual("wangpengliang", result);

            key = Guid.NewGuid().ToString();
            User person = new User
            {
                Name = "wangpengliang",
                Age = 25
            };
            stackRedis.HashSet(key, "user", person);

            User result2 = stackRedis.HashGet<User>(key, "user");
            Assert.AreEqual("wangpengliang", result2.Name);
        }

        [TestMethod]
        public void HashIncrement()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.HashSet(key, "name", "wangpengliang");
            stackRedis.HashSet(key, "age", "25");

            stackRedis.HashIncrement(key, "age");
            Assert.AreEqual("26", stackRedis.HashGet<string>(key, "age"));
        }

        [TestMethod]
        public void HashDecrement()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.HashSet(key, "name", "wangpengliang");
            stackRedis.HashSet(key, "age", "25");

            stackRedis.HashDecrement(key, "age");
            Assert.AreEqual("24", stackRedis.HashGet<string>(key, "age"));
        }

        [TestMethod]
        public void HashKeys()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.HashSet(key, "name", "wangpengliang");
            stackRedis.HashSet(key, "age", "25");

            List<string> result = stackRedis.HashKeys<string>(key);
            Assert.IsTrue(result.Count == 2);
        }

        /// <summary>
        /// Queue
        /// </summary>
        [TestMethod]
        public void ListQueue()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.ListRightPush(key, "1");
            stackRedis.ListRightPush(key, "2");
            stackRedis.ListRightPush(key, "3");

            string result1 = stackRedis.ListRightPop<string>(key);
            Assert.AreEqual("3", result1);

            string result2 = stackRedis.ListLeftPop<string>(key);
            Assert.AreEqual("1", result2);
        }

        [TestMethod]
        public void ListStack()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.ListLeftPush(key, "3");
            stackRedis.ListLeftPush(key, "2");
            stackRedis.ListLeftPush(key, "1");

            string result1 = stackRedis.ListRightPop<string>(key);
            Assert.AreEqual("3", result1);

            string result2 = stackRedis.ListLeftPop<string>(key);
            Assert.AreEqual("1", result2);
        }

        [TestMethod]
        public void ListLength()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.ListLeftPush(key, "3");
            stackRedis.ListLeftPush(key, "2");
            stackRedis.ListLeftPush(key, "1");

            long result = stackRedis.ListLength(key);
            Assert.AreEqual(3, result);

            stackRedis.ListLeftPop<string>(key);
            result = stackRedis.ListLength(key);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void SortedSetAdd()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.SortedSetAdd(key, "wang", 10);
            stackRedis.SortedSetAdd(key, "li", 11);
            stackRedis.SortedSetAdd(key, "zhou", 12);
            stackRedis.SortedSetAdd(key, "zhang", 4);

            long result = stackRedis.SortedSetLength(key);
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void SortedSetRemove()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.SortedSetAdd(key, "wang", 10);
            stackRedis.SortedSetAdd(key, "li", 11);
            stackRedis.SortedSetAdd(key, "zhou", 12);
            stackRedis.SortedSetAdd(key, "zhang", 4);

            stackRedis.SortedSetRemove(key, "zhou");

            long result = stackRedis.SortedSetLength(key);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void SortedSetRangeByRank()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.SortedSetAdd(key, "wang", 10);
            stackRedis.SortedSetAdd(key, "li", 11);
            stackRedis.SortedSetAdd(key, "zhou", 12);
            stackRedis.SortedSetAdd(key, "zhang", 4);
            List<string> result = stackRedis.SortedSetRangeByRank<string>(key, 0, 2);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void KeyDelete()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.StringSet(key, $"{nameof(KeyDelete)}");

            stackRedis.KeyDelete(key);
            string result = stackRedis.StringGet(key);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void KeyDeleteList()
        {
            string key1 = Guid.NewGuid().ToString();
            string key2 = Guid.NewGuid().ToString();
            stackRedis.StringSet(key1, $"{nameof(KeyDeleteList)}");
            stackRedis.StringSet(key2, $"{nameof(KeyDeleteList)}");

            stackRedis.KeyDelete(new List<string> { key1, key2 });
            RedisValue[] result = stackRedis.StringGet(new List<string> { key1, key2 });
            foreach (RedisValue item in result)
            {
                Assert.IsTrue(item.IsNull);
            }
        }


        [TestMethod]
        public void KeyExists()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.StringSet(key, $"{nameof(KeyExists)}");
            Assert.IsTrue(stackRedis.KeyExists(key));

            stackRedis.KeyDelete(key);
            Assert.IsFalse(stackRedis.KeyExists(key));
        }

        [TestMethod]
        public void KeyRename()
        {
            string key = Guid.NewGuid().ToString();
            string value = "wangpengliang";
            stackRedis.StringSet(key, value);
            string newKey = Guid.NewGuid().ToString();
            stackRedis.KeyRename(key, newKey);
            string result = stackRedis.StringGet(newKey);

            Assert.AreEqual("wangpengliang", result);
        }

        [TestMethod]
        public void KeyExpire()
        {
            string key = Guid.NewGuid().ToString();
            string value = "wangpengliang";
            stackRedis.StringSet(key, value);
            stackRedis.StringGet(key);

            stackRedis.KeyExpire(key, TimeSpan.FromSeconds(3));
            string result = stackRedis.StringGet(key);
            Assert.AreEqual("wangpengliang", result);

            Thread.Sleep(3000);

            result = stackRedis.StringGet(key);
            Assert.AreEqual(null, result);
        }

        /// <summary>
        /// 关于事务的处理
        /// StackExchange.Redis：命令不会抛出异常,所有正常的命令都会被执行
        /// </summary>
        [TestMethod]
        public void CreateTransaction()
        {
            ITransaction tran = stackRedis.CreateTransaction(0);
            string strKey = Guid.NewGuid().ToString();
            string Strval = Faker.Name.FullName();
            tran.StringSetAsync(strKey, Strval);
            tran.StringIncrementAsync($"{strKey}", 5);

            string intKey = Guid.NewGuid().ToString();
            int intVal = Faker.RandomNumber.Next(1, 10);
            tran.StringSetAsync(intKey, intVal);

            bool committed = tran.Execute();
            Assert.IsTrue(committed);
        }

        /// <summary>
        /// Redis有三个最基本属性来保证分布式锁的有效实现
        /// 安全性：互斥,在任何时候,只有一个客户端能持有锁
        /// 活跃性：没有死锁.即使客户端在持有锁的时候崩溃,最后也会有其他客户端能获得锁,超时机制
        /// 活跃性：故障容忍.只有大多数Redis节点时存活的,客户端仍可以获得锁和释放锁
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DistributedLockTest()
        {
            Parallel.For(0, 10, j =>
            {
                Parallel.Invoke(DistributedLock);
            });
        }

        private void DistributedLock()
        {
            string key = Guid.NewGuid().ToString();
            RedisValue token = Environment.MachineName;
            if (stackRedis.db.LockTake(key, token, TimeSpan.FromMilliseconds(10)))
            {
                try
                {
                    stackRedis.StringSet(Guid.NewGuid().ToString(), Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                }
                finally
                {
                    stackRedis.db.LockRelease(key, token);
                }
            }
        }

        private class User
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
