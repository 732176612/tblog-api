global using System;
global using System.Collections.Generic;
global using System.Linq;
global using TBlog.IService;
global using TBlog.Model;
global using TBlog.IRepository;
global using System.Threading.Tasks;
global using System.Linq.Expressions;
global using TBlog.Extensions;
global using TBlog.Common;
global using Nest;
namespace TBlog.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        public IBaseRepository<TEntity> baseRepository { get; set; }

        public BaseService(IBaseRepository<TEntity> _baseRepository)
        {
            baseRepository = _baseRepository;
        }

        public Task AddEntity(TEntity entity)
        {
            return baseRepository.AddEntity(entity);
        }

        public Task AddEntities(List<TEntity> entities)
        {
            return baseRepository.AddEntities(entities);
        }

        public async Task<bool> Delete(TEntity entity)
        {
            return await baseRepository.Delete(entity);
        }

        public async Task<bool> DeleteById(object id)
        {
            return await baseRepository.DeleteById(id);
        }

        public async Task<bool> DeleteByIds(object[] ids)
        {
            return await baseRepository.DeleteByIds(ids);
        }

        public async Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await baseRepository.Get(whereExpression);
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await baseRepository.GetAll();
        }

        public async Task<TEntity> GetById(object id)
        {
            return await baseRepository.GetById(id);
        }

        public async Task<List<TEntity>> GetByIds(List<object> ids)
        {
            return await baseRepository.GetByIds(ids);
        }

        public async Task<PageModel<TEntity>> GetPage(int PageIndex = 1, int PageSize = 20, Expression<Func<TEntity, bool>> filter = null, Dictionary<Expression<Func<TEntity, object>>, bool> sorts = null)
        {
            return await baseRepository.GetPage(PageIndex, PageSize, filter, sorts);
        }

        public async Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await baseRepository.GetSingle(whereExpression);
        }

        public async Task<bool> Update(TEntity entity)
        {
            return await baseRepository.Update(entity);
        }

        public Task Update(List<TEntity> entities)
        {
            return baseRepository.Update(entities);
        }

        public async Task<long> Delete(Expression<Func<TEntity, bool>> filter)
        {
            return await baseRepository.Delete(filter);
        }
    }
}
