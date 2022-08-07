namespace TBlog.Model
{
    /// <summary>
    /// 文章类型
    /// </summary>
    public enum EnumActicleType
    {
        [Description("未设置")]
        None = 0,

        [Description("原创")]
        Original = 1,

        [Description("转载")]
        Reprint = 2
    }
}
