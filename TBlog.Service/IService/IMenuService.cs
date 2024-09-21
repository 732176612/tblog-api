using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IService
{
    public interface IMenuService:IBaseService<MenuEntity>
    {
        /// <summary>
        /// 根据角色Id获取菜单
        /// </summary>
        public IEnumerable<MenuDto> GetByRoleIds(IEnumerable<long> roleIds);
    }
}
