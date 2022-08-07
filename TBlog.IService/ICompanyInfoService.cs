using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.IService
{
    /// <summary>
    /// 工作经历
    /// </summary>
    public interface ICompanyInfoService : IBaseService<CompanyInfoEntity>
    {
        public Task Save(IEnumerable<CompanyInfoDto> dtos, long cuserid);

        public Task<IEnumerable<CompanyInfoDto>> Get(long cuserid);
    }
}
