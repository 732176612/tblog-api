namespace TBlog.Model
{
    /// <summary>
    /// 枚举模型
    /// </summary>
    [Serializable]
    public class EnumModel
    {
        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 枚举键值对
        /// </summary>
        public System.Collections.Generic.IEnumerable<KeyValueModel> EnumKeyValues { get; set; }
    }
}
