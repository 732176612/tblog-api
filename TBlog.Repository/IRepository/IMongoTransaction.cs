using MongoDB.Driver;
namespace TBlog.IRepository
{
    public interface IMongoTransaction : ITransaction, IDisposable
    {
        IMongoClient GetDbClient();

        IClientSessionHandle GetSessionHandle();
    }
}
