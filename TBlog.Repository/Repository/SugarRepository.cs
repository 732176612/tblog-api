namespace TBlog.Repository
{
    public class SugarRepository<TEntity> : ISugarRepository<TEntity> where TEntity : class, IEntity, new()
    {
        public ISugarQueryable<TEntity> DBQuery { get => DBHelper.DB.Queryable<TEntity>(); }

        public IUpdateable<TEntity> DBUpdate { get => DBHelper.DB.Updateable<TEntity>(); }

        #region 插入
        public Task<int> AddEntity(TEntity entity)
        {
            entity.CDate = DateTime.UtcNow;
            entity.MDate = DateTime.UtcNow;
            return DBHelper.DB.Insertable(entity).ExecuteCommandAsync();
        }

        public Task<int> AddEntities(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.CDate = DateTime.UtcNow;
                entity.MDate = DateTime.UtcNow;
            });
            return DBHelper.DB.Insertable(entities).ExecuteCommandAsync();
        }
        #endregion

        #region 更新
        public Task<bool> Update(TEntity entity)
        {
            entity.MDate = DateTime.Now;
            return DBHelper.DB.Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        public Task<bool> Update(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.MDate = DateTime.UtcNow;
            });
            return DBHelper.DB.Updateable(entities).ExecuteCommandHasChangeAsync();
        }
        #endregion


        public IDeleteable<TEntity> DBDelete { get => DBHelper.DB.Deleteable<TEntity>(); }

        public Task<bool> Delete(TEntity DeleteObj)
        {
            return DBHelper.DB.Deleteable(DeleteObj).ExecuteCommandHasChangeAsync();
        }
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
