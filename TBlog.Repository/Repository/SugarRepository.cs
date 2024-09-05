namespace TBlog.Repository
{
    public class SugarRepository<TEntity> : ISugarRepository<TEntity> where TEntity : class, IEntity, new()
    {

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
