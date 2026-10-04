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
            var entities = await Repository.DBQuery.Where(c => c.CUserId == cuserid)
                .OrderBy(c => c.Sort).OrderBy(c => c.Id).ToListAsync();
            return entities.ToDto<SkillInfoDto, SkillInfoEntity>();
        }

        [Transaction]
        public async Task Save(IEnumerable<SkillInfoDto> dtos, long cuserid)
        {
            try
            {
                var entities = dtos.ToEntity<SkillInfoEntity, SkillInfoDto>().ToList();
                for (var index = 0; index < entities.Count; index++)
                {
                    var item = entities[index];
                    item.CUserId = cuserid;
                    item.Id = SnowFlakeSingle.instance.NextId();
                    item.Sort = index;
                    item.DisplayMode = item.DisplayMode == "text" ? "text" : "progress";
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
