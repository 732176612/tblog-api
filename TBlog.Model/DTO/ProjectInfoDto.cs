namespace TBlog.Model
{
    /// <summary>
    /// 项目经历
    /// </summary>
    public class ProjectInfoDto : IDto
    {
        /// <summary>
        /// 创建者Id
        /// </summary>
        public long CUserId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// 担任角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; }

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
