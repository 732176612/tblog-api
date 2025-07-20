using AutoMapper;
using SqlSugar;
using TBlog.Model;
using TBlog.Repository;

namespace TBlog.Service
{
    /// <summary>
    /// 评论服务实现
    /// </summary>
    public class CommentService : BaseService<CommentEntity>, ICommentService
    {
        private readonly ISugarRepository<CommentEntity> _repository;
        private readonly ISugarRepository<CommentLikeEntity> _commentLikeRepository;

        public CommentService(ISugarRepository<CommentEntity> repository,
            ISugarRepository<CommentLikeEntity> commentLikeRepository)
        {
            _repository = repository;
            _commentLikeRepository = commentLikeRepository;
        }

        /// <summary>
        /// 创建评论
        /// </summary>
        public async Task<long> CreateComment(CreateCommentDto dto, long userId)
        {
            var comment = new CommentEntity
            {
                ActicleId = dto.ActicleId.ToLong(),
                Content = dto.Content.Trim(),
                UserId = userId,
                ParentId = dto.ParentId.IsEmptyOrNull() ? 0 : dto.ParentId.ToLong(),
                Level = 0
            };

            // 如果是回复评论，设置层级和根评论ID
            if (comment.ParentId != 0)
            {
                var parentComment = await _repository.DBQuery.FirstAsync(it => it.Id == comment.ParentId);
                if (parentComment == null)
                {
                    throw new Exception("父评论不存在");
                }

                comment.Level = parentComment.Level + 1;
                comment.RootId = parentComment.RootId == 0 ? parentComment.Id : parentComment.RootId;

                // 更新父评论的回复数
                await _repository.DBUpdate.SetColumns(it => new CommentEntity { ReplyCount = it.ReplyCount + 1 })
                    .Where(it => it.Id == comment.ParentId).ExecuteCommandAsync();
            }

            await _repository.AddEntity(comment);
            return comment.Id;
        }

