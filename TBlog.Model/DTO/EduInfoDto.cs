namespace TBlog.Model
{
    /// <summary>
    /// 教育经历
    /// </summary>
    public class EduInfoDto : IDto
    {
        /// <summary>
        /// 创建者Id
        /// </summary>
        public long CUserId { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }

        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 经历描述
        /// </summary>
        public string Introduction { get; set; }
    }
}
