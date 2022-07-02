namespace TBlog.Model
{
    /// <summary>
    /// 项目经历
    /// </summary>
    public class ProjectInfoEntity : IEntity
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
        /// 创建者Id
        /// </summary>
        public long CUserId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [Description("项目名称")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 20, IsNullable = false)]
        [StringLength(20)]
        public string Project { get; set; }

        /// <summary>
        /// 担任角色
        /// </summary>
        [Description("担任角色")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 20, IsNullable = false)]
        [StringLength(20)]
        public string Role { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        [Description("所在城市")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 20, IsNullable = false)]
        [StringLength(20)]
        public string City { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Description("开始时间")]
        [SugarColumn(ColumnDataType = "Date", IsNullable = false)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Description("结束时间")]
        [SugarColumn(ColumnDataType = "Date", IsNullable = false)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 经历描述
        /// </summary>
        [Description("经历描述")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 140, IsNullable = true)]
        public string Introduction { get; set; }
        #endregion
    }
}
