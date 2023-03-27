namespace TBlog.Service
{
    public class CompanyInfoService : BaseService<CompanyInfoEntity>, ICompanyInfoService
    {
        public CompanyInfoService(IMongoRepository<CompanyInfoEntity> companyInfoRepository) : base(companyInfoRepository)
        {
            baseRepository = companyInfoRepository;
        }

        public async Task<IEnumerable<CompanyInfoDto>> Get(long cuserid)
        {
            return (await Get(c => c.CUserId == cuserid)).ToDto<CompanyInfoDto, CompanyInfoEntity>();
        }

        [Transaction]
        public async Task Save(IEnumerable<CompanyInfoDto> dtos, long cuserid)
        {
            try
            {
                var entities = dtos.ToEntity<CompanyInfoEntity, CompanyInfoDto>();
                foreach (var item in entities)
                {
                    item.CUserId = cuserid;
                    item.Id = IdBuilder.CreateId();
                }
                await baseRepository.Delete(c => c.CUserId == cuserid);
                await baseRepository.AddEntities(entities.ToList());
            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.ToString());
            }
        }
    }
}
