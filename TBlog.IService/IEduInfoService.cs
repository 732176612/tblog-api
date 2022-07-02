using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IService
{
    /// <summary>
    /// 教育经历
    /// </summary>
    public interface IEduInfoService:IBaseService<EduInfoEntity>
    {
        public Task Save(IEnumerable<EduInfoDto> dtos, long cuserid);

        public Task<IEnumerable<EduInfoDto>> Get(long cuserid);
    }
}
