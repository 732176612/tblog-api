namespace TBlog.Model
{
    /// <summary>
    /// 验证正则表达式
    /// </summary>
    public enum EnumRegex
    {
        /// <summary>
        /// 手机号码正则表达式
        /// </summary>
        [Description("手机号码正则表达式")]
        PhoneRegex = 1,

        /// <summary>
        /// 邮箱正则表达式
        /// </summary>
        [Description("邮箱正则表达式")]
        MailRegex = 2,
    }
}
