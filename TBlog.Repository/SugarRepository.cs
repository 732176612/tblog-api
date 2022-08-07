using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBlog.IRepository;
using TBlog.Model;
using TBlog.Common;
using System.Linq;
using System.Linq.Expressions;
using IdGen;
namespace TBlog.Repository
{
    public class SugarRepository<TEntity> : ISugarRepository<TEntity> where TEntity : class, IEntity, new()
    {
        public ISqlSugarClient BaseDB { get; set; }

        public SugarRepository(ISqlSugarTransaction tranProcess)
        {
            BaseDB = tranProcess.GetDbClient();
        }

        #region 查询
        public Expression<Func<TEntity, bool>> GetBaseFilter()
        {
            return FilterHelper.CreateExp<TEntity>(c => c.IsDeleted == false);
        }

        public async Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter)
        {
            filter = GetBaseFilter().AddExp(filter);
            return await BaseDB.Queryable<TEntity>().WhereIF(filter != null, filter).ToListAsync();
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await BaseDB.Queryable<TEntity>().Where(GetBaseFilter()).ToListAsync();
        }

        public async Task<TEntity> GetById(object id)
        {
            return await BaseDB.Queryable<TEntity>().InSingleAsync(id);
        }

        public async Task<List<TEntity>> GetByIds(IEnumerable<object> ids)
        {
            return await BaseDB.Queryable<TEntity>().In(ids).ToListAsync();
        }

        public async Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> filter)
        {
            filter = GetBaseFilter().AddExp(filter);
            return await BaseDB.Queryable<TEntity>().WhereIF(filter != null, filter).SingleAsync();
        }

        public async Task<PageModel<TEntity>> GetPage(int PageIndex = 1, int PageSize = 20, Expression<Func<TEntity, bool>> filter = null, Dictionary<Expression<Func<TEntity, object>>, bool> sorts = null)
        {

            RefAsync<int> totalCount = 0;
            filter = GetBaseFilter().AddExp(filter);
            var queryable = BaseDB.Queryable<TEntity>().WhereIF(filter != null, filter);
            foreach (var sort in sorts)
            {
                if (sort.Value)
                {
                    queryable.OrderBy(sort.Key, OrderByType.Asc);
                }
                else
                {
                    queryable.OrderBy(sort.Key, OrderByType.Desc);
                }
            }
            var list = await queryable.ToPageListAsync(PageIndex, PageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.Value / (decimal)PageSize)).ToInt();
            return new PageModel<TEntity>() { TotalCount = totalCount, PageCount = pageCount, PageIndex = PageIndex, PageSize = PageSize, Data = list };
        }
        #endregion

        #region 插入

        public async Task AddEntity(TEntity entity)
        {
            entity.CDate = DateTime.UtcNow;
            entity.MDate = DateTime.UtcNow;
            await BaseDB.Insertable(entity).ExecuteCommandAsync();
        }

        public async Task AddEntities(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.CDate = DateTime.UtcNow;
                entity.MDate = DateTime.UtcNow;
            });
            await BaseDB.Insertable(entities).ExecuteCommandAsync();
        }
        #endregion

        #region 更新
        public async Task<bool> Update(TEntity entity)
        {
            entity.MDate = DateTime.UtcNow;
            return await BaseDB.Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        public Task Update(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.MDate = DateTime.UtcNow;
            });
            return BaseDB.Updateable(entities).ExecuteCommandHasChangeAsync();
        }
        #endregion

        #region 删除
        public async Task<bool> Delete(TEntity entity)
        {
            return await BaseDB.Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        public async Task<long> Delete(Expression<Func<TEntity, bool>> filter)
        {
            return await BaseDB.Deleteable(filter).ExecuteCommandAsync();
        }

        public async Task<bool> DeleteById(object id)
        {
            return await BaseDB.Deleteable<TEntity>(id).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> DeleteByIds(object[] ids)
        {
            return await BaseDB.Deleteable<TEntity>().In(ids).ExecuteCommandHasChangeAsync();
        }
        #endregion

        #region 计数
        public async Task<long> Count()
        {
            return await BaseDB.Queryable<TEntity>().CountAsync();
        }

        public async Task<long> Count(Expression<Func<TEntity, bool>> filter)
        {
            return await BaseDB.Queryable<TEntity>().WhereIF(filter != null, filter).CountAsync();
        }
        #endregion
    }
}
