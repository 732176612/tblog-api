using System.Collections.Generic;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IService
{
    public interface IActicleHisLogService : IBaseService<ActicleHisLogEntity>
    {
        public Task<bool> AddLog(ActicleHisLogEntity entity);
    }
}