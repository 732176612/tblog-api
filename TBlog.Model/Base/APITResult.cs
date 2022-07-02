namespace TBlog.Model
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    [Serializable]
    public class APITResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; } = "";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Data { get; set; }

        public bool IsSuccess { get { return Status == 200; } set { if (value) Status = 200; else Status = 500; } }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static APITResult<T> Success(string msg)
        {
            return Message(true, msg, default);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="response">消息</param>
        /// <returns></returns>
        public static APITResult<T> Success(T response)
        {
            return Message(true, "获取成功", response);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static APITResult<T> Success(string msg, T response)
        {
            return Message(true, msg, response);
        }

        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static APITResult<T> Fail(string msg="")
        {
            return Message(false, msg, default);
        }
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static APITResult<T> Fail(string msg, T response)
        {
            return Message(false, msg, response);
        }
        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="isSuccess">失败/成功</param>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static APITResult<T> Message(bool isSuccess, string msg, T response)
        {
            return new APITResult<T>() { Msg = msg, Data = response, IsSuccess = isSuccess };
        }
    }
}
