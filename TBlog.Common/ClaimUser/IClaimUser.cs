using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Common
{
    public interface IClaimUser
    {
        /// <summary>
        /// 姓名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ID
        /// </summary>
        int ID { get; }

        /// <summary>
        /// 是否通过了验证
        /// </summary>
        /// <returns></returns>
        bool IsAuthenticated();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Claim> GetClaimsIdentity();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClaimType"></param>
        /// <returns></returns>
        List<string> GetClaimValueByType(string ClaimType);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetToken();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClaimType"></param>
        /// <returns></returns>
        List<string> GetUserInfoFromToken(string ClaimType);
    }
}
