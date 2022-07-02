namespace TBlog.Model
{
    /// <summary>
    /// 用户注册模型
    /// </summary>
    public class UserResigterDto : IDto
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VCode { get; set; }
    }
}
