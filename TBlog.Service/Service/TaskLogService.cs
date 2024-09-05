namespace TBlog.Service
{
    public class TaskLogService : BaseService<TaskLogEntity>, ITaskLogService
    {
        readonly ISugarRepository<TaskLogEntity> Repository;

        public TaskLogService(ISugarRepository<TaskLogEntity> baseRepository) 
        {
            Repository = baseRepository;
        }
    }
}
