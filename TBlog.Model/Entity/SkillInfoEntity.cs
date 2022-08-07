namespace TBlog.Model
{
    /// <summary>
    /// 专业技能
    /// </summary>
    public class SkillInfoEntity : IEntity
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
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 技能
        /// </summary>
        [Description("技能")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 20, IsNullable = false)]
        [StringLength(20)]
        public string Skill { get; set; }

        [Description("熟练度")]
        public int Progress { get; set; }
        #endregion
    }
}
