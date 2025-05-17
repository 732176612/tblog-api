using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Common
{
    /// <summary>
    /// 常量帮助类
    /// </summary>
    public class ConstHelper
    {
        /// <summary>
        /// 用户名称最长长度
        /// </summary>
        public const int UserNameLength = 20;

        /// <summary>
        /// 手机号码正则表达式
        /// </summary>
        public const string PhoneRegex = @"^1\d{10}$";

        /// <summary>
        /// 邮箱正则表达式
        /// </summary>
        public const string MailRegex = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        /// <summary>
        /// 用户密码正则表达式
        /// </summary>
        public const string PassWordRegex = @"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{8,16}$";

        /// <summary>
        /// 博客名称正则表达式
        /// </summary>
        public const string BlogNameRegex = "^[0-9a-zA-Z_]{1,12}$";

        /// <summary>
        /// 系统管理员
        /// </summary>
        public const string SystemRole = "System";

        /// <summary>
        /// 普通用户
        /// </summary>
        public const string UserRole = "User";

        /// <summary>
        /// 主库ConfigID
        /// </summary>
        public const string MainDBConfig = "Main";
    }
}
