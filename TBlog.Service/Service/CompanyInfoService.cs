using SqlSugar;

namespace TBlog.Service
{
    public class CompanyInfoService : BaseService<CompanyInfoEntity>, ICompanyInfoService
    {
        private readonly IMongoRepository<CompanyInfoEntity> Repository;
        public CompanyInfoService(IMongoRepository<CompanyInfoEntity> companyInfoRepository)
        {
            Repository = companyInfoRepository;
        }

        public async Task<IEnumerable<CompanyInfoDto>> Get(long cuserid)
        {
            return (await Repository.Get(c => c.CUserId == cuserid)).ToDto<CompanyInfoDto, CompanyInfoEntity>();
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
