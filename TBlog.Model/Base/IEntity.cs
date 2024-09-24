namespace TBlog.Model
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        object EntityId { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime MDate { get; set; }
    }
}
