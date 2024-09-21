using System.Collections.Generic;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IRepository
{
    public interface IActicleRepository : IMongoRepository<ActicleEntity>
    {
        Task<long> CountByUIdAndTitle(long userid, string title);

        Task<IEnumerable<string>> GetTagsByUseId(long userid, EnumActicleReleaseForm releaseForm);
    }
}
