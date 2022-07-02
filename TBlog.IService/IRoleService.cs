using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IService
{
    public interface IRoleService:IBaseService<RoleEntity>
    {
        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RoleEntity GetByName(string name);

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<RoleEntity> GetByName(IEnumerable<string> name);
    }
}
