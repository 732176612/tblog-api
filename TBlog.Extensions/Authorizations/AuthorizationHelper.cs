using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TBlog.Common;
using TBlog.Model;
using Newtonsoft.Json;
namespace TBlog.Extensions
{
    public class AuthorizationHelper
    {
        public static SymmetricSecurityKey SignKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiConfig.JwtBearer.Secret));

        public static SigningCredentials SigningCredentials = new SigningCredentials(SignKey, SecurityAlgorithms.HmacSha256);

        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
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
            {
                claims.AddRange(tokenModel.RoleName.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));
            }

            if (tokenModel.RoleIds.Any())
            {
                claims.AddRange(tokenModel.RoleIds.Select(s => new Claim("RoleIds", s.ToString())));
            }

            var jwt = new JwtSecurityToken(
                issuer: ApiConfig.JwtBearer.Issuer,
                audience: ApiConfig.JwtBearer.Audience,
                claims: claims,
                signingCredentials: SigningCredentials);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// 解析Jwt
        /// </summary>
        public static TokenJwtInfoModel SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            TokenJwtInfoModel tokenModelJwt = new TokenJwtInfoModel();
            if (jwtStr.IsNotEmptyOrNull() && jwtHandler.CanReadToken(jwtStr))
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = ApiConfig.JwtBearer.Issuer,
                    ValidAudience = ApiConfig.JwtBearer.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiConfig.JwtBearer.Secret)),
                    ClockSkew = TimeSpan.FromSeconds(30), //过期时间容错值，解决服务器端时间不同步问题（秒）
                    RequireExpirationTime = true,
                };
                SecurityToken validatedToken;
                try
                {
                    jwtHandler.ValidateToken(jwtStr, validationParameters, out validatedToken);
                }
                catch
                {
                    return null;
                }
                JwtSecurityToken jwtToken = validatedToken as JwtSecurityToken;
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
                        tokenModelJwt.RoleIds = [long.Parse(roleIds.ToString())];
                    }
                }
            }
            return tokenModelJwt;
        }

        /// <summary>
        /// 获取基于JWT的Token
        /// </summary>
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
