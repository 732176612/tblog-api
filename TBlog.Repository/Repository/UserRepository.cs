using TBlog.Model;
using TBlog.IRepository;
using TBlog.Common;
using System.Threading.Tasks;

namespace TBlog.Repository
{
    public class UserRepository : SugarRepository<UserEntity>, IUserRepository
    {
        public Task<UserEntity> GetByPhoneOrMail(string phoneOrMail)
        {
            return DBQuery.FirstAsync(c => c.Phone == phoneOrMail || c.Email == phoneOrMail);
        }

        public Task<UserEntity> GetByBlogName(string blogName)
        {
            return DBQuery.FirstAsync(c => c.BlogName.ToLower() == blogName.ToLower());
        }
    }
}
