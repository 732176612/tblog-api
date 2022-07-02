using SqlSugar;
using System;

namespace TBlog.IRepository
{
    public interface ISqlSugarTransaction:ITransaction, IDisposable
    {
        SqlSugarClient GetDbClient();
    }
}
