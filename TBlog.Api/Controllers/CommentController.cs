using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TBlog.Api
{
    /// <summary>
    /// 评论
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController : TblogController
    {
        private readonly ICommentService _commentService;
        private static ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            this._commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// 创建评论
        /// </summary>
        [HttpPost]
        public async Task<APITResult<long>> CreateComment(CreateCommentDto dto)
        {
            var token = GetToken();
            var commentId = await _commentService.CreateComment(dto, token.UserId);
            return APITResult<long>.Success("评论成功", commentId);
        }

        /// <summary>
        /// 获取文章评论列表
        /// </summary>
        [HttpGet]
        public async Task<APITResult<PageModel<CommentDto>>> GetCommentList(long acticleId, int pageIndex = 1, int pageSize = 10)
        {
            var token = GetToken(true);
            var page = await _commentService.GetCommentList(acticleId, pageIndex, pageSize, token?.UserId);
            return APITResult<PageModel<CommentDto>>.Success("获取成功", page);
        }

        /// <summary>
        /// 获取评论的子评论
        /// </summary>
        [HttpGet]
        public async Task<APITResult<PageModel<CommentDto>>> GetChildComments(long rootId, int pageIndex = 1, int pageSize = 10)
        {
            var token = GetToken(true);
            var page = await _commentService.GetChildComments(rootId, pageIndex, pageSize, token?.UserId);
            return APITResult<PageModel<CommentDto>>.Success("获取成功", page);
        }

        /// <summary>
        /// 点赞评论
        /// </summary>
        [HttpGet]
        public async Task<APIResult> LikeComment(long commentId)
        {
            var token = GetToken();
            await _commentService.LikeComment(commentId, token.UserId, HttpContext.GetIpAddress());
            return APIResult.Success("点赞成功");
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        [HttpGet]
        public async Task<APIResult> DeleteComment(long commentId)
        {
            var token = GetToken();
            await _commentService.DeleteComment(commentId, token.UserId);
            return APIResult.Success("删除成功");
        }

        /// <summary>
        /// 获取评论详情
        /// </summary>
        [HttpGet]
        public async Task<APITResult<CommentDto>> GetCommentDetail(long commentId)
        {
            var token = GetToken(true);
            var comment = await _commentService.GetCommentDetail(commentId, token?.UserId);
            return APITResult<CommentDto>.Success("获取成功", comment);
        }
    }
} 