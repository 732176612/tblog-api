namespace TBlog.Service
{
    /// <summary>
    /// 工作经历
    /// </summary>
    public interface ICompanyInfoService : IBaseService<CompanyInfoEntity>
    {
        public Task Save(IEnumerable<CompanyInfoDto> dtos, long cuserid);

        public Task<IEnumerable<CompanyInfoDto>> Get(long cuserid);
    }
}
