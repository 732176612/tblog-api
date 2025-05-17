namespace TBlog.Service
{
    public class SugarService<TEntity> : BaseService<TEntity> where TEntity : class, new() 
    {
        public ISugarRepository<TEntity> Repository { get; set; }
    }
}