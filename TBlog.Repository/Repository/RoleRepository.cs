namespace TBlog.Repository
{
    public class RoleRepository : SugarRepository<RoleEntity>, IRoleRepository
    {
        public Task<RoleEntity> GetByName(string name)
        {
            return DBQuery.FirstAsync(c => name.Equals(c.Name));
        }

        public Task<List<RoleEntity>> GetByNames(IEnumerable<string> names)
        {
            return DBQuery.Where(c => names.Contains(c.Name)).ToListAsync();
        }
    }
}
