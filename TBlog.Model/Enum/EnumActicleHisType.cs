namespace TBlog.Model
{
    /// <summary>
    /// 文章历史操作类型
    /// </summary>
    public enum EnumActicleHisType
    {
        /// <summary>
        /// 点赞
        /// </summary>
        [Description("点赞")]
        Like = 1,

        /// <summary>
        /// 查阅
        /// </summary>
        [Description("查阅")]
        Look = 2
    }
}
