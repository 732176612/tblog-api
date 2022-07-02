namespace TBlog.Model
{
    /// <summary>
    /// 发布形式
    /// </summary>
    public enum EnumActicleReleaseForm
    {
        [Description("未设置")]
        None = 0,

        [Description("公共")]
        Public = 1,

        [Description("私密")]
        Private = 2,

        [Description("草稿")]
        Draft=3,
    }
}
