namespace TBlog.Service
{
    public class TaskQzService:BaseService<TaskQzEntity>,ITaskQzService
    {
        public TaskQzService(ISugarRepository<TaskQzEntity> baseRepository):base(baseRepository)
        {
            base.baseRepository = baseRepository;
        }
    }
}
