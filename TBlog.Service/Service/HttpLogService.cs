﻿namespace TBlog.Service
{
    public class HttpLogService : BaseService<HttpLogEntity>, IHttpLogService
    {
        readonly ISugarRepository<HttpLogEntity> _httpLogRepository;
        public HttpLogService(ISugarRepository<HttpLogEntity> httpLogRepository)
        {
            _httpLogRepository = httpLogRepository;
        }
    }
}
