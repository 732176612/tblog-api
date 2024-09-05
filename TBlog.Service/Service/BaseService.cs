global using System;
global using System.Collections.Generic;
global using System.Linq;
global using TBlog.IService;
global using TBlog.Model;
global using TBlog.IRepository;
global using System.Threading.Tasks;
global using System.Linq.Expressions;
global using TBlog.Common;
global using Nest;
global using TBlog.Extensions;

namespace TBlog.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        public BaseService() { }
    }
}
