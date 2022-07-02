using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using TBlog.IRepository;
namespace TBlog.Repository
{
    /// <summary>
    /// 事务处理
    /// </summary>
    public class SqlSugarTransaction : ISqlSugarTransaction
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<SqlSugarTransaction> _logger;

        public SqlSugarTransaction(ISqlSugarClient sqlSugarClient, ILogger<SqlSugarTransaction> logger)
        {
            _sqlSugarClient = sqlSugarClient;
            _logger = logger;
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetDbClient()
        {
            return _sqlSugarClient as SqlSugarClient;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            GetDbClient().BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            try
            {
                GetDbClient().CommitTran();
            }
            catch (Exception ex)
            {
                GetDbClient().RollbackTran();
                _logger.LogError($"{ex.Message}\r\n{ex.InnerException}");
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }

        public void Dispose()
        {
            GetDbClient().Dispose();
        }
    }
}
