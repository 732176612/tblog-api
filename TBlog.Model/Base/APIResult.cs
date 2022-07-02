namespace TBlog.Model
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class APIResult
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回成功
        /// </summary>
        public static APIResult Success()
        {
            return Message(true);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static APIResult Success(string msg)
        {
            return Message(true, msg);
        }

        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static APIResult Fail(string msg="")
        {
            return Message(false, msg);
        }

        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="success">失败/成功</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static APIResult Message(bool success, string msg="")
        {
            return new APIResult() { Msg = msg, Status = success ? 200 : 400 };
        }

        public static APIResult Return401()
        {
            return new APIResult() { Msg = "很抱歉，您无权访问该接口，请确保已经登录!", Status = 401 };
        }

        public static APIResult Return403()
        {
            return new APIResult() { Msg = "很抱歉，您的访问权限等级不够!", Status = 403 };
        }

        public static APIResult Return404()
        {
            return new APIResult() { Msg = "无此链接", Status = 404 };
        }
    }
}
