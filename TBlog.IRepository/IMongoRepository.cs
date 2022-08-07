using MongoDB.Driver;

namespace TBlog.IRepository
{
    public interface IMongoRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        IMongoCollection<TEntity> Collection { get; set; }
    }
}
