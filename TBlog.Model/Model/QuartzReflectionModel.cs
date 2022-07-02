namespace TBlog.Model
{
    /// <summary>
    /// 实现IJob的类
    /// </summary>
    public class QuartzReflectionModel:IModel
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
