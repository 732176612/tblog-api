namespace TBlog.Model
{
    /// <summary>
    /// 树字典模型
    /// </summary>
    [Serializable]
    public  class TreeNodeModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public List<TreeNodeModel> Children { get; set; }
    }
}
