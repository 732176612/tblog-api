namespace TBlog.Model
{
    /// <summary>
    /// 用户登录模型
    /// </summary>
    public class UserLoginDto : IDto
    {
        /// <summary>
        /// 邮箱或手机号码
        /// </summary>
        public string PhoneOrMail { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
