


namespace CodeSnippet.Redis
{

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using CommonLib;

    using StackExchange.Redis;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    [TestCategory("Redis")]
    [TestClass()]
    public class RedisClientTests
    {
        private const string redisConnection = "192.168.31.143:6379";
        private readonly RedisHelper redis = new(0, redisConnection);

        [TestMethod]
        public void SetString()
        {
            bool result = redis.StringSet($"{Guid.NewGuid()}", $"{nameof(SetString)}");
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
            bool result = redis.StringSet(keyValues);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SetObject()
        {
            bool result = redis.StringSet("user", new
            {
                name = "wangpengliang",
                age = 25,
                address = "BEIJING"
            });
            Assert.IsTrue(result);

            bool result1 = redis.StringSet("user1", 100);
            Assert.IsTrue(result1);

            bool result2 = redis.StringSet("user2", "100");
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void GetString()
        {
            string key = "name";
            string value = "wangpengliang";
            redis.StringSet(key, value);
            string result = redis.StringGet(key);
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
            redis.StringSet(keyValues);

            RedisValue[] result = redis.StringGet(listKeys);

            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(result[i], redisValues[i]);
            }
        }

        [TestMethod]
        public void StringIncrement()
        {
            string key = Guid.NewGuid().ToString();
            double result = redis.StringIncrement(key);
            Assert.AreEqual(1, result);
            result = redis.StringIncrement(key, 10);
            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void StringDecrement()
        {
            string key = Guid.NewGuid().ToString();
            double result = redis.StringDecrement(key);
            Assert.AreEqual(-1, result);
            result = redis.StringDecrement(key);
            Assert.AreEqual(-2, result);
        }

        [TestMethod]
        public void HashSet()
        {
            string key = Guid.NewGuid().ToString();
            bool result = redis.HashSet(key, "name", "wangpengliang");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HashExists()
        {
            string key = Guid.NewGuid().ToString();
            redis.HashSet(key, "name", "wangpengliang");
            Assert.IsTrue(redis.HashExists(key, "name"));
            Assert.IsFalse(redis.HashExists(key, "age"));
        }

        [TestMethod]
        public void HashDelete()
        {
            string key = Guid.NewGuid().ToString();
            redis.HashSet(key, "name", "wangpengliang");
            var result = redis.HashDelete(key, "name");
            Assert.IsTrue(result);
            Assert.IsFalse(redis.HashExists(key, "name"));
        }

        [TestMethod]
        public void HashDeleteList()
        {
            string key = Guid.NewGuid().ToString();
            redis.HashSet(key, "name", "wangpengliang");
            redis.HashSet(key, "age", "25");

            List<RedisValue> hashFieldNames = new List<RedisValue>
            {
                new RedisValue("name"),
                new RedisValue("age")
            };

            redis.HashDelete(key, hashFieldNames);

            Assert.IsFalse(redis.HashExists(key, "name"));
            Assert.IsFalse(redis.HashExists(key, "age"));
        }

        [TestMethod]
        public void HashGet()
        {
            string key = Guid.NewGuid().ToString();
            redis.HashSet(key, "name", "wangpengliang");
            redis.HashSet(key, "age", "25");

            var result = redis.HashGet<string>(key, "name");
            Assert.AreEqual("wangpengliang", result);

            key = Guid.NewGuid().ToString();
            var person = new User
            {
                Name = "wangpengliang",
                Age = 25
            };
            redis.HashSet(key, "user", person);

            var result2 = redis.HashGet<User>(key, "user");
            Assert.AreEqual("wangpengliang", result2.Name);
        }

        [TestMethod]
        public void HashIncrement()
        {
            string key = Guid.NewGuid().ToString();
            redis.HashSet(key, "name", "wangpengliang");
            redis.HashSet(key, "age", "25");

            redis.HashIncrement(key, "age");
            Assert.AreEqual("26", redis.HashGet<string>(key, "age"));
        }

        [TestMethod]
        public void HashDecrement()
        {
            string key = Guid.NewGuid().ToString();
            redis.HashSet(key, "name", "wangpengliang");
            redis.HashSet(key, "age", "25");

            redis.HashDecrement(key, "age");
            Assert.AreEqual("24", redis.HashGet<string>(key, "age"));
        }

        [TestMethod]
        public void HashKeys()
        {
            string key = Guid.NewGuid().ToString();
            redis.HashSet(key, "name", "wangpengliang");
            redis.HashSet(key, "age", "25");

            var result = redis.HashKeys<string>(key);
            Assert.IsTrue(result.Count == 2);
        }

        /// <summary>
        /// Queue
        /// </summary>
        [TestMethod]
        public void ListQueue()
        {
            string key = Guid.NewGuid().ToString();
            redis.ListRightPush(key, "1");
            redis.ListRightPush(key, "2");
            redis.ListRightPush(key, "3");

            var result1 = redis.ListRightPop<string>(key);
            Assert.AreEqual("3", result1);

            var result2 = redis.ListLeftPop<string>(key);
            Assert.AreEqual("1", result2);
        }

        [TestMethod]
        public void ListStack()
        {
            string key = Guid.NewGuid().ToString();
            redis.ListLeftPush(key, "3");
            redis.ListLeftPush(key, "2");
            redis.ListLeftPush(key, "1");

            var result1 = redis.ListRightPop<string>(key);
            Assert.AreEqual("3", result1);

            var result2 = redis.ListLeftPop<string>(key);
            Assert.AreEqual("1", result2);
        }

        [TestMethod]
        public void ListLength()
        {
            string key = Guid.NewGuid().ToString();
            redis.ListLeftPush(key, "3");
            redis.ListLeftPush(key, "2");
            redis.ListLeftPush(key, "1");

            var result = redis.ListLength(key);
            Assert.AreEqual(3, result);

            redis.ListLeftPop<string>(key);
            result = redis.ListLength(key);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void SortedSetAdd()
        {
            string key = Guid.NewGuid().ToString();
            redis.SortedSetAdd(key, "wang", 10);
            redis.SortedSetAdd(key, "li", 11);
            redis.SortedSetAdd(key, "zhou", 12);
            redis.SortedSetAdd(key, "zhang", 4);

            var result = redis.SortedSetLength(key);
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void SortedSetRemove()
        {
            string key = Guid.NewGuid().ToString();
            redis.SortedSetAdd(key, "wang", 10);
            redis.SortedSetAdd(key, "li", 11);
            redis.SortedSetAdd(key, "zhou", 12);
            redis.SortedSetAdd(key, "zhang", 4);

            redis.SortedSetRemove(key, "zhou");

            var result = redis.SortedSetLength(key);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void SortedSetRangeByRank()
        {
            string key = Guid.NewGuid().ToString();
            redis.SortedSetAdd(key, "wang", 10);
            redis.SortedSetAdd(key, "li", 11);
            redis.SortedSetAdd(key, "zhou", 12);
            redis.SortedSetAdd(key, "zhang", 4);
            var result = redis.SortedSetRangeByRank<string>(key, 0, 2);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void KeyDelete()
        {
            string key = Guid.NewGuid().ToString();
            redis.StringSet(key, $"{nameof(KeyDelete)}");

            redis.KeyDelete(key);
            var result = redis.StringGet(key);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void KeyDeleteList()
        {
            string key1 = Guid.NewGuid().ToString();
            string key2 = Guid.NewGuid().ToString();
            redis.StringSet(key1, $"{nameof(KeyDeleteList)}");
            redis.StringSet(key2, $"{nameof(KeyDeleteList)}");

            redis.KeyDelete(new List<string> { key1, key2 });
            var result = redis.StringGet(new List<string> { key1, key2 });
            foreach (var item in result)
            {
                Assert.IsTrue(item.IsNull);
            }
        }


        [TestMethod]
        public void KeyExists()
        {
            string key = Guid.NewGuid().ToString();
            redis.StringSet(key, $"{nameof(KeyExists)}");
            Assert.IsTrue(redis.KeyExists(key));

            redis.KeyDelete(key);
            Assert.IsFalse(redis.KeyExists(key));
        }

        [TestMethod]
        public void KeyRename()
        {
            string key = Guid.NewGuid().ToString();
            string value = "wangpengliang";
            redis.StringSet(key, value);
            string newKey = Guid.NewGuid().ToString();
            redis.KeyRename(key, newKey);
            var result = redis.StringGet(newKey);

            Assert.AreEqual("wangpengliang", result);
        }

        [TestMethod]
        public void KeyExpire()
        {
            string key = Guid.NewGuid().ToString();
            string value = "wangpengliang";
            redis.StringSet(key, value);
            var result = redis.StringGet(key);

            redis.KeyExpire(key, TimeSpan.FromSeconds(3));
            result = redis.StringGet(key);
            Assert.AreEqual("wangpengliang", result);

            Thread.Sleep(3000);

            result = redis.StringGet(key);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void CreateTransaction()
        {
            var tran = redis.CreateTransaction(0);
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
            if (redis.db.LockTake(key, token, TimeSpan.FromMilliseconds(10)))
            {
                try
                {
                    redis.StringSet(Guid.NewGuid().ToString(), Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                }
                finally
                {
                    redis.db.LockRelease(key, token);
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
