namespace TBlog.Model
{
    /// <summary>
    /// Token信息
    /// </summary>
    public class TBlogTokenModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 令牌
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 有效时间
        /// </summary>
        public double expires_in { get; set; }

        /// <summary>
        /// 令牌类型
        /// </summary>
        public string token_type { get; set; }
    }
}
