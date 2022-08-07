namespace TBlog.Model
{
    /// <summary>
    /// 文章
    /// </summary>
    public class ActicleDto : IDto
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CDate { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 封面海报
        /// </summary>
        public string PosterUrl { get; set; }

        /// <summary>
        /// 文章标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        public EnumActicleType ActicleType { get; set; }

        /// <summary>
        /// 发布形式
        /// </summary>
        public EnumActicleReleaseForm ReleaseForm { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public long LikeNum { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        public long ShareNum { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public long CollectNum { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public long LookNum { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CBlogName { get; set; }
    }
}
