namespace TBlog.IRepository
{
    public interface IRoleRepository: ISugarRepository<RoleEntity>
    {
        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        public Task<RoleEntity> GetByName(string name);

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        public Task<List<RoleEntity>> GetByNames(IEnumerable<string> names);
    }
}
