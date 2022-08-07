namespace TBlog.Model
{
    /// <summary>
    /// 菜单实体
    /// </summary>
    public class MenuEntity : IEntity
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
        /// 父级Id
        /// </summary>
        public long PId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 20, IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// 菜单链接地址
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string Url { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderSort { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 拥有此菜单的角色ID
        /// </summary>
        public long[] RoleIds { get; set; }
        #endregion
    }
}
