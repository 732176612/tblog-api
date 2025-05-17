namespace TBlog.Service
{
    /// <summary>
    /// 项目经历
    /// </summary>
    public interface IProjectInfoService:IBaseService<ProjectInfoEntity>
    {
        public Task Save(IEnumerable<ProjectInfoDto> dtos, long cuserid);

        public Task<IEnumerable<ProjectInfoDto>> Get(long cuserid);
    }
}
