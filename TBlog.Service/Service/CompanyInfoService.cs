namespace TBlog.Service
{
    public class CompanyInfoService : SugarService<CompanyInfoEntity>, ICompanyInfoService
    {
        public async Task<IEnumerable<CompanyInfoDto>> Get(long cuserid)
        {
            var entities = await Repository.DBQuery.Where(c => c.CUserId == cuserid).ToListAsync();
            return entities.ToDto<CompanyInfoDto, CompanyInfoEntity>();
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
                await Repository.DBDelete.Where(c => c.CUserId == cuserid).ExecuteCommandAsync();
                await Repository.AddEntities(entities.ToList());
            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.ToString());
            }
        }
    }
}
