namespace TBlog.Model
{
    /// <summary>
    /// 通用分页信息类
    /// </summary>
    public class PageModel<T>
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public long PageIndex { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long PageCount => Math.Ceiling(TotalCount / (decimal)PageSize).ToInt();

        /// <summary>
        /// 数据总数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public long PageSize { set; get; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public IEnumerable<T> Data { get; set; } = new T[0];
    }

    public static class PageModelExtensions
    {
        public static PageModel<T1> AsPageModel<T1, T2>(this PageModel<T2> pageModel, IEnumerable<T1> data)
        {
            return new PageModel<T1>()
            {
                PageSize = pageModel.PageSize,
                Data = data,
                PageIndex = pageModel.PageIndex,
                TotalCount = pageModel.TotalCount,
            };
        }
    }
}
