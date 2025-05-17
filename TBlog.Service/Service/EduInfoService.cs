namespace TBlog.Service
{
    public class EduInfoService : BaseService<EduInfoEntity>, IEduInfoService
    {
        readonly ISugarRepository<EduInfoEntity> _EduInfoRepository;
        public EduInfoService(ISugarRepository<EduInfoEntity> eduInfoRepository) 
        {
            _EduInfoRepository = eduInfoRepository;
        }

        public async Task<IEnumerable<EduInfoDto>> Get(long cuserid)
        {
            var entities = await _EduInfoRepository.DBQuery.Where(c => c.CUserId == cuserid).ToListAsync();
            return entities.ToDto<EduInfoDto, EduInfoEntity>();
        }

        [Transaction]
        public async Task Save(IEnumerable<EduInfoDto> dtos, long cuserid)
        {
            try
            {
                var entities = dtos.ToEntity<EduInfoEntity, EduInfoDto>();
                foreach (var item in entities)
                {
                    item.CUserId = cuserid;
                    item.Id = SnowFlakeSingle.instance.NextId();
                }
                await _EduInfoRepository.Delete(c => c.CUserId == cuserid);
                await _EduInfoRepository.AddEntities(entities.ToList());

            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.ToString());
            }
        }
    }
}
