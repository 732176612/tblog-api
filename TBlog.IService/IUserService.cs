using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IService
{
    public interface IUserService : IBaseService<UserEntity>
    {
        public Task<bool> CheckHavePhoneOrMail(string phoneOrMail);

        public Task<bool> CheckHaveBlogName(string blogName);

        public Task RegisterUser(UserResigterDto dto);

        public Task<UserEntity> LoginUser(UserLoginDto dto);

        public Task<UserEntity> SaveUserInfo(long userId,UserInfoDto dto);

        public Task<UserInfoDto> GetUserInfo(string blogName);
    }
}