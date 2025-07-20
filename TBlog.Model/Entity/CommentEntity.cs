using SqlSugar;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TBlog.Model
{
    /// <summary>
    /// 评论实体
    /// </summary>
    [BsonIgnoreExtraElements]
    public class CommentEntity : IDeleteEntity
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
        /// 文章ID
        /// </summary>
        public long ActicleId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [SugarColumn(ColumnDataType = "TEXT")]
        [MaxLength(2000, ErrorMessage = "评论内容不能超过2000字")]
        public string Content { get; set; } = "";

        /// <summary>
        /// 评论者ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 父评论ID（用于回复功能）
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 根评论ID（用于分组显示）
        /// </summary>
        public long RootId { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; } = 0;

        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; } = 0;

        /// <summary>
        /// 评论层级（0为一级评论，1为二级评论，以此类推）
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// 排序权重（用于排序显示）
        /// </summary>
        public int SortWeight { get; set; } = 0;
        #endregion

        /// <summary>
        /// 评论者信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(UserId), nameof(UserEntity.Id))]
        [BsonIgnore]
        public UserEntity User { get; set; }

        /// <summary>
        /// 子评论列表
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ParentId), nameof(Id))]
        [BsonIgnore]
        public List<CommentEntity> Children { get; set; }

        /// <summary>
        /// 父评论信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(ParentId))]
        [BsonIgnore]
        public CommentEntity Parent { get; set; }
    }
} 