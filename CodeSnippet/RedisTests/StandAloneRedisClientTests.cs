namespace CodeSnippet.Redis
{
    using CommonLib;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using StackExchange.Redis;

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// 单机部署的Redis测试
    /// </summary>
    [TestCategory("RedisStanndAlone")]
    [TestClass()]
    public class StandAloneRedisClientTests
    {
        private static ServiceCollection serviceCollection;
        private static IServiceProvider serviceProvider;
        private const string redisConnection = "192.168.189.143:6379";
        private static StackExchangeRedisHelper stackRedis;
        //private readonly StackExchangeRedisHelper stackRedis = new(0, redisConnection);

        [TestInitialize]
        public void MethodInit()
        {
            serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(p => new StackExchangeRedisHelper(0, redisConnection));
            serviceProvider = serviceCollection.BuildServiceProvider();
            stackRedis = serviceProvider
               .GetService<StackExchangeRedisHelper>();
            GCHandle h = GCHandle.Alloc(stackRedis, GCHandleType.WeakTrackResurrection);
            IntPtr addr = GCHandle.ToIntPtr(h);
            Console.WriteLine("0x" + addr.ToString("X"));
        }

        [TestMethod]
        public void SetString()
        {
            bool result = stackRedis.StringSet($"{Guid.NewGuid()}", $"{nameof(SetString)}");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SetStringList()
        {
            List<KeyValuePair<RedisKey, RedisValue>> keyValues = new();
            for (int i = 0; i < 5; i++)
            {
                keyValues.Add(new KeyValuePair<RedisKey, RedisValue>($"{Guid.NewGuid()}", nameof(SetStringList)));
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
            var result = stackRedis.HashDelete(key, "name");
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

            var result = stackRedis.HashGet<string>(key, "name");
            Assert.AreEqual("wangpengliang", result);

            key = Guid.NewGuid().ToString();
            var person = new User
            {
                Name = "wangpengliang",
                Age = 25
            };
            stackRedis.HashSet(key, "user", person);

            var result2 = stackRedis.HashGet<User>(key, "user");
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

            var result = stackRedis.HashKeys<string>(key);
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

            var result1 = stackRedis.ListRightPop<string>(key);
            Assert.AreEqual("3", result1);

            var result2 = stackRedis.ListLeftPop<string>(key);
            Assert.AreEqual("1", result2);
        }

        [TestMethod]
        public void ListStack()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.ListLeftPush(key, "3");
            stackRedis.ListLeftPush(key, "2");
            stackRedis.ListLeftPush(key, "1");

            var result1 = stackRedis.ListRightPop<string>(key);
            Assert.AreEqual("3", result1);

            var result2 = stackRedis.ListLeftPop<string>(key);
            Assert.AreEqual("1", result2);
        }

        [TestMethod]
        public void ListLength()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.ListLeftPush(key, "3");
            stackRedis.ListLeftPush(key, "2");
            stackRedis.ListLeftPush(key, "1");

            var result = stackRedis.ListLength(key);
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

            var result = stackRedis.SortedSetLength(key);
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

            var result = stackRedis.SortedSetLength(key);
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
            var result = stackRedis.SortedSetRangeByRank<string>(key, 0, 2);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void KeyDelete()
        {
            string key = Guid.NewGuid().ToString();
            stackRedis.StringSet(key, $"{nameof(KeyDelete)}");

            stackRedis.KeyDelete(key);
            var result = stackRedis.StringGet(key);
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
            var result = stackRedis.StringGet(new List<string> { key1, key2 });
            foreach (var item in result)
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
            var result = stackRedis.StringGet(newKey);

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
            var result = stackRedis.StringGet(key);
            Assert.AreEqual("wangpengliang", result);

            Thread.Sleep(3000);

            result = stackRedis.StringGet(key);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void CreateTransaction()
        {
            var tran = stackRedis.CreateTransaction(0);
            tran.StringSetAsync("name", "wangpengliang");
            tran.StringSetAsync("name1", "wangkaining");
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
