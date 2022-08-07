using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBlog.Redis
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisRepository
    {
        Task<string> GetValue(string key);
        Task<TEntity> Get<TEntity>(string key);
        Task Set(string key, object value, TimeSpan cacheTime);
        Task Set(string key, object value, int second);
        Task<bool> Exist(string key);
        Task Remove(string key);
        Task Clear();
    }
}
