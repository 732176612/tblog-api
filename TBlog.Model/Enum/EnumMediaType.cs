namespace TBlog.Model
{
    /// <summary>
    /// 媒体文件类型
    /// </summary>
    public enum EnumMediaType
    {
        [Description("文件")]
        File = 0,

        [Description("图像")]
        Image = 1,

        [Description("视频")]
        Video = 2,

        [Description("音频")]
        Audio = 3,
    }
}
