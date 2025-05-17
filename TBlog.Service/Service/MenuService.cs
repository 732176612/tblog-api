namespace TBlog.Service
{
    public class MenuService : SugarService<MenuEntity>, IMenuService
    {
        public async Task<IEnumerable<MenuDto>> GetByRoleIds(IEnumerable<long> roleIds)
        {
            var menus = await Repository.DBQuery.OrderBy(c => c.OrderSort).ToListAsync();
             menus.Where(c => (c.RoleIds?.Any() ?? false) == false || c.RoleIds.Intersect(roleIds).Any()).ToList();
            return menus.ToDto<MenuDto, MenuEntity>();
        }
    }
}
