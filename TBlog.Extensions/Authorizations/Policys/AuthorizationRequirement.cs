using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Extensions
{
    /// <summary>
    /// 必要参数类
    /// </summary>
    public class AuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 认证授权类型
        /// </summary>
        public string ClaimType { internal get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }

        /// <summary>
        /// 签名验证实体
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        public AuthorizationRequirement(string claimType, string issuer, string audience, SigningCredentials signingCredentials, TimeSpan expiration)
        {
            ClaimType = claimType;
            Issuer = issuer;
            Audience = audience;
            Expiration = expiration;
            SigningCredentials = signingCredentials;
        }
    }
}
