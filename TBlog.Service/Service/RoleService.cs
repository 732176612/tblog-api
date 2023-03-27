namespace TBlog.Service
{
    public class RoleService : BaseService<RoleEntity>, IRoleService
    {
        readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository) : base(roleRepository)
        {
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RoleEntity GetByName(string name)
        {
            return _roleRepository.GetByNames(new string[] { name }).Result.FirstOrDefault();
        }

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<RoleEntity> GetByName(IEnumerable<string> names)
        {
            return _roleRepository.GetByNames(names).Result;
        }
    }
}
