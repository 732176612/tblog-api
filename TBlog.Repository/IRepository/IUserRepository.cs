using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IRepository
{
    public interface IUserRepository : IMongoRepository<UserEntity>
    {
        Task<UserEntity> GetByPhoneOrMail(string phoneOrMail);
        Task<UserEntity> GetByBlogName(string blogName);
    }
}
