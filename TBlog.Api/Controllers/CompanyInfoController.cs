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
    /// 获取工作经历
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyInfoController : TblogController
    {
        private readonly ICompanyInfoService _CompanyInfoServer;
        private readonly IUserRepository _UserRepository;
        private static ILogger<CompanyInfoController> _logger;

        public CompanyInfoController(ICompanyInfoService CompanyInfoServer, IUserRepository userRepository, ILogger<CompanyInfoController> logger)
        {
            this._CompanyInfoServer = CompanyInfoServer;
            this._UserRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取工作经历
        /// </summary>
        /// <param name="blogName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APITResult<IEnumerable<CompanyInfoDto>>> Get(string blogName)
        {
            var userid = (await _UserRepository.GetByBlogName(blogName))?.Id ?? 0;
            return APITResult<IEnumerable<CompanyInfoDto>>.Success(await _CompanyInfoServer.Get(userid));
        }

        /// <summary>
        /// 保存工作经历
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> Save([FromBody] CompanyInfoDto[] dtos)
        {
            var token = GetToken();
            await _CompanyInfoServer.Save(dtos, token.UserId);
            return APIResult.Success("保存成功");
        }
    }
}
