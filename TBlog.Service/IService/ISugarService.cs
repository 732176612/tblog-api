namespace TBlog.IService
{
    public interface ISugarService<TRepository, TEntity> : IBaseService<TEntity> where TEntity : class, new() where TRepository : ISugarRepository<TEntity>
    {
        public TRepository Repository { get; set; }
    }
}