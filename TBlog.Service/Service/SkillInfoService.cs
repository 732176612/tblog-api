namespace TBlog.Service
{
    public class SkillInfoService : BaseService<SkillInfoEntity>, ISkillInfoService
    {
        readonly ISugarRepository<SkillInfoEntity> Repository;
        public SkillInfoService(ISugarRepository<SkillInfoEntity> skillInfoRepository)
        {
            Repository = skillInfoRepository;
        }

        public async Task<IEnumerable<SkillInfoDto>> Get(long cuserid)
        {
            var entities = await Repository.DBQuery.Where(c => c.CUserId == cuserid).ToListAsync();
            return entities.ToDto<SkillInfoDto, SkillInfoEntity>();
        }

        [Transaction]
        public async Task Save(IEnumerable<SkillInfoDto> dtos, long cuserid)
        {
            try
            {
                var entities = dtos.ToEntity<SkillInfoEntity, SkillInfoDto>();
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
