using TBlog.Model;
using TBlog.IRepository;
using TBlog.Common;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TBlog.Repository
{
    public class ActicleRepository : MongoRepository<ActicleEntity>, IActicleRepository
    {
        public ActicleRepository(IMongoTransaction transaction) : base(transaction)
        {

        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<long> AddEntity(ActicleEntity entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = IdBuilder.CreateId();
            }
            await base.AddEntity(entity);
            return entity.Id;
        }

        public Task<long> CountByUIdAndTitle(long userid, string title)
        {
            return base.Count(c => c.CUserId == userid && c.Title == title);
        }

        public async Task<IEnumerable<string>> GetTagsByUseId(long userid, EnumActicleReleaseForm releaseForm)
        {
            var result = await Collection.Aggregate().Match(c => c.CUserId == userid && c.ReleaseForm == releaseForm).Unwind((FieldDefinition<ActicleEntity, string[]>)"Tags").Group(new BsonDocument { { "_id", "$Tags" } }).ToListAsync();
            return result.Select(c => c.GetElement("_id").Value.ToString()).OrderBy(c => c);
        }
    }
}