using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;

namespace TBlog.Repository
{
    public class RedisRepository : IRedisRepository
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetValue(string key)
        {
            return await _database.StringGetAsync(key);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<TEntity> Get<TEntity>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public async Task Remove(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        public async Task Set(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                await _database.StringSetAsync(key, SerializeHelper.Serialize(value), cacheTime);
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second">秒数</param>
        public async Task Set(string key, object value, int second)
        {
            if (value != null)
            {
                await _database.StringSetAsync(key, SerializeHelper.Serialize(value), TimeSpan.FromSeconds(second));
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        public async Task Publish(string key, object value)
        {
            if (value != null)
            {
                await _database.ListLeftPushAsync(key, SerializeHelper.Serialize(value));
            }
        }

        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetValue(string key, byte[] value)
        {
            return await _database.StringSetAsync(key, value);
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
                    await _database.KeyDeleteAsync(key);
                }
            }
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Exist(string key)
        {
            return await _database.KeyExistsAsync(key);
        }
    }
}
