using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 根据Where表达式树查询实体
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 根据Where表达式树查询实体
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 查询集合所有实体
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> GetAll();

        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetById(object id);

        /// <summary>
        /// 根据id数组查询实体list
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetByIds(IEnumerable<object> ids);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="sorts"></param>
        /// <returns></returns>
        Task<PageModel<TEntity>> GetPage(int PageIndex = 1, int PageSize = 20, Expression<Func<TEntity, bool>> filter = null, Dictionary<Expression<Func<TEntity, object>>, bool> sorts = null);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddEntity(TEntity entity);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddEntities(List<TEntity> entities);

        /// <summary>
        /// 更新model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity entity);

        /// <summary>
        /// 更新model
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task Update(List<TEntity> entities);

        /// <summary>
        /// 根据对象，删除某一实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Delete(TEntity entity);

        /// <summary>
        /// 根据表达式删除实体
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<long> Delete(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 根据id 删除某一实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteById(object id);

        /// <summary>
        /// 根据id数组，删除实体list
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteByIds(object[] ids);

        /// <summary>
        /// 计数
        /// </summary>
        /// <returns></returns>
        Task<long> Count();

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<long> Count(Expression<Func<TEntity, bool>> filter);
    }
}
