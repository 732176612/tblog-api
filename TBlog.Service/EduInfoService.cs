namespace TBlog.Service
{
    public class EduInfoService : BaseService<EduInfoEntity>, IEduInfoService
    {
        readonly IMongoRepository<EduInfoEntity> _EduInfoRepository;
        public EduInfoService(IMongoRepository<EduInfoEntity> eduInfoRepository):base(eduInfoRepository)
        {
            _EduInfoRepository = eduInfoRepository;
        }

        public async Task<IEnumerable<EduInfoDto>> Get(long cuserid)
        {
            return (await Get(c => c.CUserId == cuserid)).ToDto<EduInfoDto, EduInfoEntity>();
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
                    item.Id = IdBuilder.CreateId();
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
