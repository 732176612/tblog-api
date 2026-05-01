using TBlog.IRepository;

namespace TBlog.Service
{
    public class HttpLogService : BaseService<HttpLogEntity>, IHttpLogService
    {
        readonly IMongoRepository<HttpLogEntity> _httpLogRepository;
        public HttpLogService(IMongoRepository<HttpLogEntity> httpLogRepository)
        {
            _httpLogRepository = httpLogRepository;
        }
    }
}
