using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.Model;
using Newtonsoft.Json;
namespace TBlog.Extensions
{
    public class AuthorizationHelper
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static string GenerateJwtStr(TokenJwtInfoModel tokenModel)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenModel.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(ApiConfig.JwtBearer.ExpressTime)).ToUnixTimeSeconds()}"),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(ApiConfig.JwtBearer.ExpressTime).ToString()),
                new Claim(JwtRegisteredClaimNames.Iss,ApiConfig.JwtBearer.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud,ApiConfig.JwtBearer.Audience),
            };

            if (!string.IsNullOrEmpty(tokenModel.UserName))
            {
                claims.Add(new Claim("UserName", tokenModel.UserName));
            }

            if (!string.IsNullOrEmpty(tokenModel.BlogName))
            {
                claims.Add(new Claim("BlogName", tokenModel.BlogName));
            }

            if (!string.IsNullOrEmpty(tokenModel.RoleName))
                claims.AddRange(tokenModel.RoleName.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));
            if (tokenModel.RoleIds.Any())
                claims.AddRange(tokenModel.RoleIds.Select(s => new Claim("RoleIds", s.ToString())));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiConfig.JwtBearer.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: ApiConfig.JwtBearer.Issuer,
                claims: claims,
                signingCredentials: creds);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// 解析Jwt
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenJwtInfoModel SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            TokenJwtInfoModel tokenModelJwt = new TokenJwtInfoModel();
            if (jwtStr.IsNotEmptyOrNull() && jwtHandler.CanReadToken(jwtStr))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out object roleName);
                jwtToken.Payload.TryGetValue("RoleIds", out object roleIds);
                jwtToken.Payload.TryGetValue("BlogName", out object blogName);
                jwtToken.Payload.TryGetValue("UserName", out object userName);
                tokenModelJwt = new TokenJwtInfoModel
                {
                    UserId = (jwtToken.Id).ToLong(),
                    UserName = userName != null ? userName.ObjToString() : "",
                    RoleName = roleName != null ? roleName.ObjToString() : "",
                    BlogName = blogName != null ? blogName.ObjToString() : "",
                };
                tokenModelJwt.RoleIds = Enumerable.Empty<long>();
                if (roleIds != null && !string.IsNullOrEmpty(roleIds.ToString()))
                {
                    if (roleIds.ToString().Contains(','))
                    {
                        tokenModelJwt.RoleIds = JsonConvert.DeserializeObject<long[]>(roleIds.ObjToString());
                    }
                    else
                    {
                        tokenModelJwt.RoleIds = new long[] { long.Parse(roleIds.ToString()) };
                    }
                }
            }
            return tokenModelJwt;
        }

        /// <summary>
        /// 获取基于JWT的Token
        /// </summary>
        /// <param name="claims">需要在登陆的时候配置</param>
        /// <param name="requirement">在startup中定义的参数</param>
        /// <returns></returns>
        public static TBlogTokenModel GenerateToken(Claim[] claims, AuthorizationRequirement requirement)
        {
            var jwt = new JwtSecurityToken(
                issuer: requirement.Issuer,
                audience: requirement.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.Add(requirement.Expiration),
                signingCredentials: requirement.SigningCredentials
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var responseJson = new TBlogTokenModel
            {
                success = true,
                token = encodedJwt,
                expires_in = requirement.Expiration.TotalSeconds,
                token_type = "Bearer"
            };
            return responseJson;
        }
    }
}
