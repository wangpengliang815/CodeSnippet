namespace CodeSnippet.Redis
{

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RedisHelp;

    using StackExchange.Redis;

    using System.Collections.Generic;

    [TestCategory("Redis")]
    [TestClass()]
    public class RedisClientTests
    {
        private const string redisConnection = "192.168.31.143:6379";
        private readonly RedisHelper redis = new(0, "test", redisConnection);

        [TestMethod]
        public void SetString()
        {
            for (int i = 0; i < 5; i++)
            {
                bool result = redis.SetString($"name_{i}", nameof(SetString));
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void SetStringList()
        {
            List<KeyValuePair<RedisKey, RedisValue>> keyValues = new();
            for (int i = 0; i < 5; i++)
            {
                keyValues.Add(new KeyValuePair<RedisKey, RedisValue>($"name_{i}", nameof(SetStringList)));
            }
            bool result = redis.SetString(keyValues);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SetObject()
        {
            bool result = redis.SetString("user", new
            {
                name = "wangpengliang",
                age = 25,
                address = "BEIJING"
            });
            Assert.IsTrue(result);

            bool result1 = redis.SetString("user1", 100);
            Assert.IsTrue(result1);

            bool result2 = redis.SetString("user2", "100");
            Assert.IsTrue(result2);
        }

        [TestMethod]
        public void GetString()
        {
            string key = "name";
            string value = "wangpengliang";
            redis.SetString(key, value);
            string result = redis.GetString(key);
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
            redis.SetString(keyValues);

            RedisValue[] result = redis.GetString(listKeys);

            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(result[i], redisValues[i]);
            }
        }
    }
}
