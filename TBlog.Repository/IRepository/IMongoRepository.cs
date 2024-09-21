using MongoDB.Driver;
using System.Linq.Expressions;

namespace TBlog.IRepository
{
    public interface IMongoRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        IMongoCollection<TEntity> Collection { get; set; }

        /// <summary>
        /// 根据Where表达式树查询实体
        /// </summary>
        Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 根据Where表达式树查询实体
        /// </summary>
        Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 查询集合所有实体
        /// </summary>
        Task<List<TEntity>> GetAll();

        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        Task<TEntity> GetById(object id);

        /// <summary>
        /// 根据id数组查询实体list
        /// </summary>
        Task<List<TEntity>> GetByIds(IEnumerable<object> ids);

        /// <summary>
        /// 分页查询
        /// </summary>
        Task<PageModel<TEntity>> GetPage(int PageIndex = 1, int PageSize = 20, Expression<Func<TEntity, bool>> filter = null, Dictionary<Expression<Func<TEntity, object>>, bool> sorts = null);

        /// <summary>
        /// 根据对象，删除某一实体
        /// </summary>
        Task<bool> Delete(TEntity entity);

        /// <summary>
        /// 根据表达式删除实体
        /// </summary>
        Task<long> Delete(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 根据id 删除某一实体
        /// </summary>
        Task<bool> DeleteById(object id);

        /// <summary>
        /// 根据id数组，删除实体list
        /// </summary>
        Task<bool> DeleteByIds(object[] ids);

        /// <summary>
        /// 计数
        /// </summary>
        Task<long> Count();

        /// <summary>
        /// 计数
        /// </summary>
        Task<long> Count(Expression<Func<TEntity, bool>> filter);
    }
}
