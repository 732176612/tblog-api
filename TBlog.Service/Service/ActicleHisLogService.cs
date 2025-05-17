namespace TBlog.Service
{
    public class ActicleHisLogService : SugarService<ActicleHisLogEntity>, IActicleHisLogService
    {
        public async Task<bool> AddLog(ActicleHisLogEntity entity)
        {
            var isExist = await DbScoped.SugarScope.Queryable<ActicleHisLogEntity>()
                .AnyAsync(c => c.ActicleId == entity.ActicleId && c.HisType == entity.HisType && c.CUserId == entity.CUserId);
            if (isExist) return false;
            await Repository.AddEntity(entity);
            return true;
        }
    }
}
