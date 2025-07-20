namespace TBlog.Service
{
    /// <summary>
    /// 评论服务接口
    /// </summary>
    public interface ICommentService : IBaseService<CommentEntity>
    {
        /// <summary>
        /// 创建评论
        /// </summary>
        /// <param name="dto">评论信息</param>
        /// <param name="userId">用户ID</param>
        /// <returns>评论ID</returns>
        Task<long> CreateComment(CreateCommentDto dto, long userId);

        /// <summary>
        /// 获取文章评论列表
        /// </summary>
        /// <param name="acticleId">文章ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>评论列表</returns>
        Task<PageModel<CommentDto>> GetCommentList(long acticleId, int pageIndex, int pageSize, long? userId = null);

        /// <summary>
        /// 获取评论的子评论
        /// </summary>
        /// <param name="rootId">根评论ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>子评论列表</returns>
        Task<PageModel<CommentDto>> GetChildComments(long rootId, int pageIndex, int pageSize, long? userId = null);

        /// <summary>
        /// 点赞评论
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="ipAddress">IP地址</param>
        /// <returns></returns>
        Task LikeComment(long commentId, long userId, string ipAddress);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task DeleteComment(long commentId, long userId);

        /// <summary>
        /// 获取评论详情
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <param name="userId">当前用户ID</param>
        /// <returns>评论详情</returns>
        Task<CommentDto> GetCommentDetail(long commentId, long? userId = null);
    }
} 