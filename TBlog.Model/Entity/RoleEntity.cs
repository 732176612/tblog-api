namespace TBlog.Model
{
    /// <summary>
    /// 角色实体
    /// </summary>
    public class RoleEntity : IEntity
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
        /// 上级角色ID
        /// </summary>
        public long PId { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = ConstHelper.UserNameLength, IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        ///排序
        /// </summary>
        public int OrderSort { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 150, IsNullable = true)]
        public string Desc { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CUserId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = ConstHelper.UserNameLength, IsNullable = true)]
        public string CUserName { get; set; }

        /// <summary>
        /// 修改ID
        /// </summary>
        public long MUserId { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = ConstHelper.UserNameLength, IsNullable = true)]
        public string MUserName { get; set; }
        #endregion
    }
}
