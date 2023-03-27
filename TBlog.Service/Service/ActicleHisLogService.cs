namespace TBlog.Service
{
    public class ActicleHisLogService : BaseService<ActicleHisLogEntity>, IActicleHisLogService
    {
        private readonly IActicleHisLogRepository _ActicleHisLogRepository;
        public ActicleHisLogService(IActicleHisLogRepository acticleHisLogRepository) : base(acticleHisLogRepository)
        {
            _ActicleHisLogRepository = acticleHisLogRepository;
        }
        public async Task<bool> AddLog(ActicleHisLogEntity entity)
        {
            if (await _ActicleHisLogRepository.Count(c => c.ActicleId == entity.ActicleId && c.HisType == entity.HisType && (c.CUserId == entity.CUserId || c.IpAddress == entity.IpAddress)) != 0)
            {
                return false;
            }
            else
            {
                await _ActicleHisLogRepository.AddEntity(entity);
                return true;
            }
        }
    }
}
