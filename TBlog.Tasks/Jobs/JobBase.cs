using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.IService;

namespace TBlog.Tasks
{
    public class JobBase
    {
        public ITaskQzService _tasksQzServices;
        /// <summary>
        /// 执行指定任务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        public async Task<string> ExecuteJob(IJobExecutionContext context, Func<Task> func)
        {
            //记录Job时间
            Stopwatch stopwatch = new();
            //JOBID
            int jobid = context.JobDetail.Key.Name.ToInt();
            //JOB组名
            string groupName = context.JobDetail.Key.Group;
            //日志
            string jobHistory = $"【{DateTime.Now:yyyy-MM-dd HH:mm:ss}】【执行开始】【Id：{jobid}，组别：{groupName}】";
            try
            {
                stopwatch.Start();
                await func();//执行任务
                stopwatch.Stop();
                jobHistory += $"，【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】【执行成功】";
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                //true  是立即重新执行任务 
                e2.RefireImmediately = true;
                jobHistory += $"，【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】【执行失败:{ex.Message}】";
            }
            finally
            {
                jobHistory += $"，【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}】【执行结束】(耗时:{Math.Round(stopwatch.Elapsed.TotalSeconds, 3)}秒)";
                if (_tasksQzServices != null)
                {
                    var model = await _tasksQzServices.GetById(jobid);
                    if (model != null)
                    {
                        model.RunTimes += 1;
                        var separator = "<br>";
                        // 这里注意数据库字段的长度问题，超过限制，会造成数据库remark不更新问题。
                        model.Remark =
                            $"{jobHistory}{separator}" + string.Join(separator, StringHelper.GetTopDataBySeparator(model.Remark, separator, 9));
                        await _tasksQzServices.Update(model);
                    }
                }
            }

            Console.Out.WriteLine(jobHistory);
            return jobHistory;
        }
    }

}
