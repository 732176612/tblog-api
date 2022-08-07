using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IRepository
{
    public interface IRoleRepository: IMongoRepository<RoleEntity>
    {
        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<RoleEntity> GetByName(string name);

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public Task<List<RoleEntity>> GetByNames(IEnumerable<string> names);
    }
}
