namespace TBlog.Model
{
    /// <summary>
    /// 菜单DTO
    /// </summary>
    public class MenuDto : IDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单链接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderSort { get; set; }
    }
}
