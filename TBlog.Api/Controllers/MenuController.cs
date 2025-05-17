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
    /// 菜单
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MenuController : TblogController
    {
        private readonly IMenuService _menuServer;
        private static ILogger<MenuController> _logger;

        public MenuController(IMenuService menuServer, ILogger<MenuController> logger)
        {
            this._menuServer = menuServer;
            _logger = logger;
        }

        /// <summary>
        /// 根据角色权限获取菜单
        /// </summary>
        [HttpGet]
        public async Task<APITResult<IEnumerable<MenuDto>>> GetMenus()
        {
            var token = GetToken(true);
            var roleIds = (token == null || token.RoleIds.Count() == 0) ? null: token.RoleIds;
            var dto = await _menuServer.GetByRoleIds(roleIds);
            return APITResult<IEnumerable<MenuDto>>.Success(dto);
        }
    }
}
