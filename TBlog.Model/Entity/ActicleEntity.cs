using Nest;

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
        /// 文章标题
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 30, IsNullable = true)]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string Content { get; set; }

        /// <summary>
        /// 封面海报
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string PosterUrl { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ActicleTagEntity.ActicleId))]
        public List<ActicleTagEntity> Tags { get; set; }

        /// <summary>
        /// 文章统计信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ActicleStatsEntity.ActicleId))]
        public ActicleStatsEntity Stats { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public EnumActicleType ActicleType { get; set; }

        /// <summary>
        /// 发布形式
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public EnumActicleReleaseForm ReleaseForm { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long CUserId { get; set; }
        #endregion
    }
}
