global using Nest;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using TBlog.Common;
global using TBlog.Extensions;
global using TBlog.IRepository;
global using TBlog.IService;
global using TBlog.Model;

namespace TBlog.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {

    }
}
