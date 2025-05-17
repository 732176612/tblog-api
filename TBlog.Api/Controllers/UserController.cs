using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TBlog.Service;
using TBlog.IRepository;
using TBlog.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using TBlog.Common;
using System.Linq;
using TBlog.Extensions;
using Microsoft.AspNetCore.Http;
namespace TBlog.Api
{
    /// <summary>
    /// 用户API
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : TblogController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IRedisRepository _redis;
        private readonly IUserRepository _userRepository;

        public UserController(ILogger<UserController> logger, IUserService userServer, IUserRepository userRepository, IRedisRepository redis)
        {
            _logger = logger;
            _userService = userServer;
            _userRepository = userRepository;
            _redis = redis;
        }

        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        [HttpGet]
        public async Task<APIResult> CheckHavePhoneOrMail(string phoneOrMail)
        {
            if (await _userService.CheckHavePhoneOrMail(phoneOrMail))
            {
                return APIResult.Fail("该手机或邮箱已被注册");
            }
            return APIResult.Success();
        }

        /// <summary>
        /// 检查博客名称是否存在
        /// </summary>
        [HttpGet]
        public async Task<APIResult> CheckHaveBlogName(string blogName)
        {
            if (await _userService.CheckHaveBlogName(blogName))
            {
                return APIResult.Fail("该博客名称已被注册");
            }
            return APIResult.Success();
        }

