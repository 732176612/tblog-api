namespace TBlog.Model
{
    /// <summary>
    /// Http日记记录
    /// </summary>
    [SplitTable(SplitType.Year)]
    [SugarTable("HttpLog_{year}{month}{day}")]
    [SugarIndex("CDate", "CDate", OrderByType.Asc)]
    public class HttpLogEntity : IEntity
    {
        #region 基础属性
        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; } = SnowFlakeSingle.instance.NextId();

        /// <summary>
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SplitField]
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
        /// 用户姓名
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = ConstHelper.UserNameLength)]
        public string UserName { get; set; } = "";

        /// <summary>
        /// 请求IP
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 39)]
        public string IP { get; set; } = "";

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; } = "";

        /// <summary>
        /// 路径
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 255)]
        public string Url { get; set; } = "";

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
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 50)]
        public string RequestMethod { get; set; } = "";

        /// <summary>
        /// 请求数据
        /// </summary>
        [SugarColumn(ColumnDataType = "TEXT")]
        public string RequestData { get; set; } = "";

        /// <summary>
        /// 响应数据
        /// </summary>
        [SugarColumn(ColumnDataType = "TEXT")]
        public string ResponetData { get; set; } = "";

        /// <summary>
        /// 用户代理标识
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 100)]
        public string UserAgent { get; set; } = "";
        #endregion
    }
}
