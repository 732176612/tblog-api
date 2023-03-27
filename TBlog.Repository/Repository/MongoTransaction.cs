using Microsoft.Extensions.Logging;
using System;
using TBlog.IRepository;
using MongoDB.Driver;

namespace TBlog.Repository
{
    /// <summary>
    /// 事务处理
    /// </summary>
    public class MongoTransaction : IMongoTransaction
    {
        private readonly IMongoClient _MongoClient;
        private IClientSessionHandle _ClientSession;
        private readonly ILogger<MongoTransaction> _logger;

        public MongoTransaction(IMongoClient mongoClient, ILogger<MongoTransaction> logger)
        {
            _MongoClient = mongoClient;
            _logger = logger;
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public IMongoClient GetDbClient()
        {
            return _MongoClient;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            if (_ClientSession == null)
                _ClientSession = _MongoClient.StartSession();
            _ClientSession.StartTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            if (_ClientSession != null)
            {
                try
                {
                    _ClientSession.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _ClientSession.AbortTransaction();
                    _logger.LogError($"{ex.Message}\r\n{ex.InnerException}");
                }
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            if (_ClientSession != null)
                _ClientSession.AbortTransaction();
        }

        public void Dispose()
        {
            if (_ClientSession != null)
                _ClientSession.Dispose();
        }

        public IClientSessionHandle GetSessionHandle()
        {
            return _ClientSession;
        }
    }
}
