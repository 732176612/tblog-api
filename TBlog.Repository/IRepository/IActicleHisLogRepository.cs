using System.Collections.Generic;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IRepository
{
    public interface IActicleHisLogRepository : IMongoRepository<ActicleHisLogEntity>
    {
        public new Task AddEntity(ActicleHisLogEntity entity);

        public Task<long> CountByActicleIdAndHisType(long id,EnumActicleHisType hisType);
    }
}
