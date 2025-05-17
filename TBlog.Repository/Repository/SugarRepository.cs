using System.Linq.Expressions;

namespace TBlog.Repository
{
    public class SugarRepository<TEntity> : ISugarRepository<TEntity> where TEntity : class, IEntity, new()
    {
        public ISugarQueryable<TEntity> DBQuery { get => DbScoped.SugarScope.Queryable<TEntity>(); }

        public IUpdateable<TEntity> DBUpdate { get => DbScoped.SugarScope.Updateable<TEntity>(); }

        public IDeleteable<TEntity> DBDelete { get => DbScoped.SugarScope.Deleteable<TEntity>(); }


        #region 插入
        public Task<int> AddEntity(TEntity entity)
        {
            entity.CDate = DateTime.UtcNow;
            entity.MDate = DateTime.UtcNow;
            return DbScoped.SugarScope.Insertable(entity).ExecuteCommandAsync();
        }

        public Task<int> AddEntities(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.CDate = DateTime.UtcNow;
                entity.MDate = DateTime.UtcNow;
            });
            return DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
        }
        #endregion

        #region 更新
        public Task<bool> Update(TEntity entity)
        {
            entity.MDate = DateTime.Now;
            return DbScoped.SugarScope.Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        public Task<bool> Update(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.MDate = DateTime.UtcNow;
            });
            return DbScoped.SugarScope.Updateable(entities).ExecuteCommandHasChangeAsync();
        }

        public async Task<long> Delete(Expression<Func<TEntity, bool>> filter)
        {
            var deleteCount = 0;
            if (typeof(TEntity) is IDeleteEntity)
            {
                deleteCount = await DBUpdate.SetColumns("IsDeleted", true).Where(filter).ExecuteCommandAsync();
            }
            deleteCount =await DBDelete.Where(filter).ExecuteCommandAsync();
            return deleteCount;
        }

        public async Task<long> DeleteByIds(params object[] entityIds)
        {
            var deleteCount = 0;
            if (typeof(TEntity) is IDeleteEntity)
            {
                deleteCount = await DBUpdate.SetColumns("IsDeleted", true).In(entityIds).ExecuteCommandAsync();
            }
            deleteCount = await DBDelete.In(entityIds).ExecuteCommandAsync();
            return deleteCount;
        }
        #endregion

    }

    public static class RepositoryExtensionHelper
    {
        public static async Task<PageModel<TEntity>> ToPageModel<TEntity>(this ISugarQueryable<TEntity> queryable, int pageIndex = 1, int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var list = await queryable.ToPageListAsync(pageIndex, pageSize, totalCount);
            return new PageModel<TEntity>() { PageIndex = pageIndex, TotalCount = totalCount, PageSize = pageSize, Data = list };
        }
    }
}
