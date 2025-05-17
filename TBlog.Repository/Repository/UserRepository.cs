namespace TBlog.Repository
{
    public class UserRepository : SugarRepository<UserEntity>, IUserRepository
    {
        public Task<UserEntity> GetByPhoneOrMail(string phoneOrMail)
        {
            if (phoneOrMail.Contains("@")) return DBQuery.FirstAsync(c => c.Email == phoneOrMail);
            return DBQuery.FirstAsync(c => c.Phone == phoneOrMail);
        }

        public Task<UserEntity> GetByBlogName(string blogName)
        {
            return DBQuery.FirstAsync(c => c.BlogName == blogName);
        }
    }
}
