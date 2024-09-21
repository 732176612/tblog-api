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

        public Task<long> CountByUIdAndTitle(long userid, string title)
        {
            return Count(c => c.CUserId == userid && c.Title == title);
        }

        public async Task<IEnumerable<string>> GetTagsByUseId(long userid, EnumActicleReleaseForm releaseForm)
        {
            var result = await Collection.Aggregate().Match(c => c.CUserId == userid && c.ReleaseForm == releaseForm).Unwind((FieldDefinition<ActicleEntity, string[]>)"Tags").Group(new BsonDocument { { "_id", "$Tags" } }).ToListAsync();
            return result.Select(c => c.GetElement("_id").Value.ToString()).OrderBy(c => c);
        }
    }
}