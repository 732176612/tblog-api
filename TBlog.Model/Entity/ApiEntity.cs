namespace TBlog.Model
{
    /// <summary>
    /// 接口实体
    /// </summary>
    public class ApiEntity : IEntity
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
        /// 接口路径
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string Url { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string Desc { get; set; }

        /// <summary>
        /// 过滤规则
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public ApiFilterRuleModel[] FilterRules { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderSort { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 拥有此接口权限的角色ID
        /// </summary>
        public long[] RoleIds { get; set; } = new long[0];
        #endregion
    }

    /// <summary>
    /// 接口过滤模型
    /// </summary>
    public class ApiFilterRuleModel
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 秒数
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int Count { get; set; }
    }
}