        /// <summary>
        /// 普通用户注册
        /// </summary>
        [HttpPost]
        public async Task<APIResult> RegisterUser([FromBody] UserResigterDto dto)
        {
            var ip = HttpHelper.GetIpAddress(HttpContext);
            var cacheKey = $"{ApiConfig.BaseSetting.ApiName}_RequestVCode_{ip}";
            var vcode = await _redis.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(vcode) || dto.VCode != vcode)
            {
                return APIResult.Fail("验证码错误!");
            }
            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (await _userService.CheckHavePhoneOrMail(dto.Email))
                {
                    return APIResult.Fail("该邮箱或手机号码已被注册");
                }
            }
            if (!string.IsNullOrEmpty(dto.Phone))
            {
                if (await _userService.CheckHavePhoneOrMail(dto.Phone))
                {
                    return APIResult.Fail("该邮箱或手机号码已被注册");
                }
            }
            await _userService.RegisterUser(dto);
            return APIResult.Success("注册成功");
        }

        /// <summary>
        /// 请求验证码
        /// </summary>
        [HttpGet]
        public async Task<APIResult> RequestVCode(string phoneOrMail)
        {
            if (string.IsNullOrEmpty(phoneOrMail))
                return APIResult.Fail("手机号或邮箱不能为空!");
            var ip = HttpHelper.GetIpAddress(HttpContext);
            var cacheKey = $"{ApiConfig.BaseSetting.ApiName}_RequestVCode_{ip}";
            var timeKey = $"{ApiConfig.BaseSetting.ApiName}_RequestTime_{ip}";
            var timeValue = DateTime.Parse((await _redis.Get<string>(timeKey)) ?? DateTime.Now.ToString());
            if ((DateTime.Now - timeValue).TotalSeconds < 60)
            {
                if (await _userService.CheckHavePhoneOrMail(phoneOrMail))
                {
                    return APIResult.Fail("该手机或邮箱已被注册");
                }
                var vcode = new Random().Next(1000, 9999).ToString();
                if (Regex.IsMatch(phoneOrMail, ConstHelper.MailRegex))
                {
                    TencentCloudCos.SendResigterMail(phoneOrMail, vcode);
                }
                else if (Regex.IsMatch(phoneOrMail, ConstHelper.PhoneRegex))
                {
                    TencentCloudCos.SendResigterSMS(phoneOrMail, vcode);
                }
                else
                {
                    return APIResult.Fail("请输入正确的邮箱或手机号码");
                }
                await _redis.Set(cacheKey, vcode, 900);
                await _redis.Set(timeKey, DateTime.Now.ToString(), 60);
            }
            else
            {
                return APIResult.Fail("请求过于频繁，请耐心等待");
            }
            return APIResult.Success("验证码已发送");
        }

        /// <summary>
        /// 普通用户登陆
        /// </summary>
        [HttpPost]
        public async Task<APITResult<string>> LoginUser([FromBody] UserLoginDto loginDTO)
        {
            var userEntity = await _userService.LoginUser(loginDTO);
            if (userEntity == null)
            {
                return APITResult<string>.Fail("账号或密码不正确");
            }
            userEntity.LoginDate = DateTime.Now;

            var roleEntities = await DbScoped.SugarScope.Queryable<RoleEntity>().In(userEntity.RoleIds).ToListAsync();
            var roleNames = string.Join(",", roleEntities.Select(c => c.Name));
            var roleIds = roleEntities.Select(c => c.Id);
            TokenJwtInfoModel tokenModel = new TokenJwtInfoModel { UserId = userEntity.Id, UserName = userEntity.UserName, RoleIds = roleIds, RoleName = roleNames, BlogName = userEntity.BlogName };
            SetToken(tokenModel);
            return APITResult<string>.Success("登陆成功", userEntity.BlogName ?? string.Empty);
        }

        /// <summary>
        /// 根据博客名称获取用户信息
        /// </summary>
        [HttpGet]
        public async Task<APITResult<UserInfoDto>> GetUserInfo(string blogName = "")
        {
            UserInfoDto dto = await _userService.GetUserInfo(string.IsNullOrEmpty(blogName) ? GetToken(true).BlogName : blogName);
            if (dto == null) return APITResult<UserInfoDto>.Fail("查找不到该用户信息");
            return APITResult<UserInfoDto>.Success("获取成功", dto);
        }

        /// <summary>
        /// 注销
        /// </summary>
        [HttpPost]
        public APIResult LogOut()
        {
            DeleteCookie("token");
            return APIResult.Success("注销成功!");
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        [HttpPost]
        public async Task<APIResult> SaveUserInfo(UserInfoDto dto)
        {
            try
            {
                var jwtModel = GetToken();
                var userEntity = await _userService.SaveUserInfo(jwtModel.UserId, dto);
                await RefreshToken();
                return APIResult.Success("欢迎来到TBlog!");
            }
            catch (TBlogApiException ex)
            {
                return APIResult.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 根据token获取用户信息
        /// </summary>
        [HttpGet]
        public APITResult<TokenJwtInfoModel> SerializeJwt(string token)
        {
            return APITResult<TokenJwtInfoModel>.Success("获取成功", AuthorizationHelper.SerializeJwt(token));
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        [HttpGet]
        public async Task<APITResult<string>> RefreshToken()
        {
            var oldToken = GetToken();
            if (oldToken == null)
            {
                return APITResult<string>.Fail("请先登陆");
            }
            var userid = oldToken.UserId;
            var userEntity = await _userRepository.DBQuery.InSingleAsync(userid);
            var roleEntities = await DbScoped.SugarScope.Queryable<RoleEntity>().In(userEntity.RoleIds).ToListAsync();
            var roleNames = string.Join(",", roleEntities.Select(c => c.Name));
            var roleIds = roleEntities.Select(c => c.Id);
            TokenJwtInfoModel tokenModel = new TokenJwtInfoModel { UserId = userEntity.Id, UserName = userEntity.UserName, RoleIds = roleIds, RoleName = roleNames, BlogName = userEntity.BlogName };
            var tokenStr = AuthorizationHelper.GenerateJwtStr(tokenModel);
            SetToken(tokenModel);
            return APITResult<string>.Success("刷新成功", tokenStr);
        }

        /// <summary>
        /// 请求找回密码
        /// </summary>
        [HttpGet]
        public async Task<APIResult> RequestRecoverPwd(string phoneOrMail)
        {
            if (string.IsNullOrEmpty(phoneOrMail))
                return APIResult.Fail("手机号或邮箱不能为空!");

            var userEntity = await _userRepository.GetByPhoneOrMail(phoneOrMail);

            if (userEntity == null)
            {
                return APIResult.Fail("找不到该账号信息");
            }

            var cacheKey = $"{ApiConfig.BaseSetting.ApiName}_RecoverPwd_{userEntity.Id}";
            if (string.IsNullOrEmpty(await _redis.Get<string>(cacheKey)))
            {
                var vcode = new Random().Next(1000, 9999).ToString();
                if (Regex.IsMatch(phoneOrMail, ConstHelper.MailRegex))
                {
                    string href = $"{ApiConfig.BaseSetting.Host}/view/recoverpwd?vcode={vcode}&phoneormail={phoneOrMail}";
                    TencentCloudCos.SendRecoverPwdMail(phoneOrMail, vcode);
                }
                else if (Regex.IsMatch(phoneOrMail, ConstHelper.PhoneRegex))
                {
                    TencentCloudCos.SendRecoverPwdSMS(phoneOrMail, vcode);
                }
                else
                {
                    return APIResult.Fail("请输入正确的邮箱或手机号码");
                }
                await _redis.Set(cacheKey, vcode, 900);
            }
            return APIResult.Success("验证码已发送，请查收");
        }

        /// <summary>
        /// 找回密码-重设密码
        /// </summary>
        [HttpPost]
        public async Task<APIResult> ResponeRecoverPwd([FromBody] UserRecoverDto dto)
        {
            if (string.IsNullOrEmpty(dto.PhoneOrMail))
                return APIResult.Fail("手机号或邮箱不能为空!");

            if (!Regex.IsMatch(dto.Password, ConstHelper.PassWordRegex))
            {
                return APIResult.Fail("密码格式不符合要求!");
            }

            var userEntity = await _userRepository.GetByPhoneOrMail(dto.PhoneOrMail);

            if (userEntity == null)
            {
                return APIResult.Fail("找不到该账号信息");
            }

            var cacheKey = $"{ApiConfig.BaseSetting.ApiName}_RecoverPwd_{userEntity.Id}";
            var cacheVcode = await _redis.Get<string>(cacheKey);

            if (string.IsNullOrEmpty(cacheVcode))
            {
                return APIResult.Fail("验证码不能为空");
            }

            if (dto.VCode == cacheVcode)
            {
                string newPassWord = MD5Helper.MD5Encrypt32(dto.Password);
                if (userEntity.Password == newPassWord)
                {
                    return APIResult.Fail("旧密码不能与新密码一致");
                }
                userEntity.Password = MD5Helper.MD5Encrypt32(dto.Password);
                await DbScoped.SugarScope.Updateable(userEntity).ExecuteCommandAsync();
                await _redis.Remove(cacheKey);
                return APIResult.Success("密码修改成功!");
            }
            else
            {
                return APIResult.Fail("验证码有误");
            }
        }
    }
}
