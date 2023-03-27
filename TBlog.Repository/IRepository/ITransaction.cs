using SqlSugar;

namespace TBlog.IRepository
{
    public interface ITransaction
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 回滚
        /// </summary>
        void RollbackTran();
    }
}
