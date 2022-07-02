using MongoDB.Driver;
using System;

namespace TBlog.IRepository
{
    public interface IMongoTransaction : ITransaction, IDisposable
    {
        IMongoClient GetDbClient();

        IClientSessionHandle GetSessionHandle();
    }
}
