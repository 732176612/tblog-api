namespace TBlog.IRepository
{
    public interface ISugarRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        public ISugarQueryable<TEntity> DBQuery { get; }

        public IUpdateable<TEntity> DBUpdate { get; }

        public IDeleteable<TEntity> DBDelete { get; }
    }
}
