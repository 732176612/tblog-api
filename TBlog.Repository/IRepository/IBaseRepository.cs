using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        
    }
}
