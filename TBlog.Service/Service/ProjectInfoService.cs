namespace TBlog.Service
{
    public class ProjectInfoService : BaseService<ProjectInfoEntity>, IProjectInfoService
    {
        readonly ISugarRepository<ProjectInfoEntity> Repository;
        public ProjectInfoService(ISugarRepository<ProjectInfoEntity> projectInfoRepository)
        {
            Repository = projectInfoRepository;
        }

        public async Task<IEnumerable<ProjectInfoDto>> Get(long cuserid)
        {
            var entities = await Repository.DBQuery.Where(c => c.CUserId == cuserid).ToListAsync();
            if (entities == null || entities.Any() == false) return Enumerable.Empty<ProjectInfoDto>();
            return entities.ToDto<ProjectInfoDto, ProjectInfoEntity>();
        }

        [Transaction]
        public async Task Save(IEnumerable<ProjectInfoDto> dtos, long cuserid)
        {
            try
            {
                var entities = dtos.ToEntity<ProjectInfoEntity, ProjectInfoDto>();
                foreach (var item in entities)
                {
                    item.CUserId = cuserid;
                    item.Id = SnowFlakeSingle.instance.NextId();
                }
                await Repository.Delete(c => c.CUserId == cuserid);
                await Repository.AddEntities(entities.ToList());
            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.ToString());
            }
        }
    }
}
