namespace TBlog.Service
{
    public class SugarService<TRepository,TEntity> : BaseService<TEntity> where TEntity : class, new() where TRepository : ISugarRepository<TEntity>
    {
        public TRepository Repository { get; set; }
    }
}