namespace TBlog.Model
{
    /// <summary>
    /// Token令牌
    /// </summary>
    public class TokenJwtInfoModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 博客名称
        /// </summary>
        public string BlogName { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        public IEnumerable<long> RoleIds { get; set; } = Enumerable.Empty<long>();

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

    }
}
