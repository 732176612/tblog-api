namespace TBlog.Service
{
    public class RoleService : BaseService<RoleEntity>, IRoleService
    {
        readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        public RoleEntity GetByName(string name)
        {
            return _roleRepository.GetByNames(new string[] { name }).Result.FirstOrDefault();
        }

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        public List<RoleEntity> GetByName(IEnumerable<string> names)
        {
            return _roleRepository.GetByNames(names).Result;
        }
    }
}
