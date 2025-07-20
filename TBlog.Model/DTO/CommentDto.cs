using System.ComponentModel.DataAnnotations;

namespace TBlog.Model
{
    /// <summary>
    /// 评论DTO
    /// </summary>
    public class CommentDto : IDto
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 文章ID
        /// </summary>
        public string ActicleId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required(ErrorMessage = "评论内容不能为空")]
        [MaxLength(2000, ErrorMessage = "评论内容不能超过2000字")]
        public string Content { get; set; } = "";

        /// <summary>
        /// 父评论ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 根评论ID
        /// </summary>
        public string RootId { get; set; }

        /// <summary>
        /// 评论层级
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; } = 0;

        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CDate { get; set; }

        /// <summary>
        /// 是否已点赞
        /// </summary>
        public bool IsLiked { get; set; } = false;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 评论者信息
        /// </summary>
        public UserInfoDto User { get; set; }

        /// <summary>
        /// 子评论列表
        /// </summary>
        public List<CommentDto> Children { get; set; } = new List<CommentDto>();

        /// <summary>
        /// 父评论信息（用于回复显示）
        /// </summary>
        public CommentDto Parent { get; set; }

        /// <summary>
        /// 是否显示折叠按钮
        /// </summary>
        public bool ShowCollapse { get; set; } = false;

        /// <summary>
        /// 是否已折叠
        /// </summary>
        public bool IsCollapsed { get; set; } = false;
    }

    /// <summary>
    /// 创建评论DTO
    /// </summary>
    public class CreateCommentDto : IDto
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Required(ErrorMessage = "文章ID不能为空")]
        public string ActicleId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required(ErrorMessage = "评论内容不能为空")]
        [MaxLength(2000, ErrorMessage = "评论内容不能超过2000字")]
        public string Content { get; set; } = "";

        /// <summary>
        /// 父评论ID
        /// </summary>
        public string ParentId { get; set; }
    }

    /// <summary>
    /// 评论查询DTO
    /// </summary>
    public class CommentQueryDto : IDto
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Required(ErrorMessage = "文章ID不能为空")]
        public string ActicleId { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 根评论ID（用于获取子评论）
        /// </summary>
        public string RootId { get; set; }
    }
} 