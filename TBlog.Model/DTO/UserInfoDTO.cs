namespace TBlog.Model
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoDto : IDto
    {
        /// <summary>
        /// 博客名称
        /// </summary>
        public string BlogName { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 个人介绍
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 性别 (男：1，女：2，无：0)
        /// </summary>
        public EnumSex Sex { get; set; } = 0;

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get
            {
                var isParse = DateTime.TryParse(Birthday, out var date);
                if (isParse)
                {
                    return DateTimeHelper.GetAgeByBirthdate(date).ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 简历链接
        /// </summary>
        public string ResumeUrl { get; set; }

        /// <summary>
        /// 简历名称
        /// </summary>
        public string ResumeName { get; set; }

        /// <summary>
        /// 微博主题颜色
        /// </summary>
        public string StyleColor { get; set; }

        /// <summary>
        /// 背景海报颜色
        /// </summary>
        public string BackgroundUrl { get; set; }
    }
}
