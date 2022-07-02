namespace TBlog.Service
{
    public class ProjectInfoService : BaseService<ProjectInfoEntity>, IProjectInfoService
    {
        public ProjectInfoService(IMongoRepository<ProjectInfoEntity> projectInfoRepository) : base(projectInfoRepository)
        {
            baseRepository = projectInfoRepository;
        }

        public async Task<IEnumerable<ProjectInfoDto>> Get(long cuserid)
        {
            var entities = await Get(c => c.CUserId == cuserid);
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
                    item.Id = IdBuilder.CreateId();
                }
                await Delete(c => c.CUserId == cuserid);
                await AddEntities(entities.ToList());
            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.ToString());
            }
        }
    }
}
