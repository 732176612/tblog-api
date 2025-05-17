using System.Linq.Expressions;

namespace TBlog.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 添加
        /// </summary>
        Task<int> AddEntity(TEntity entity);

        /// <summary>
        /// 批量添加
        /// </summary>
        Task<int> AddEntities(List<TEntity> entities);

        /// <summary>
        /// 更新
        /// </summary>
        Task<bool> Update(TEntity entity);

        /// <summary>
        /// 批量更新
        /// </summary>
        Task<bool> Update(List<TEntity> entities);

        /// <summary>
        /// 删除
        /// </summary>
        Task<long> Delete(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 删除
        /// </summary>
        Task<long> DeleteByIds(params object[] entityIds);
    }
}
