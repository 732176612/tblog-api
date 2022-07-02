namespace TBlog.Model
{
    /// <summary>
    /// 用户找回密码模型
    /// </summary>
    public class UserRecoverDto : IDto
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneOrMail { get; set; }

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
