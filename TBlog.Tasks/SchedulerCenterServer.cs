﻿using TBlog.Model;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using TBlog.Model;
namespace TBlog.Tasks
{
    /// <summary>
    /// 任务调度管理中心
    /// </summary>
    public class SchedulerCenterServer : ISchedulerCenter
    {
        private Task<IScheduler> _scheduler;
        private readonly IJobFactory _iocjobFactory;
        public SchedulerCenterServer(IJobFactory jobFactory)
        {
            _iocjobFactory = jobFactory;
            _scheduler = GetSchedulerAsync();
        }
        private Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler != null)
                return this._scheduler;
            else
            {
                // 从Factory中获取Scheduler实例
                NameValueCollection collection = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" },
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(collection);
                return _scheduler = factory.GetScheduler();
            }
        }

        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<APITResult<string>> StartScheduleAsync()
        {
            var result = new APITResult<string>();
            try
            {
                this._scheduler.Result.JobFactory = this._iocjobFactory;
                if (!this._scheduler.Result.IsStarted)
                {
                    //等待任务运行完成
                    await this._scheduler.Result.Start();
                    await Console.Out.WriteLineAsync("任务调度开启！");
                    result.Msg = $"任务调度开启成功";
                    return result;
                }
                else
                {
                    result.Status = 500;
                    result.Msg = $"任务调度已经开启";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<APITResult<string>> StopScheduleAsync()
        {
            var result = new APITResult<string>();
            try
            {
                if (!this._scheduler.Result.IsShutdown)
                {
                    //等待任务运行完成
                    await this._scheduler.Result.Shutdown();
                    await Console.Out.WriteLineAsync("任务调度停止！");
                    result.Msg = $"任务调度停止成功";
                    return result;
                }
                else
                {
                    result.Status = 500;
                    result.Msg = $"任务调度已经停止";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加一个计划任务（映射程序集指定IJob实现类）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        public async Task<APITResult<string>> AddScheduleJobAsync(TaskQzEntity tasksQz)
        {
            var result = new APITResult<string>();

            if (tasksQz != null)
            {
                try
                {
                    JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);
                    if (await _scheduler.Result.CheckExists(jobKey))
                    {
                        result.Status = 500;
                        result.Msg = $"该任务计划已经在执行:【{tasksQz.Name}】,请勿重复启动！";
                        return result;
                    }
                    #region 设置开始时间和结束时间

                    if (tasksQz.BeginTime == null)
                    {
                        tasksQz.BeginTime = DateTime.Now;
                    }
                    DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(tasksQz.BeginTime, 1);//设置开始时间
                    if (tasksQz.EndTime == null)
                    {
                        tasksQz.EndTime = DateTime.MaxValue.AddDays(-1);
                    }
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(tasksQz.EndTime, 1);//设置暂停时间

                    #endregion

                    #region 通过反射获取程序集类型和类   

                    Assembly assembly = Assembly.Load(new AssemblyName(tasksQz.AssemblyName));
                    Type jobType = assembly.GetType(tasksQz.AssemblyName + "." + tasksQz.ClassName);

                    #endregion
                    //判断任务调度是否开启
                    if (!_scheduler.Result.IsStarted)
                    {
                        await StartScheduleAsync();
                    }

                    //传入反射出来的执行程序集
                    IJobDetail job = new JobDetailImpl(tasksQz.Id.ToString(), tasksQz.JobGroup, jobType);
                    job.JobDataMap.Add("JobParam", tasksQz.JobParams);
                    ITrigger trigger;

                    #region 泛型传递
                    //IJobDetail job = JobBuilder.Create<T>()
                    //    .WithIdentity(sysSchedule.Name, sysSchedule.JobGroup)
                    //    .Build();
                    #endregion

                    if (tasksQz.Cron != null && CronExpression.IsValidExpression(tasksQz.Cron) && tasksQz.TriggerType > 0)
                    {
                        trigger = CreateCronTrigger(tasksQz);

                        ((CronTriggerImpl)trigger).MisfireInstruction = MisfireInstruction.CronTrigger.DoNothing;
                    }
                    else
                    {
                        trigger = CreateSimpleTrigger(tasksQz);
                    }

                    // 告诉Quartz使用我们的触发器来安排作业
                    await _scheduler.Result.ScheduleJob(job, trigger);
                    //await Task.Delay(TimeSpan.FromSeconds(120));
                    //await Console.Out.WriteLineAsync("关闭了调度器！");
                    //await _scheduler.Result.Shutdown();

                    result.Msg = $"【{tasksQz.Name}】成功";
                    return result;
                }
                catch (Exception ex)
                {
                    result.Status = 500;
                    result.Msg = $"任务计划异常:【{ex.Message}】";
                    return result;
                }
            }
            else
            {
                result.Status = 500;
                result.Msg = $"任务计划不存在:【{tasksQz?.Name}】";
                return result;
            }
        }

        /// <summary>
        /// 任务是否存在?
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsExistScheduleJobAsync(TaskQzEntity sysSchedule)
        {
            JobKey jobKey = new JobKey(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 暂停一个指定的计划任务
        /// </summary>
        /// <returns></returns>
        public async Task<APITResult<string>> StopScheduleJobAsync(TaskQzEntity sysSchedule)
        {
            var result = new APITResult<string>();
            try
            {
                JobKey jobKey = new JobKey(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.Status = 500;
                    result.Msg = $"未找到要暂停的任务:【{sysSchedule.Name}】";
                    return result;
                }
                else
                {
                    await this._scheduler.Result.DeleteJob(jobKey);

                    result.Msg = $"【{sysSchedule.Name}】成功";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 恢复指定的计划任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        public async Task<APITResult<string>> ResumeJob(TaskQzEntity sysSchedule)
        {
            var result = new APITResult<string>();
            try
            {
                if (!await _scheduler.Result.CheckExists(new JobKey(sysSchedule.Id.ToString(),
                                           sysSchedule.JobGroup)))
                {
                    result.Status = 500;
                    result.Msg = $"未找到要恢复的任务:【{sysSchedule.Name}】";
                    return result;
                }
                await this._scheduler.Result.ResumeJob(new JobKey(sysSchedule.Id.ToString(),
                                           sysSchedule.JobGroup));

                result.Msg = $"【{sysSchedule.Name}】成功";
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 暂停指定的计划任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        public async Task<APITResult<string>> PauseJob(TaskQzEntity sysSchedule)
        {
            var result = new APITResult<string>();
            try
            {
                JobKey jobKey = new JobKey(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.Status = 500;
                    result.Msg = $"未找到要暂停的任务:【{sysSchedule.Name}】";
                    return result;
                }
                await this._scheduler.Result.PauseJob(jobKey);

                result.Msg = $"【{sysSchedule.Name}】成功";
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region 状态状态帮助方法
        public async Task<List<QuartzTaskModel>> GetTaskStaus(TaskQzEntity sysSchedule)
        {

            var ls = new List<QuartzTaskModel>();
            var noTask = new List<QuartzTaskModel>{ new QuartzTaskModel {
                JobId = sysSchedule.Id.ToString(),
                JobGroup = sysSchedule.JobGroup,
                TriggerId = "",
                TriggerGroup = "",
                TriggerStatus = "不存在"
            } };
            JobKey jobKey = new JobKey(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
            IJobDetail job = await this._scheduler.Result.GetJobDetail(jobKey);
            if (job == null)
            {
                return noTask;
            }
            //info.Append(string.Format("任务ID:{0}\r\n任务名称:{1}\r\n", job.Key.Name, job.Description)); 
            var triggers = await this._scheduler.Result.GetTriggersOfJob(jobKey);
            if (triggers == null || triggers.Count == 0)
            {
                return noTask;
            }
            foreach (var trigger in triggers)
            {
                var triggerStaus = await this._scheduler.Result.GetTriggerState(trigger.Key);
                string state = GetTriggerState(triggerStaus.ToString());
                ls.Add(new QuartzTaskModel
                {
                    JobId = job.Key.Name,
                    JobGroup = job.Key.Group,
                    TriggerId = trigger.Key.Name,
                    TriggerGroup = trigger.Key.Group,
                    TriggerStatus = state
                });
                //info.Append(string.Format("触发器ID:{0}\r\n触发器名称:{1}\r\n状态:{2}\r\n", item.Key.Name, item.Description, state));

            }
            return ls;
        }
        public string GetTriggerState(string key)
        {
            string state = null;
            if (key != null)
                key = key.ToUpper();
            switch (key)
            {
                case "1":
                    state = "暂停";
                    break;
                case "2":
                    state = "完成";
                    break;
                case "3":
                    state = "出错";
                    break;
                case "4":
                    state = "阻塞";
                    break;
                case "0":
                    state = "正常";
                    break;
                case "-1":
                    state = "不存在";
                    break;
                case "BLOCKED":
                    state = "阻塞";
                    break;
                case "COMPLETE":
                    state = "完成";
                    break;
                case "ERROR":
                    state = "出错";
                    break;
                case "NONE":
                    state = "不存在";
                    break;
                case "NORMAL":
                    state = "正常";
                    break;
                case "PAUSED":
                    state = "暂停";
                    break;
            }
            return state;
        }
        #endregion
        #region 创建触发器帮助方法

        /// <summary>
        /// 创建SimpleTrigger触发器（简单触发器）
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <param name="starRunTime"></param>
        /// <param name="endRunTime"></param>
        /// <returns></returns>
        private static ITrigger CreateSimpleTrigger(TaskQzEntity sysSchedule)
        {
            if (sysSchedule.CycleRunTimes > 0)
            {
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(sysSchedule.Id.ToString(), sysSchedule.JobGroup)
                .StartAt(sysSchedule.BeginTime.Value)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(sysSchedule.IntervalSecond)
                    .WithRepeatCount(sysSchedule.CycleRunTimes - 1))
                .EndAt(sysSchedule.EndTime.Value)
                .Build();
                return trigger;
            }
            else
            {
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(sysSchedule.Id.ToString(), sysSchedule.JobGroup)
                .StartAt(sysSchedule.BeginTime.Value)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(sysSchedule.IntervalSecond)
                    .RepeatForever()
                )
                .EndAt(sysSchedule.EndTime.Value)
                .Build();
                return trigger;
            }
            // 触发作业立即运行，然后每10秒重复一次，无限循环

        }
        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static ITrigger CreateCronTrigger(TaskQzEntity sysSchedule)
        {
            // 作业触发器
            return TriggerBuilder.Create()
                   .WithIdentity(sysSchedule.Id.ToString(), sysSchedule.JobGroup)
                   .StartAt(sysSchedule.BeginTime.Value)//开始时间
                   .EndAt(sysSchedule.EndTime.Value)//结束数据
                   .WithCronSchedule(sysSchedule.Cron)//指定cron表达式
                   .ForJob(sysSchedule.Id.ToString(), sysSchedule.JobGroup)//作业名称
                   .Build();
        }
        #endregion

    }
}
