﻿using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.IService;
using TBlog.Model;

/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace TBlog.Tasks
{
    public class Job_OperateLog_Quartz : JobBase, IJob
    {
        private readonly ITaskLogService _taskLogServices;

        public Job_OperateLog_Quartz(ITaskLogService taskLogServices, ITaskQzService tasksQzServices)
        {
            _taskLogServices = taskLogServices;
            _tasksQzServices = tasksQzServices;
        }
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            var executeLog = await ExecuteJob(context, async () => await Run(context));
        }
        public async System.Threading.Tasks.Task Run(IJobExecutionContext context)
        {

            // 可以直接获取 JobDetail 的值
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            // 也可以通过数据库配置，获取传递过来的参数
            JobDataMap data = context.JobDetail.JobDataMap;

            List<LogInfo> excLogs = new List<LogInfo>();
            var exclogContent = LogLock.ReadLog(Path.Combine(LogLock.ContentRoot, "Log"), $"GlobalExceptionLogs_{DateTime.Now.ToString("yyyMMdd")}.log", Encoding.UTF8);

            if (!string.IsNullOrEmpty(exclogContent))
            {
                excLogs = exclogContent.Split("--------------------------------")
                             .Where(d => !string.IsNullOrEmpty(d) && d != "\n" && d != "\r\n")
                             .Select(d => new LogInfo
                             {
                                 Datetime = (d.Split("|")[0]).Split(',')[0].ToDateTime(),
                                 Content = d.Split("|")[1]?.Replace("\r\n", "<br>"),
                                 LogColor = "EXC",
                                 Import = 9,
                             }).ToList();
            }

            var filterDatetime = DateTime.Now.AddHours(-1);
            excLogs = excLogs.Where(d => d.Datetime >= filterDatetime).ToList();

            var operateLogs = new List<TaskLogEntity>() { };
            excLogs.ForEach(m =>
            {
                operateLogs.Add(new TaskLogEntity()
                {
                    LogTime = m.Datetime,
                    Description = m.Content,
                    IPAddress = m.IP,
                    UserId = 0,
                    IsDeleted = false,
                });
            });


            if (operateLogs.Count > 0)
            {
                await _taskLogServices.AddEntities(operateLogs);
            }
        }
    }
}
