namespace TBlog.Service
{
    public class MenuService : BaseService<MenuEntity>, IMenuService
    {
        readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository):base(menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public IEnumerable<MenuDto> GetByRoleIds(IEnumerable<long> roleIds)
        {
            var menuEntitys = _menuRepository.GetByRoleIds(roleIds).Where(c => c.Enabled).OrderBy(c => c.OrderSort).AsEnumerable();
            return menuEntitys.ToDto<MenuDto, MenuEntity>();
        }
    }
}
