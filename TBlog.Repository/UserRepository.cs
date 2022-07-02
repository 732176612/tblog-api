using TBlog.Model;
using TBlog.IRepository;
using TBlog.Common;
using System.Threading.Tasks;

namespace TBlog.Repository
{
    public class UserRepository : MongoRepository<UserEntity>, IUserRepository
    {
        public UserRepository(IMongoTransaction transaction) : base(transaction)
        {

        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new Task AddEntity(UserEntity entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = IdBuilder.CreateId();
            }
            return base.AddEntity(entity);
        }

        public Task<UserEntity> GetByPhoneOrMail(string phoneOrMail)
        {
            return GetSingle(c => c.Phone.ToLower() == phoneOrMail.ToLower() || c.Email.ToLower() == phoneOrMail.ToLower());
        }

        public Task<UserEntity> GetByBlogName(string blogName)
        {
            return GetSingle(c => c.BlogName.ToLower() == blogName.ToLower());
        }
    }
}
