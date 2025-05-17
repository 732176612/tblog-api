using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using TBlog.IRepository;
namespace TBlog.Api
{
    /// <summary>
    /// 获取教育经历
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SkillInfoController : TblogController
    {
        private readonly ISkillInfoService _SkillInfoServer;
        private readonly IUserRepository _UserRepository;
        private static ILogger<SkillInfoController> _logger;

        public SkillInfoController(ISkillInfoService SkillInfoServer, IUserRepository userRepository, ILogger<SkillInfoController> logger)
        {
            this._SkillInfoServer = SkillInfoServer;
            this._UserRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取工作经历
        /// </summary>
        [HttpGet]
        public async Task<APITResult<IEnumerable<SkillInfoDto>>> Get(string blogName)
        {
            var userid = (await _UserRepository.GetByBlogName(blogName))?.Id ?? 0;
            return APITResult<IEnumerable<SkillInfoDto>>.Success(await _SkillInfoServer.Get(userid));
        }

        /// <summary>
        /// 保存工作经历
        /// </summary>
        [HttpPost]
        public async Task<APIResult> Save([FromBody] SkillInfoDto[] dtos)
        {
            var token = GetToken();
            await _SkillInfoServer.Save(dtos, token.UserId);
            return APIResult.Success("保存成功");
        }
    }
}
