namespace TBlog.Model
{
    /// <summary>
    /// 教育经历
    /// </summary>
    [BsonIgnoreExtraElements]
    public class EduInfoEntity : IDeleteEntity
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
        /// 创建者Id
        /// </summary>
        public long CUserId { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 20)]
        [StringLength(20)]
        public string School { get; set; } = string.Empty;

        [Description("专业")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 20)]
        [StringLength(20)]
        public string Major { get; set; } = string.Empty;

        /// <summary>
        /// 开始时间
        /// </summary>
        [Description("开始时间")]
        [SugarColumn(IsNullable =false, Length = 15)]
        public string StartDate { get; set; } = string.Empty;

        /// <summary>
        /// 结束时间
        /// </summary>
        [Description("结束时间")]
        [SugarColumn(IsNullable = false,Length =15)]
        public string EndDate { get; set; } = string.Empty;

        /// <summary>
        /// 经历描述
        /// </summary>
        [Description("经历描述")]
        [SugarColumn(ColumnDataType = "TEXT")]
        public string Introduction { get; set; } = string.Empty;
        #endregion
    }
}
