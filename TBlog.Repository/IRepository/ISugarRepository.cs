using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IRepository
{
    public interface ISugarRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        ISqlSugarClient BaseDB { get; }     
    }
}
