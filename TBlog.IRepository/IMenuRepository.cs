using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IRepository
{
    public interface IMenuRepository: IMongoRepository<MenuEntity>
    {
        /// <summary>
        /// 根据角色Id获取菜单
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public IEnumerable<MenuEntity> GetByRoleIds(IEnumerable<long> roleIds);
    }
}
