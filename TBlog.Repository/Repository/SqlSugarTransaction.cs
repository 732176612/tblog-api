namespace TBlog.Repository
{
    /// <summary>
    /// 事务处理
    /// </summary>
    public class SqlSugarTransaction : ISqlSugarTransaction
    {
        private readonly ILogger<SqlSugarTransaction> _logger;

        public SqlSugarTransaction(ILogger<SqlSugarTransaction> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            DBHelper.DB.BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            try
            {
                DBHelper.DB.CommitTran();
            }
            catch (Exception ex)
            {
                DBHelper.DB.RollbackTran();
                _logger.LogError($"SQLSugar事务异常:[{ex.Message}\r\n{ex.InnerException}]");
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            DBHelper.DB.RollbackTran();
        }

        public void Dispose()
        {
            DBHelper.DB.Dispose();
        }
    }
}
