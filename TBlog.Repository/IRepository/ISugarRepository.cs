using TBlog.Repository;

namespace TBlog.IRepository
{
    public interface ISugarRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        public ISugarQueryable<TEntity> Queryable => SqlSugarHelper.DB.Queryable<TEntity>();
    }
}
