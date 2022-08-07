namespace TBlog.Model
{
    /// <summary>
    /// 文章历史记录
    /// </summary>
    public class ActicleHisLogEntity : IEntity
    {
        #region 基础属性
        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

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

        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object EntityId => Id;
        #endregion

        #region 实体属性

        /// <summary>
        /// 文章Id
        /// </summary>
        public long ActicleId { get; set; }

        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public EnumActicleHisType HisType { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CUserId { get; set; }
        #endregion
    }
}
