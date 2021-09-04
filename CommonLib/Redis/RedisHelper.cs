namespace RedisHelp
{
    using Newtonsoft.Json;

    using StackExchange.Redis;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class RedisHelper
    {
        /// <summary>
        /// 自定义缓存Key前缀
        /// </summary>
        /// <value>
        /// The custom key prefix.
        /// </value>
        private string CustomKeyPrefix { get; }

        /// <summary>
        /// ConnectionMultiplexer对象
        /// </summary>
        private readonly ConnectionMultiplexer conn;

        /// <summary>
        /// Redis数据库对象
        /// </summary>
        private readonly IDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisHelper"/> class.
        /// </summary>
        /// <param name="dbSerialNumber">The database serial number.</param>
        /// <param name="redisConnectionString">The redis connection string.</param>
        /// <param name="customKeyPrefix">The custom key prefix.</param>
        public RedisHelper(int dbSerialNumber, string redisConnectionString, string customKeyPrefix = null)
        {
            RedisConnectionMultiplexerHelper.RedisConnectionString = redisConnectionString;
            conn = RedisConnectionMultiplexerHelper.Instance;
            CustomKeyPrefix = customKeyPrefix;
            db = conn.GetDatabase(dbSerialNumber);
        }

        /// <summary>
        /// 保存单个k/v
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default)
        {
            key = GenerateCustomKey(key);
            return db.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 保存多个k/v
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns></returns>
        public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues = keyValues
                .Select(p => new KeyValuePair<RedisKey, RedisValue>(GenerateCustomKey(p.Key), p.Value))
                .ToList();
            return db.StringSet(newkeyValues.ToArray());
        }

        /// <summary>
        /// 以Json格式保存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="object">The object.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T @object, TimeSpan? expiry = default)
        {
            key = GenerateCustomKey(key);
            string json = ConvertToJson(@object);
            return db.StringSet(key, json, expiry);
        }

        /// <summary>
        /// 获取单个Key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = GenerateCustomKey(key);
            return db.StringGet(key);
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKeys">The list keys.</param>
        /// <returns></returns>
        public RedisValue[] StringGet(List<string> listKeys)
        {
            List<string> redisKeys = listKeys.Select(GenerateCustomKey).ToList();
            return db.StringGet(ConvertToRedisKeys(redisKeys));
        }

        /// <summary>
        /// 获取value转换为Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = GenerateCustomKey(key);
            return ConvertToObject<T>(db.StringGet(key));
        }

        /// <summary>
        /// Increment,如果key存在,则:value+=value
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="incrementVal">The increment value.</param>
        /// <returns></returns>
        public double StringIncrement(string key, double incrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return db.StringIncrement(key, incrementVal);
        }

        /// <summary>
        /// Decrement,如果key存在,则:value-=value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="decrementVal">The decrement value.</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double decrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return db.StringDecrement(key, decrementVal);
        }

        /// <summary>
        /// 判断hash中某个key是否已经被缓存
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hashFieldName">Name of the hash field.</param>
        /// <returns></returns>
        public bool HashExists(string key, string hashFieldName)
        {
            key = GenerateCustomKey(key);
            return db.HashExists(key, hashFieldName);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="hashFieldName">Name of the hash field.</param>
        /// <param name="hashValue">The hash value.</param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string hashFieldName, T hashValue)
        {
            key = GenerateCustomKey(key);
            string json = ConvertToJson(hashValue);
            return db.HashSet(key, hashFieldName, json);
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hashFieldName">Name of the hash field.</param>
        /// <returns></returns>
        public bool HashDelete(string key, string hashFieldName)
        {
            key = GenerateCustomKey(key);
            return db.HashDelete(key, hashFieldName);
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hashFieldNames">The hash field names.</param>
        /// <returns></returns>
        public long HashDelete(string key, List<RedisValue> hashFieldNames)
        {
            key = GenerateCustomKey(key);
            return db.HashDelete(key, hashFieldNames.ToArray());
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="hashFieldName">Name of the hash field.</param>
        /// <returns></returns>
        public T HashGet<T>(string key, string hashFieldName)
        {
            key = GenerateCustomKey(key);
            string value = db.HashGet(key, hashFieldName);
            return ConvertToObject<T>(value);
        }

        /// <summary>
        /// hash为数字增长value
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hashFieldName">Name of the hash field.</param>
        /// <param name="incrementVal">The increment value.</param>
        /// <returns></returns>
        public double HashIncrement(string key, string hashFieldName, double incrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return db.HashIncrement(key, hashFieldName, incrementVal);
        }

        /// <summary>
        /// hash为数字减少value
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hashFieldName">Name of the hash field.</param>
        /// <param name="decrementVal">The decrement value.</param>
        /// <returns></returns>
        public double HashDecrement(string key, string hashFieldName, double decrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return db.HashDecrement(key, hashFieldName, decrementVal);
        }

        /// <summary>
        /// 获取hashkey所有rediskey
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = GenerateCustomKey(key);
            RedisValue[] values = db.HashKeys(key);
            return ConvetToList<T>(values);
        }

        /// <summary>
        /// 移除指定List内部的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void ListRemove<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            db.ListRemove(key, ConvertToJson(value));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            key = GenerateCustomKey(key);
            RedisValue[] values = db.ListRange(key);
            return ConvetToList<T>(values);
        }

        /// <summary>
        /// 入队：列表从最右边插入一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public long ListRightPush<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            return db.ListRightPush(key, ConvertToJson(value));
        }

        /// <summary>
        /// 出队：列表从最右边获取一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            key = GenerateCustomKey(key);
            RedisValue value = db.ListRightPop(key);
            return ConvertToObject<T>(value);
        }

        /// <summary>
        /// 入栈：列表从最左边插入一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public long ListLeftPush<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            return db.ListLeftPush(key, ConvertToJson(value));
        }

        /// <summary>
        /// 出栈：列表从最左边获取一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            key = GenerateCustomKey(key);
            var value = db.ListLeftPop(key);
            return ConvertToObject<T>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = GenerateCustomKey(key);
            return db.ListLength(key);
        }

        /// <summary>
        /// Sorteds the set add.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="score">The score.</param>
        /// <returns></returns>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            key = GenerateCustomKey(key);
            return db.SortedSetAdd(key, ConvertToJson<T>(value), score);
        }

        /// <summary>
        /// Sorteds the set remove.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            return db.SortedSetRemove(key, ConvertToJson(value));
        }

        /// <summary>
        /// Sorteds the set range by rank.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="start">The start.</param>
        /// <param name="stop">The stop.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key, long start, long stop, Order order = Order.Ascending)
        {
            key = GenerateCustomKey(key);
            var values = db.SortedSetRangeByRank(key, start, stop, order);
            return ConvetToList<T>(values);
        }

        /// <summary>
        /// Sorteds the length of the set.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = GenerateCustomKey(key);
            return db.SortedSetLength(key);
        }

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool KeyDelete(string key)
        {
            key = GenerateCustomKey(key);
            return db.KeyDelete(key);
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public long KeyDelete(List<string> keys)
        {
            List<string> newKeys = keys.Select(GenerateCustomKey).ToList();
            return db.KeyDelete(ConvertToRedisKeys(newKeys));
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = GenerateCustomKey(key);
            return db.KeyExists(key);
        }

        /// <summary>
        /// 重命名key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newKey">The new key.</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = GenerateCustomKey(key);
            return db.KeyRename(key, newKey);
        }

        /// <summary>
        /// 设置Key过期时间
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="expiry">The expiry.</param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default)
        {
            key = GenerateCustomKey(key);
            return db.KeyExpire(key, expiry);
        }

        /// <summary>
        /// Redis订阅
        /// </summary>
        /// <param name="subChannel">The sub channel.</param>
        /// <param name="handler">The handler.</param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = conn.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }

        /// <summary>
        /// Redis发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public long Publish<T>(string channel, T message)
        {
            ISubscriber sub = conn.GetSubscriber();
            return sub.Publish(channel, ConvertToJson(message));
        }

        /// <summary>
        /// Redis取消订阅
        /// </summary>
        /// <param name="channel">The channel.</param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = conn.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = conn.GetSubscriber();
            sub.UnsubscribeAll();
        }

        public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default)
        {
            key = GenerateCustomKey(key);
            return await db.StringSetAsync(key, value, expiry);
        }

        public async Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues = keyValues
                .Select(p => new KeyValuePair<RedisKey, RedisValue>(GenerateCustomKey(p.Key), p.Value))
                .ToList();
            return await db.StringSetAsync(newkeyValues.ToArray());
        }

        public async Task<bool> StringSetAsync<T>(string key, T @object, TimeSpan? expiry = default)
        {
            key = GenerateCustomKey(key);
            string json = ConvertToJson(@object);
            return await db.StringSetAsync(key, json, expiry);
        }

        public async Task<string> StringGetAsync(string key)
        {
            key = GenerateCustomKey(key);
            return await db.StringGetAsync(key);
        }

        public async Task<RedisValue[]> StringGetAsync(List<string> listKeys)
        {
            List<string> redisKeys = listKeys.Select(GenerateCustomKey).ToList();
            return await db.StringGetAsync(ConvertToRedisKeys(redisKeys));
        }

        public async Task<T> StringGetAsync<T>(string key)
        {
            key = GenerateCustomKey(key);
            return ConvertToObject<T>(await db.StringGetAsync(key));
        }

        public async Task<double> StringIncrementAsync(string key, double incrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return await db.StringIncrementAsync(key, incrementVal);
        }

        public async Task<double> StringDecrementAsync(string key, double decrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return await db.StringDecrementAsync(key, decrementVal);
        }

        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = GenerateCustomKey(key);
            return await db.HashExistsAsync(key, dataKey);
        }

        public async Task<bool> HashSetAsync<T>(string key, string hashFieldName, T hashValue)
        {
            key = GenerateCustomKey(key);
            string json = ConvertToJson(hashValue);
            return await db.HashSetAsync(key, hashFieldName, json);
        }

        public async Task<bool> HashDeleteAsync(string key, string hashFieldName)
        {
            key = GenerateCustomKey(key);
            return await db.HashDeleteAsync(key, hashFieldName);
        }

        public async Task<long> HashDeleteAsync(string key, List<RedisValue> hashFieldNames)
        {
            key = GenerateCustomKey(key);
            return await db.HashDeleteAsync(key, hashFieldNames.ToArray());
        }

        public async Task<T> HashGetAsync<T>(string key, string hashFieldName)
        {
            key = GenerateCustomKey(key);
            string value = await db.HashGetAsync(key, hashFieldName);
            return ConvertToObject<T>(value);
        }

        public async Task<double> HashIncrementAsync(string key, string hashFieldName, double incrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return await db.HashIncrementAsync(key, hashFieldName, incrementVal);
        }

        public async Task<double> HashDecrementAsync(string key, string hashFieldName, double decrementVal = 1)
        {
            key = GenerateCustomKey(key);
            return await db.HashDecrementAsync(key, hashFieldName, decrementVal);
        }

        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            key = GenerateCustomKey(key);
            RedisValue[] values = await db.HashKeysAsync(key);
            return ConvetToList<T>(values);
        }

        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            return await db.ListRemoveAsync(key, ConvertToJson(value));
        }

        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            key = GenerateCustomKey(key);
            RedisValue[] values = await db.ListRangeAsync(key);
            return ConvetToList<T>(values);
        }

        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            return await db.ListRightPushAsync(key, ConvertToJson(value));
        }

        public async Task<T> ListRightPopAsync<T>(string key)
        {
            key = GenerateCustomKey(key);
            RedisValue value = await db.ListRightPopAsync(key);
            return ConvertToObject<T>(value);
        }

        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            return await db.ListLeftPushAsync(key, ConvertToJson(value));
        }

        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            key = GenerateCustomKey(key);
            var value = await db.ListLeftPopAsync(key);
            return ConvertToObject<T>(value);
        }

        public async Task<long> ListLengthAsync(string key)
        {
            key = GenerateCustomKey(key);
            return await db.ListLengthAsync(key);
        }

        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = GenerateCustomKey(key);
            return await db.SortedSetAddAsync(key, ConvertToJson<T>(value), score);
        }

        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = GenerateCustomKey(key);
            return await db.SortedSetRemoveAsync(key, ConvertToJson(value));
        }

        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start, long stop, Order order = Order.Ascending)
        {
            key = GenerateCustomKey(key);
            var values = await db.SortedSetRangeByRankAsync(key, start, stop, order);
            return ConvetToList<T>(values);
        }

        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = GenerateCustomKey(key);
            return await db.SortedSetLengthAsync(key);
        }

        public async Task<bool> KeyDeleteAsync(string key)
        {
            key = GenerateCustomKey(key);
            return await db.KeyDeleteAsync(key);
        }

        public async Task<long> KeyDeleteAsync(List<string> keys)
        {
            List<string> newKeys = keys.Select(GenerateCustomKey).ToList();
            return await db.KeyDeleteAsync(ConvertToRedisKeys(newKeys));
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            key = GenerateCustomKey(key);
            return await db.KeyExistsAsync(key);
        }

        public async Task<bool> KeyRenameAsync(string key, string newKey)
        {
            key = GenerateCustomKey(key);
            return await db.KeyRenameAsync(key, newKey);
        }

        public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry = default)
        {
            key = GenerateCustomKey(key);
            return await db.KeyExpireAsync(key, expiry);
        }

        public async Task SubscribeAsync(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = conn.GetSubscriber();
            await sub.SubscribeAsync(subChannel, (channel, message) =>
             {
                 if (handler == null)
                 {
                     Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                 }
                 else
                 {
                     handler(channel, message);
                 }
             });
        }

        public async Task<long> PublishAsync<T>(string channel, T message)
        {
            ISubscriber sub = conn.GetSubscriber();
            return await sub.PublishAsync(channel, ConvertToJson(message));
        }

        public async Task UnsubscribeAsync(string channel)
        {
            ISubscriber sub = conn.GetSubscriber();
            await sub.UnsubscribeAsync(channel);
        }

        public async Task UnsubscribeAllAsync()
        {
            ISubscriber sub = conn.GetSubscriber();
            await sub.UnsubscribeAllAsync();
        }

        /// <summary>
        /// 生成自定义Key,格式(CustomKeyPrefix+"_"+Key)
        /// </summary>
        /// <param name="oldKey"></param>
        /// <returns></returns>
        private string GenerateCustomKey(string oldKey)
        {
            if (!string.IsNullOrWhiteSpace(CustomKeyPrefix))
            {
                return $"{CustomKeyPrefix}_{oldKey}";
            }
            return oldKey;
        }

        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertToJson<T>(T value)
        {
            return value is string ? value.ToString() : JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// RedisValue转Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T ConvertToObject<T>(RedisValue value)
        {
            if (typeof(T).Name.Equals(typeof(string).Name))
            {
                return JsonConvert.DeserializeObject<T>($"'{value}'");
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// RedisValue转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private static List<T> ConvetToList<T>(RedisValue[] values)
        {
            List<T> result = new();
            foreach (RedisValue item in values)
            {
                T @object = ConvertToObject<T>(item);
                result.Add(@object);
            }
            return result;
        }

        /// <summary>
        /// 转换为RedisKey
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        private static RedisKey[] ConvertToRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }
    }
}