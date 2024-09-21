namespace TBlog.IRepository
{
    public interface IMenuRepository: ISugarRepository<MenuEntity>
    {
        /// <summary>
        /// 根据角色Id获取菜单
        /// </summary>
        public IEnumerable<MenuEntity> GetByRoleIds(IEnumerable<long> roleIds);
    }
}
