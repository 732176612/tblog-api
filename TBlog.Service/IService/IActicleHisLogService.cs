namespace TBlog.Service
{
    public interface IActicleHisLogService : IBaseService<ActicleHisLogEntity>
    {
        public Task<bool> AddLog(ActicleHisLogEntity entity);
    }
}