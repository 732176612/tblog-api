using StackExchange.Redis;
namespace TBlog.Repository
{
    public class RedisRepository : IRedisRepository
    {
        private readonly ConnectionMultiplexer _redis;
        public IDatabase _db { get; set; }

        public RedisRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        public async Task<bool> Exist(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        /// <summary>
        /// 查询
        /// </summary>
        public async Task<string> GetValue(string key)
        {
            return await _db.StringGetAsync(key);
        }

        /// <summary>
        /// 获取
        /// </summary>
        public async Task<TEntity> Get<TEntity>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.HasValue)
            {
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            return default;

        }

        /// <summary>
        /// 移除
        /// </summary>
        public async Task Remove(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 设置
        /// </summary>
        public async Task Set(string key, object value, TimeSpan cacheTime)
        {
            await _db.StringSetAsync(key, SerializeHelper.Serialize(value), cacheTime);
        }

        /// <summary>
        /// 设置
        /// </summary>
        public async Task Set(string key, object value, int second)
        {
            await _db.StringSetAsync(key, SerializeHelper.Serialize(value), TimeSpan.FromSeconds(second));
        }

        /// <summary>
        /// 设置
        /// </summary>
        public async Task<long> ListLeftPush(string key, object value)
        {
            return await _db.ListLeftPushAsync(key, SerializeHelper.Serialize(value));
        }

        /// <summary>
        /// 清除
        /// </summary>
        public async Task Clear()
        {
            foreach (var endPoint in _redis.GetEndPoints())
            {
                var server = _redis.GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    await _db.KeyDeleteAsync(key);
                }
            }
        }
    }
}
