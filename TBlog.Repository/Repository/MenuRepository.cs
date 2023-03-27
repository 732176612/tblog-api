using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.IRepository;
using TBlog.Model;

namespace TBlog.Repository
{
    public class MenuRepository : MongoRepository<MenuEntity>, IMenuRepository
    {
        public MenuRepository(IMongoTransaction transaction) : base(transaction)
        {

        }

        public IEnumerable<MenuEntity> GetByRoleIds(IEnumerable<long> roleIds)
        {
            return GetAll().Result.Where(c => roleIds.Intersect(c.RoleIds).Any()).AsQueryable();
        }
    }
}
