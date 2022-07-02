using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.IRepository;
using TBlog.Model;
using TBlog.Common;
namespace TBlog.Repository
{
    public class RoleRepository : MongoRepository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(IMongoTransaction transaction) : base(transaction)
        {

        }

        public new Task AddEntity(RoleEntity entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = IdBuilder.CreateId();
            }
            return base.AddEntity(entity);
        }

        public Task<RoleEntity> GetByName(string name)
        {
            return GetSingle(c => name.Equals(c.Name));
        }

        public Task<List<RoleEntity>> GetByNames(IEnumerable<string> names)
        {
            return Get(c => names.Contains(c.Name));
        }
    }
}
