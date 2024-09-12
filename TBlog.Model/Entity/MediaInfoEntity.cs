namespace TBlog.Model
{
    /// <summary>
    /// 媒体文件信息
    /// </summary>
    public class MediaInfoEntity : IEntity
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
        /// 文件名
        /// </summary>
        [Description( "文件名")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        [StringLength(100, MinimumLength = 1)]
        public string FileName { get; set; }

        [Description( "云链接")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        [StringLength(2000, MinimumLength = 1)]
        public string Url { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Description( "文件大小")]
        public long Size { get; set; }

        /// <summary>
        /// 上传者ID
        /// </summary>
        [Description( "上传者ID")]
        public long CUserId { get; set; }

        /// <summary>
        /// 媒体文件类型
        /// </summary>
        [Description( "媒体文件类型")]
        public EnumMediaType MediaType { get; set; }
        #endregion
    }
}
