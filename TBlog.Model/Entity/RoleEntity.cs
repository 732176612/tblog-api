namespace TBlog.Model
{
    /// <summary>
    /// 角色实体
    /// </summary>
    [BsonIgnoreExtraElements]
    public class RoleEntity : IEntity
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
        /// <summary>
        /// 角色名
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = ConstHelper.UserNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 50)]
        public string Desc { get; set; }

        /// <summary>
        ///排序
        /// </summary>
        public int OrderSort { get; set; }
        #endregion
    }
}
