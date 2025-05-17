namespace TBlog.Model
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public interface IDeleteEntity : IEntity
    {
        /// <summary>
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
