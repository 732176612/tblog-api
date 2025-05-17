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
    /// 获取项目经历
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectInfoController : TblogController
    {
        private readonly IProjectInfoService _ProjectInfoServer;
        private readonly IUserRepository _UserRepository;
        private static ILogger<ProjectInfoController> _logger;

        public ProjectInfoController(IProjectInfoService projectInfoServer, IUserRepository userRepository, ILogger<ProjectInfoController> logger)
        {
            this._ProjectInfoServer = projectInfoServer;
            this._UserRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取项目经历
        /// </summary>
        [HttpGet]
        public async Task<APITResult<IEnumerable<ProjectInfoDto>>> Get(string blogName)
        {
            var userid = (await _UserRepository.GetByBlogName(blogName))?.Id ?? 0;
            return APITResult<IEnumerable<ProjectInfoDto>>.Success(await _ProjectInfoServer.Get(userid));
        }

        /// <summary>
        /// 保存项目经历
        /// </summary>
        [HttpPost]
        public async Task<APIResult> Save([FromBody] ProjectInfoDto[] dtos)
        {
            var token = GetToken();
            await _ProjectInfoServer.Save(dtos, token.UserId);
            return APIResult.Success("保存成功");
        }
    }
}
