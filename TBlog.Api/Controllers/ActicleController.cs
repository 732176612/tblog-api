using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace TBlog.Api
{
    /// <summary>
    /// 文章
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ActicleController : TblogController
    {
        private readonly IActicleService _ActicleServer;
        private static ILogger<ActicleController> _logger;

        public ActicleController(IActicleService ActicleServer, ILogger<ActicleController> logger)
        {
            this._ActicleServer = ActicleServer;
            _logger = logger;
        }

        /// <summary>
        /// 检查重复标题
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APIResult> CheckRepeatTitle(string title)
        {
            var userid = GetToken().UserId;
            if (await _ActicleServer.CheckRepeatTitle(userid, title))
            {
                return APIResult.Fail("文章标题重复");
            }
            else
            {
                return APIResult.Success();
            }
        }

        /// <summary>
        /// 保存文章
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APITResult<string>> SaveActicle(ActicleDto dto)
        {
            var token = GetToken();
            return APITResult<string>.Success("发布成功", await _ActicleServer.SaveActicle(dto, token.UserId, token.BlogName));
        }

        /// <summary>
        /// 获取文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APITResult<ActicleDto>> GetActicle(long id)
        {
            var token = GetToken(true);
            return APITResult<ActicleDto>.Success("获取成功", await _ActicleServer.GetActicle(id, token.UserId));
        }

        /// <summary>
        /// 获取文章标题汇总
        /// </summary>
        [HttpGet]
        public async Task<APITResult<IEnumerable<string>>> GetTags(string blogName, EnumActicleReleaseForm releaseForm)
        {
            var token = GetToken(true);
            if (token.BlogName != blogName)
            {
                if(releaseForm == EnumActicleReleaseForm.Private|| releaseForm == EnumActicleReleaseForm.Draft)
                {
                    return APITResult<IEnumerable<string>>.Success("获取成功", Enumerable.Empty<string>());
                }
            }

            return APITResult<IEnumerable<string>>.Success("获取成功", await _ActicleServer.GetTagsByUseId(blogName,releaseForm));
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        [HttpGet]
        public async Task<APITResult<PageModel<ActicleDto>>> GetActicleList(int pageIndex, int pageSize, string blogName, EnumActicleReleaseForm releaseForm = EnumActicleReleaseForm.Public, EnumActicleSortTag acticleSortTag = EnumActicleSortTag.News, string tags = "")
        {
            var token = GetToken(true);
            if (token.BlogName != blogName)
            {
                if (releaseForm == EnumActicleReleaseForm.Private || releaseForm == EnumActicleReleaseForm.Draft)
                {
                    return APITResult<PageModel<ActicleDto>>.Success("获取成功", new PageModel<ActicleDto>());
                }
            }

            var page = await _ActicleServer.GetActicleList(pageIndex, pageSize, blogName, releaseForm, acticleSortTag, tags);
            return APITResult<PageModel<ActicleDto>>.Success("获取成功", page);
        }

        /// <summary>
        /// 点赞文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APIResult> LikeArticle(long id)
        {
            await _ActicleServer.LikeArticle(id, GetToken(true)?.UserId ?? 0, HttpContext.GetClientIP());
            return APIResult.Success();
        }

        /// <summary>
        /// 查阅文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APIResult> LookArticle(long id)
        {
            await _ActicleServer.LookArticle(id, GetToken(true)?.UserId ?? 0, HttpContext.GetClientIP());
            return APIResult.Success();
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APIResult> DeleteArticle(long id)
        {
            await _ActicleServer.DeleteArticle(id, GetToken(false)?.UserId ?? 0);
            return APIResult.Success();
        }
    }
}
