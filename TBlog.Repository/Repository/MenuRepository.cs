namespace TBlog.Repository
{
    public class MenuRepository : SugarRepository<MenuEntity>, IMenuRepository
    {
        public IEnumerable<MenuEntity> GetByRoleIds(IEnumerable<long> roleIds)
        {
            return DBQuery.ToList().Where(c => roleIds.Intersect(c.RoleIds).Any()).AsQueryable();
        }
    }
}
