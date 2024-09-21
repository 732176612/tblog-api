using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TBlog.Api
{
    public class TblogController : ControllerBase
    {
        #region Cookie
        /// <summary>
        /// 添加cookie缓存不设置过期时间
        /// </summary>
        [NonAction]
        public void AddCookie(string key, string value)
        {
            HttpContext.Response.Cookies.Append(key, value);
        }

        /// <summary>
        /// 添加cookie缓存设置过期时间
        /// </summary>
        [NonAction]
        public void AddCookie(string key, string value, int time)
        {
            HttpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddSeconds(time)
            });
        }

        /// <summary>
        /// 删除cookie缓存
        /// </summary>
        [NonAction]
        public void DeleteCookie(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 根据键获取对应的cookie
        /// </summary>
        [NonAction]
        public string GetCookie(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// 获取Token身份信息
        /// </summary>
        [NonAction]
        public TokenJwtInfoModel GetToken(bool isNull = false)
        {
            string token = GetCookie("token");
            if (!isNull && string.IsNullOrWhiteSpace(token))
            {
                throw new TBlogApiException("请先登陆!");
            }

            var tokenModel = AuthorizationHelper.SerializeJwt(token);
            if (!isNull && tokenModel == null)
            {
                throw new TBlogApiException("登陆授权已失效，请重新登陆!");
            }

            return tokenModel;
        }

        /// <summary>
        /// 设置Token身份信息
        /// </summary>
        [NonAction]
        public void SetToken(TokenJwtInfoModel tokenModel)
        {
            DeleteCookie("token");
            string jwtStr = AuthorizationHelper.GenerateJwtStr(tokenModel);
            AddCookie("token", jwtStr, ApiConfig.JwtBearer.ExpressTime);
        }
        #endregion
    }
}
