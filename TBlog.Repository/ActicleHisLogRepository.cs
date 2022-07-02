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

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new Task AddEntity(ActicleHisLogEntity entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = IdBuilder.CreateId();
            }
            return base.AddEntity(entity);
        }

        public async Task<long> CountByActicleIdAndHisType(long id, EnumActicleHisType hisType)
        {
            return await Count(c => c.ActicleId == id && c.HisType == hisType);
        }
    }
}