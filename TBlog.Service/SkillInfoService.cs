namespace TBlog.Service
{
    public class SkillInfoService : BaseService<SkillInfoEntity>, ISkillInfoService
    {
        readonly IMongoRepository<SkillInfoEntity> _SkillInfoRepository;
        public SkillInfoService(IMongoRepository<SkillInfoEntity> skillInfoRepository) : base(skillInfoRepository)
        {
            _SkillInfoRepository = skillInfoRepository;
        }

        public async Task<IEnumerable<SkillInfoDto>> Get(long cuserid)
        {
            return (await Get(c => c.CUserId == cuserid)).ToDto<SkillInfoDto, SkillInfoEntity>();
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
                    item.Id = IdBuilder.CreateId();
                }
                await _SkillInfoRepository.Delete(c => c.CUserId == cuserid);
                await _SkillInfoRepository.AddEntities(entities.ToList());
            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.ToString());
            }
        }
    }
}
