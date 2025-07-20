namespace TBlog.Model
{
    /// <summary>
    /// 评论点赞实体
    /// </summary>
    [BsonIgnoreExtraElements]
    public class CommentLikeEntity : IDeleteEntity
    {
        #region 基础属性
        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = SnowFlakeSingle.instance.NextId();

        /// <summary>
        /// 是否删除
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
        /// 评论ID
        /// </summary>
        public long CommentId { get; set; }

        /// <summary>
        /// 文章ID
        /// </summary>
        public long ActicleId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 点赞状态（1为点赞，0为取消点赞）
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// IP地址
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 50)]
        public string IpAddress { get; set; } = "";
        #endregion
    }
} 