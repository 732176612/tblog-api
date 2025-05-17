namespace TBlog.Service
{
    public interface ISugarService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        public ISugarRepository<TEntity> Repository { get; set; }
    }
}