        /// <summary>
        /// 获取文章评论列表
        /// </summary>
        public async Task<PageModel<CommentDto>> GetCommentList(long acticleId, int pageIndex, int pageSize, long? userId = null)
        {
            var query = _repository.DBQuery
                .Where(it => it.ActicleId == acticleId && it.IsDeleted == false)
                .Where(it => it.ParentId == 0) // 只获取一级评论
                .OrderByDescending(it => it.SortWeight)
                .OrderByDescending(it => it.CDate);

            var total = await query.CountAsync();
            var comments = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var rootCommentIds = comments.Select(c => c.Id).ToList();

            var childComments = await _repository.DBQuery
                .Where(it => rootCommentIds.Contains(it.RootId))
                .ToListAsync();

            var commentDtos = comments.ToDto<CommentDto, CommentEntity>();
            var childCommentDtos = childComments.ToDto<CommentDto, CommentEntity>();

            var userIds = comments.Select(c => c.UserId).Union(childComments.Select(c => c.UserId)).Distinct().ToList();
            var userInfoDtos = await DbScoped.SugarScope.Queryable<UserEntity>()
                .Where(c => userIds.Contains(c.Id)).Select(c => new UserInfoDto())
                .ToListAsync();

            var likeCommentIds = await _commentLikeRepository.DBQuery
                .Where(it => it.ActicleId == acticleId && it.UserId == userId && it.Status == 1)
                .Select(it => it.CommentId.ToString())
                .ToListAsync();

            foreach (var item in childCommentDtos)
            {
                item.User = userInfoDtos.FirstOrDefault(c => c.Id == item.UserId);
                item.IsLiked = likeCommentIds.Contains(item.Id);
            }

            // 处理点赞状态和折叠显示
            foreach (var commentDto in commentDtos)
            {
                if (userId.HasValue)
                {
                    commentDto.IsLiked = likeCommentIds.Contains(commentDto.Id);
                }
                commentDto.User = userInfoDtos.FirstOrDefault(c => c.Id == commentDto.UserId);
                commentDto.Children = childCommentDtos
                    .Where(c => c.ParentId == commentDto.Id)
                    .OrderByDescending(c => c.CDate)
                    .ToList();

                // 如果子评论超过3条，显示折叠按钮
                if (commentDto.Children.Count >3)
                {
                    commentDto.ShowCollapse = true;
                    commentDto.IsCollapsed = true;
                }
            }

            return new PageModel<CommentDto>
            {
                Data = commentDtos,
                TotalCount = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 获取评论的子评论
        /// </summary>
        public async Task<PageModel<CommentDto>> GetChildComments(long rootId, int pageIndex, int pageSize, long? userId = null)
        {
            var query = _repository.DBQuery
                .Where(it => it.RootId == rootId && it.IsDeleted == false)
                .Where(it => it.ParentId != 0) // 只获取子评论
                .OrderBy(it => it.CDate);

            var total = await query.CountAsync();
            var comments = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var userInfoDtos = await DbScoped.SugarScope.Queryable<UserEntity>()
                .Where(c => userIds.Contains(c.Id)).Select(c => new UserInfoDto())
                .ToListAsync();

            var commentDtos = comments.ToDto<CommentDto, CommentEntity>();

            var likeCommentIds = new List<string>();
            if (comments.Any() && userId.HasValue)
            {
                likeCommentIds = await _commentLikeRepository.DBQuery
                .Where(it => it.ActicleId == comments.First().ActicleId && it.UserId == userId && it.Status == 1)
                .Select(it => it.CommentId.ToString())
                .ToListAsync();
            }


            foreach (var item in commentDtos)
            {
                item.User = userInfoDtos.FirstOrDefault(c => c.Id == item.UserId);
                item.IsLiked = likeCommentIds.Contains(item.Id);
            }

            // 处理点赞状态
            foreach (var commentDto in commentDtos)
            {
                commentDto.IsLiked = likeCommentIds.Contains(commentDto.Id);
            }

            return new PageModel<CommentDto>
            {
                Data = commentDtos,
                TotalCount = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 点赞评论
        /// </summary>
        public async Task LikeComment(long commentId, long userId, string ipAddress)
        {
            var comment = await _repository.DBQuery.FirstAsync(it => it.Id == commentId);
            if (comment == null || comment.IsDeleted)
            {
                throw new Exception("评论不存在");
            }

            var existingLike = await _commentLikeRepository.DBQuery
                .Where(it => it.CommentId == commentId && it.UserId == userId)
                .FirstAsync();

            if (existingLike == null)
            {
                // 创建新的点赞记录
                var like = new CommentLikeEntity
                {
                    CommentId = commentId,
                    UserId = userId,
                    ActicleId = comment.ActicleId,
                    Status = 1,
                    IpAddress = ipAddress
                };
                await _commentLikeRepository.AddEntity(like);

                // 更新评论点赞数
                await _repository.DBUpdate.SetColumns(it => new CommentEntity { LikeCount = it.LikeCount + 1 })
                    .Where(it => it.Id == commentId).ExecuteCommandAsync();
            }
            else
            {
                // 切换点赞状态
                var newStatus = existingLike.Status == 1 ? 0 : 1;
                await _commentLikeRepository.DBUpdate.SetColumns(it => new CommentLikeEntity { Status = newStatus })
                    .Where(it => it.Id == existingLike.Id).ExecuteCommandAsync();

                // 更新评论点赞数
                var likeChange = newStatus == 1 ? 1 : -1;
                await _repository.DBUpdate.SetColumns(it => new CommentEntity { LikeCount = it.LikeCount + likeChange })
                    .Where(it => it.Id == commentId).ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        public async Task DeleteComment(long commentId, long userId)
        {
            var comment = await _repository.DBQuery.FirstAsync(it => it.Id == commentId);
            if (comment == null || comment.IsDeleted)
            {
                throw new Exception("评论不存在");
            }

            // 检查权限（只能删除自己的评论）
            if (comment.UserId != userId)
            {
                throw new Exception("无权限删除此评论");
            }

            // 软删除评论
            await _repository.DBUpdate.SetColumns(it => new CommentEntity { IsDeleted = true })
                .Where(it => it.Id == commentId).ExecuteCommandAsync();

            // 如果有父评论，减少父评论的回复数
            if (comment.ParentId != 0)
            {
                await _repository.DBUpdate.SetColumns(it => new CommentEntity { ReplyCount = it.ReplyCount - 1 })
                    .Where(it => it.Id == comment.ParentId).ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 获取评论详情
        /// </summary>
        public async Task<CommentDto> GetCommentDetail(long commentId, long? userId = null)
        {
            var comment = await _repository.DBQuery
                .Where(it => it.Id == commentId && it.IsDeleted == false)
                .Includes(it => it.User)
                .FirstAsync();

            if (comment == null)
            {
                throw new Exception("评论不存在");
            }

            var commentDto = comment.ToDto<CommentDto, CommentEntity>();

            if (userId.HasValue)
            {
                commentDto.IsLiked = await CheckUserLiked(commentId, userId.Value);
            }

            return commentDto;
        }

        /// <summary>
        /// 检查用户是否点赞了评论
        /// </summary>
        private async Task<bool> CheckUserLiked(long commentId, long userId)
        {
            var like = await _commentLikeRepository.DBQuery
                .Where(it => it.CommentId == commentId && it.UserId == userId && it.Status == 1)
                .FirstAsync();

            return like != null;
        }
    }
}