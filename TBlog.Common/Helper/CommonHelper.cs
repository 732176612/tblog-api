using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TBlog.Common
{
    public static class CommonHelper
    {
        /// <summary>
        /// 异常将重新执行
        /// </summary>
        /// <param name="retryTime">重试次数</param>
        public static async Task ExceptionRetry(this Func<bool> action, int retryTime = 5)
        {
            bool isSuccess = false;
            while (!isSuccess)
            {
                try
                {
                    isSuccess = action();
                }
                catch
                {
                    retryTime--;
                    if (retryTime <= 0)
                    {
                        isSuccess = true;
                    }
                    if (!isSuccess)
                    {
                        await Task.Delay(100);
                    }
                }
            }
        }
    }
}
