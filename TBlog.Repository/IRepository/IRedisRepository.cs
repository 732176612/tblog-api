using StackExchange.Redis;

namespace TBlog.IRepository
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisRepository
    {
        IDatabase _db { get; }
        Task<string> GetValue(string key);
        Task<TEntity> Get<TEntity>(string key);
        Task Set(string key, object value, TimeSpan cacheTime);
        Task Set(string key, object value, int second);
        Task<long> ListLeftPush(string key, object value);
        Task<bool> Exist(string key);
        Task Remove(string key);
        Task Clear();
    }
}
