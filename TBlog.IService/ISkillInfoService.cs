using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IService
{
    /// <summary>
    /// 专业技能
    /// </summary>
    public interface ISkillInfoService:IBaseService<SkillInfoEntity>
    {
        public Task Save(IEnumerable<SkillInfoDto> dtos, long cuserid);

        public Task<IEnumerable<SkillInfoDto>> Get(long cuserid);
    }
}
