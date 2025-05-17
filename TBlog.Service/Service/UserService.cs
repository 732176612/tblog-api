using System.Text.RegularExpressions;

namespace TBlog.Service
{
    public class UserService : BaseService<UserEntity>, IUserService
    {
        private readonly IUserRepository Repository;
        public UserService(IUserRepository userRepository) 
        {
            Repository = userRepository;
        }

        public async Task<bool> CheckHavePhoneOrMail(string phoneOrMail)
        {
            return await Repository.GetByPhoneOrMail(phoneOrMail) != null;
        }

        public async Task<bool> CheckHaveBlogName(string blogName)
        {
            return await Repository.GetByBlogName(blogName) != null;
        }

        public async Task<UserEntity> SaveUserInfo(long userId, UserInfoDto dto)
        {
            var userEntity = await Repository.DBQuery.InSingleAsync(userId);
            if (userEntity == null)
            {
                throw new TBlogApiException("错误的授权信息，请退出重新登陆");
            }

            if (string.IsNullOrEmpty(userEntity.BlogName))
            {
                if (string.IsNullOrEmpty(dto.BlogName))
                {
                    throw new TBlogApiException("博客名称不能为空");
                }

                //播博客名称只能英文
                if (!Regex.IsMatch(dto.BlogName, @"^[a-zA-Z]+$"))
                {
                    throw new TBlogApiException("博客名称只能包含字母");
                }
                dto.BlogName = dto.BlogName.ToLower();

                if (await CheckHaveBlogName(dto.BlogName))
                {
                    throw new TBlogApiException("博客名称已被注册");
                }

                userEntity.BlogName = dto.BlogName;
            }

            if (string.IsNullOrEmpty(userEntity.UserName))
            {
                if (string.IsNullOrEmpty(dto.BlogName))
                {
                    throw new TBlogApiException("用户名称不能为空");
                }
            }

            userEntity.UserName = dto.UserName;
            userEntity.Birthday = dto.Birthday.ToDateTime();
            userEntity.HeadImgUrl = dto.HeadImgUrl;
            userEntity.Sex = dto.Sex;
            userEntity.Introduction = dto.Introduction;
            userEntity.Sign = dto.Sign;
            userEntity.ResumeUrl = dto.ResumeUrl;
            userEntity.ResumeName = dto.ResumeName;
            userEntity.BackgroundUrl = dto.BackgroundUrl;
            userEntity.StyleColor = dto.StyleColor;
            await Repository.Update(userEntity);
            return userEntity;
        }

        public async Task<UserInfoDto> GetUserInfo(string blogName)
        {
            var userEntity = await Repository.GetByBlogName(blogName);
            if (userEntity == null) return null;
            var dto = userEntity.ToDto<UserInfoDto, UserEntity>();
            return dto;
        }

        public async Task RegisterUser(UserResigterDto dto)
        {
            var entity = dto.ToEntity<UserEntity, UserResigterDto>();
            entity.Password = MD5Helper.MD5Encrypt32(entity.Password);
            var userRole = await DbScoped.SugarScope.Queryable<RoleEntity>().FirstAsync(c => ConstHelper.UserRole.Equals(c.Name));
            entity.RoleIds = entity.RoleIds.Concat([userRole.Id]).Distinct().ToArray();
            await DbScoped.SugarScope.Insertable(entity).ExecuteCommandAsync();
        }

        public async Task<UserEntity> LoginUser(UserLoginDto dto)
        {
            var entity = await Repository.GetByPhoneOrMail(dto.PhoneOrMail);
            if (entity == null || !entity.Password.Equals(MD5Helper.MD5Encrypt32(dto.Password))) return null;
            entity.LoginDate = DateTime.UtcNow;
            await Repository.Update(entity);
            return entity;
        }
    }
}
