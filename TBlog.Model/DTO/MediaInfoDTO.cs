namespace TBlog.Model
{
    /// <summary>
    /// 文件上传信息
    /// </summary>
    public class MediaInfoDto : IDto
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 云链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 上传者ID
        /// </summary>
        public long CUserId { get; set; }

        /// <summary>
        /// 媒体文件类型
        /// </summary>
        public EnumMediaType MediaType { get; set; }
    }
}
