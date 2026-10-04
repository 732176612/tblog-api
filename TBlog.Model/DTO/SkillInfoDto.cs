namespace TBlog.Model
{
    /// <summary>
    /// 专业技能
    /// </summary>
    public class SkillInfoDto : IDto
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 技能
        /// </summary>
        public string Skill { get; set; }

        /// <summary>
        /// 熟练度
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// 展示方式：progress 或 text
        /// </summary>
        public string DisplayMode { get; set; } = "progress";
    }
}
