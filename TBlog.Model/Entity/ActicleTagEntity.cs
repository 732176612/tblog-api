namespace TBlog.Model
{
    /// <summary>
    /// 文章标签
    /// </summary>
    public class ActicleTagEntity : IEntity
    {
        #region 基础属性
        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = SnowFlakeSingle.instance.NextId();

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
        [SugarColumn(Length = 20, ColumnDataType = "VARCHAR")]
        public string Name { get; set; }

        /// <summary>
        /// 文章ID
        /// </summary>
        public long ActicleId { get; set; }
        #endregion
    }
}
