namespace TBlog.Service
{
    public class CompanyInfoService : SugarService<CompanyInfoEntity>, ICompanyInfoService
    {
        public async Task<IEnumerable<CompanyInfoDto>> Get(long cuserid)
        {
            var entities = await Repository.DBQuery.Where(c => c.CUserId == cuserid)
                .OrderBy(c => c.Sort).OrderBy(c => c.Id).ToListAsync();
            return entities.ToDto<CompanyInfoDto, CompanyInfoEntity>();
        }

        [Transaction]
        public async Task Save(IEnumerable<CompanyInfoDto> dtos, long cuserid)
        {
            try
            {
                var entities = dtos.ToEntity<CompanyInfoEntity, CompanyInfoDto>().ToList();
                for (var index = 0; index < entities.Count; index++)
                {
                    var item = entities[index];
                    item.CUserId = cuserid;
                    item.Id = SnowFlakeSingle.instance.NextId();
                    item.Sort = index;
                }
                await Repository.DBDelete.Where(c => c.CUserId == cuserid).ExecuteCommandAsync();
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
