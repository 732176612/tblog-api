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
    public class EduInfoController : TblogController
    {
        private readonly IEduInfoService _EduInfoServer;
        private readonly IUserRepository _UserRepository;
        private static ILogger<EduInfoController> _logger;

        public EduInfoController(IEduInfoService EduInfoServer, IUserRepository userRepository, ILogger<EduInfoController> logger)
        {
            this._EduInfoServer = EduInfoServer;
            this._UserRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取工作经历
        /// </summary>
        /// <param name="blogName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APITResult<IEnumerable<EduInfoDto>>> Get(string blogName)
        {
            var userid = (await _UserRepository.GetByBlogName(blogName))?.Id ?? 0;
            return APITResult<IEnumerable<EduInfoDto>>.Success(await _EduInfoServer.Get(userid));
        }

        /// <summary>
        /// 保存工作经历
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> Save([FromBody] EduInfoDto[] dtos)
        {
            var token = GetToken();
            await _EduInfoServer.Save(dtos, token.UserId);
            return APIResult.Success("保存成功");
        }
    }
}
