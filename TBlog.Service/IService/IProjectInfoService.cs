using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IService
{
    /// <summary>
    /// 项目经历
    /// </summary>
    public interface IProjectInfoService:IBaseService<ProjectInfoEntity>
    {
        public Task Save(IEnumerable<ProjectInfoDto> dtos, long cuserid);

        public Task<IEnumerable<ProjectInfoDto>> Get(long cuserid);
    }
}
