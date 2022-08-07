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
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool IsDeleted { get; set; }

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
