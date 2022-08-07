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
    /// 菜单
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : TblogController
    {
        private readonly IMenuService _testServer;
        private static ILogger<TestController> _logger;
        private readonly ISugarRepository<RoleEntity> _role;

        public TestController(IMenuService testServer, ILogger<TestController> logger, ISugarRepository<RoleEntity> role)
        {
            this._testServer = testServer;
            _logger = logger;
            _role = role;
        }

        /// <summary>
        /// 测试
        /// </summary>
        [HttpGet]
        public APIResult Test()
        {
            var test = _role.AddEntity(new RoleEntity
            {
                Id = 30000,
                Name = "test"
            });
            return APIResult.Success();
        }
    }
}
