namespace TBlog.Service
{
    public class MenuService : BaseService<MenuEntity>, IMenuService
    {
        readonly IMenuRepository Repository;

        public MenuService(IMenuRepository menuRepository)
        {
            Repository = menuRepository;
        }

        public IEnumerable<MenuDto> GetByRoleIds(IEnumerable<long> roleIds)
        {
            var menuEntitys = Repository.GetByRoleIds(roleIds).Where(c => c.Enabled).OrderBy(c => c.OrderSort).AsEnumerable();
            return menuEntitys.ToDto<MenuDto, MenuEntity>();
        }
    }
}
