namespace TBlog.IRepository
{
    public interface IUserRepository : ISugarRepository<UserEntity>
    {
        Task<UserEntity> GetByPhoneOrMail(string phoneOrMail);
        Task<UserEntity> GetByBlogName(string blogName);
    }
}
