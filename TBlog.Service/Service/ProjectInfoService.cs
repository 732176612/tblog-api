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
            var entities = await Repository.DBQuery.Where(c => c.CUserId == cuserid)
                .OrderBy(c => c.Sort).OrderBy(c => c.Id).ToListAsync();
            if (entities == null || entities.Any() == false) return Enumerable.Empty<ProjectInfoDto>();
            return entities.ToDto<ProjectInfoDto, ProjectInfoEntity>();
        }

        [Transaction]
        public async Task Save(IEnumerable<ProjectInfoDto> dtos, long cuserid)
        {
            try
            {
                var entities = dtos.ToEntity<ProjectInfoEntity, ProjectInfoDto>().ToList();
                for (var index = 0; index < entities.Count; index++)
                {
                    var item = entities[index];
                    item.CUserId = cuserid;
                    item.Id = SnowFlakeSingle.instance.NextId();
                    item.Sort = index;
                }
                await Repository.Delete(c => c.CUserId == cuserid);
                if (entities.Count > 0)
                    await Repository.AddEntities(entities);
            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.ToString());
            }
        }
    }
}
