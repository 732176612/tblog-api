namespace TBlog.Service
{
    public class TaskLogService:BaseService<TaskLogEntity>,ITaskLogService
    {
        public TaskLogService(ISugarRepository<TaskLogEntity> baseRepository):base(baseRepository)
        {
            base.baseRepository = baseRepository;
        }
    }
}
