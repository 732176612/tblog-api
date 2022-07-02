namespace TBlog.Model
{
    /// <summary>
    /// 文章实体
    /// </summary>
    public class ActicleEntity : IEntity
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
        /// 文章标题
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 30, IsNullable = false)]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", IsNullable = true)]
        public string Content { get; set; }

        /// <summary>
        /// 封面海报
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", IsNullable = true)]
        public string PosterUrl { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        public EnumActicleType ActicleType { get; set; }

        /// <summary>
        /// 发布形式
        /// </summary>
        public EnumActicleReleaseForm ReleaseForm { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public long LikeNum { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        public long ShareNum { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public long CollectNum { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public long LookNum { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = false)]
        public long CUserId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = ConstHelper.UserNameLength, IsNullable = true)]
        public string CBlogName { get; set; }
        #endregion
    }
}
