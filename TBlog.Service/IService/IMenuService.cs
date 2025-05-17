namespace TBlog.Service
{
    public interface IMenuService:IBaseService<MenuEntity>
    {
        /// <summary>
        /// 根据角色Id获取菜单
        /// </summary>
        public Task<IEnumerable<MenuDto>> GetByRoleIds(IEnumerable<long> roleIds);
    }
}
