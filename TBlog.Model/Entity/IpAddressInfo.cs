namespace TBlog.Model
{
    public class IpAddressInfo : IEntity
    {
        #region 基础属性
        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = SnowFlakeSingle.instance.NextId();

        /// <summary>
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime MDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object EntityId => Id;
        #endregion

        #region 实体属性
        /// <summary>
        /// IP
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 20)]
        public string Ip { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 255)]
        public string Address { get; set; } = string.Empty;
        #endregion
    }
}
