using TBlog.Model;
using TBlog.IRepository;
using TBlog.Common;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TBlog.Repository
{
    public class ActicleHisLogRepository : MongoRepository<ActicleHisLogEntity>, IActicleHisLogRepository
    {
        public ActicleHisLogRepository(IMongoTransaction transaction) : base(transaction)
        {

        }

        public async Task<long> CountByActicleIdAndHisType(long id, EnumActicleHisType hisType)
        {
            return await Count(c => c.ActicleId == id && c.HisType == hisType);
        }
    }
}