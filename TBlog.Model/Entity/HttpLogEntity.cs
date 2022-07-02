namespace TBlog.Model
{
    /// <summary>
    /// Http日记记录
    /// </summary>
    public class HttpLogEntity : IEntity
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
        /// 用户姓名
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = ConstHelper.UserNameLength, IsNullable = true)]
        public string UserName { get; set; }

        /// <summary>
        /// 请求IP
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 128, IsNullable = true)]
        public string IP { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string Url { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 12, IsNullable = true)]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 请求数据
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 8000, IsNullable = true)]
        public string RequestData { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 8000, IsNullable = true)]
        public string ResponetData { get; set; }

        /// <summary>
        /// 用户代理标识
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string UserAgent { get; set; }
        #endregion
    }
}
