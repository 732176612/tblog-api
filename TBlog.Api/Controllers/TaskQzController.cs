using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using TBlog.Common;
using TBlog.IService;
using TBlog.Tasks;
using TBlog.Model;
using TBlog.IRepository;

namespace TBlog.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(ConstHelper.SystemRole)]
    public class TasksQzController : ControllerBase
    {
        private readonly ITaskQzService _tasksQzServices;
        private readonly ISchedulerCenter _schedulerCenter;
        private readonly ISqlSugarTransaction _tranProcess;

        public TasksQzController(ITaskQzService tasksQzServices, ISchedulerCenter schedulerCenter, ISqlSugarTransaction tranProcess)
        {
            _tranProcess = tranProcess;
            _tasksQzServices = tasksQzServices;
            _schedulerCenter = schedulerCenter;
        }

        /// <summary>
        /// 分页获取
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        // GET: api/Buttons/5
        [HttpGet]
        public async Task<APITResult<PageModel<TaskQzEntity>>> Get(int page = 1, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }
            int intPageSize = 50;

            Expression<Func<TaskQzEntity, bool>> whereExpression = a => a.IsDeleted == false && (a.Name != null && a.Name.Contains(key));

            var data = await _tasksQzServices.GetPage(page, intPageSize, whereExpression);
            if (data.TotalCount > 0)
            {
                foreach (var item in data.Data)
                {
                    item.Triggers = await _schedulerCenter.GetTaskStaus(item);
                }
            }
            return APITResult<PageModel<TaskQzEntity>>.Message(data.TotalCount >= 0, "获取成功", data);
        }

        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APITResult<string>> Post([FromBody] TaskQzEntity tasksQz)
        {
            var data = new APITResult<string>();
            _tranProcess.BeginTran();
            await _tasksQzServices.AddEntity(tasksQz);
            var id = tasksQz.Id;
            data.IsSuccess = id > 0;
            try
            {
                if (data.IsSuccess)
                {
                    tasksQz.Id = id;
                    data.Data = id.ObjToString();
                    data.Msg = "添加成功";
                    if (tasksQz.IsStart)
                    {
                        //如果是启动自动
                        var ResuleModel = await _schedulerCenter.AddScheduleJobAsync(tasksQz);
                        data.IsSuccess = ResuleModel.IsSuccess;
                        if (ResuleModel.IsSuccess)
                        {
                            data.Msg = $"{data.Msg}=>启动成功=>{ResuleModel.Msg}";
                        }
                        else
                        {
                            data.Msg = $"{data.Msg}=>启动失败=>{ResuleModel.Msg}";
                        }
                    }
                }
                else
                {
                    data.Msg = "添加失败";

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (data.IsSuccess)
                    _tranProcess.CommitTran();
                else
                    _tranProcess.RollbackTran();
            }
            return data;
        }


        /// <summary>
        /// 修改计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<APITResult<string>> Put([FromBody] TaskQzEntity tasksQz)
        {
            var data = new APITResult<string>();
            if (tasksQz != null && tasksQz.Id > 0)
            {
                _tranProcess.BeginTran();
                data.IsSuccess = await _tasksQzServices.Update(tasksQz);
                try
                {
                    if (data.IsSuccess)
                    {
                        data.Msg = "修改成功";
                        data.Data = tasksQz?.Id.ObjToString();
                        if (tasksQz.IsStart)
                        {
                            var ResuleModelStop = await _schedulerCenter.StopScheduleJobAsync(tasksQz);
                            data.Msg = $"{data.Msg}=>停止:{ResuleModelStop.Msg}";
                            var ResuleModelStar = await _schedulerCenter.AddScheduleJobAsync(tasksQz);
                            data.IsSuccess = ResuleModelStar.IsSuccess;
                            data.Msg = $"{data.Msg}=>启动:{ResuleModelStar.Msg}";
                        }
                        else
                        {
                            var ResuleModelStop = await _schedulerCenter.StopScheduleJobAsync(tasksQz);
                            data.Msg = $"{data.Msg}=>停止:{ResuleModelStop.Msg}";
                        }
                    }
                    else
                    {
                        data.Msg = "修改失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.IsSuccess)
                        _tranProcess.CommitTran();
                    else
                        _tranProcess.RollbackTran();
                }
            }
            return data;
        }
        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<APITResult<string>> Delete(int jobId)
        {
            var data = new APITResult<string>();

            var model = await _tasksQzServices.GetById(jobId);
            if (model != null)
            {
                _tranProcess.BeginTran();
                data.IsSuccess = await _tasksQzServices.Delete(model);
                try
                {
                    data.Data = jobId.ObjToString();
                    if (data.IsSuccess)
                    {
                        data.Msg = "删除成功";
                        var ResuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                        data.Msg = $"{data.Msg}=>任务状态=>{ResuleModel.Msg}";
                    }
                    else
                    {
                        data.Msg = "删除失败";
                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.IsSuccess)
                        _tranProcess.CommitTran();
                    else
                        _tranProcess.RollbackTran();
                }
            }
            else
            {
                data.Msg = "任务不存在";
            }
            return data;

        }
        /// <summary>
        /// 启动计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APITResult<string>> StartJob(int jobId)
        {
            var data = new APITResult<string>();

            var model = await _tasksQzServices.GetById(jobId);
            if (model != null)
            {
                _tranProcess.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.IsSuccess = await _tasksQzServices.Update(model);
                    data.Data = jobId.ObjToString();
                    if (data.IsSuccess)
                    {
                        data.Msg = "更新成功";
                        var ResuleModel = await _schedulerCenter.AddScheduleJobAsync(model);
                        data.IsSuccess = ResuleModel.IsSuccess;
                        if (ResuleModel.IsSuccess)
                        {
                            data.Msg = $"{data.Msg}=>启动成功=>{ResuleModel.Msg}";

                        }
                        else
                        {
                            data.Msg = $"{data.Msg}=>启动失败=>{ResuleModel.Msg}";
                        }
                    }
                    else
                    {
                        data.Msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.IsSuccess)
                        _tranProcess.CommitTran();
                    else
                        _tranProcess.RollbackTran();
                }
            }
            else
            {
                data.Msg = "任务不存在";
            }
            return data;
        }
        /// <summary>
        /// 停止一个计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>        
        [HttpGet]
        public async Task<APITResult<string>> StopJob(int jobId)
        {
            var data = new APITResult<string>();

            var model = await _tasksQzServices.GetById(jobId);
            if (model != null)
            {
                model.IsStart = false;
                data.IsSuccess = await _tasksQzServices.Update(model);
                data.Data = jobId.ObjToString();
                if (data.IsSuccess)
                {
                    data.Msg = "更新成功";
                    var ResuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                    if (ResuleModel.IsSuccess)
                    {
                        data.Msg = $"{data.Msg}=>停止成功=>{ResuleModel.Msg}";
                    }
                    else
                    {
                        data.Msg = $"{data.Msg}=>停止失败=>{ResuleModel.Msg}";
                    }
                }
                else
                {
                    data.Msg = "更新失败";
                }
            }
            else
            {
                data.Msg = "任务不存在";
            }
            return data;
        }
        /// <summary>
        /// 暂停一个计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>        
        [HttpGet]
        public async Task<APITResult<string>> PauseJob(int jobId)
        {
            var data = new APITResult<string>();
            var model = await _tasksQzServices.GetById(jobId);
            if (model != null)
            {
                _tranProcess.BeginTran();
                try
                {
                    data.IsSuccess = await _tasksQzServices.Update(model);
                    data.Data = jobId.ObjToString();
                    if (data.IsSuccess)
                    {
                        data.Msg = "更新成功";
                        var ResuleModel = await _schedulerCenter.PauseJob(model);
                        if (ResuleModel.IsSuccess)
                        {
                            data.Msg = $"{data.Msg}=>暂停成功=>{ResuleModel.Msg}";
                        }
                        else
                        {
                            data.Msg = $"{data.Msg}=>暂停失败=>{ResuleModel.Msg}";
                        }
                        data.IsSuccess = ResuleModel.IsSuccess;
                    }
                    else
                    {
                        data.Msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.IsSuccess)
                        _tranProcess.CommitTran();
                    else
                        _tranProcess.RollbackTran();
                }
            }
            else
            {
                data.Msg = "任务不存在";
            }
            return data;
        }
        /// <summary>
        /// 恢复一个计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>        
        [HttpGet]
        public async Task<APITResult<string>> ResumeJob(int jobId)
        {
            var data = new APITResult<string>();

            var model = await _tasksQzServices.GetById(jobId);
            if (model != null)
            {
                _tranProcess.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.IsSuccess = await _tasksQzServices.Update(model);
                    data.Data = jobId.ObjToString();
                    if (data.IsSuccess)
                    {
                        data.Msg = "更新成功";
                        var ResuleModel = await _schedulerCenter.ResumeJob(model);
                        if (ResuleModel.IsSuccess)
                        {
                            data.Msg = $"{data.Msg}=>恢复成功=>{ResuleModel.Msg}";
                        }
                        else
                        {
                            data.Msg = $"{data.Msg}=>恢复失败=>{ResuleModel.Msg}";
                        }
                        data.IsSuccess = ResuleModel.IsSuccess;
                    }
                    else
                    {
                        data.Msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.IsSuccess)
                        _tranProcess.CommitTran();
                    else
                        _tranProcess.RollbackTran();
                }
            }
            else
            {
                data.Msg = "任务不存在";
            }
            return data;
        }
        /// <summary>
        /// 重启一个计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APITResult<string>> ReCovery(int jobId)
        {
            var data = new APITResult<string>();
            var model = await _tasksQzServices.GetById(jobId);
            if (model != null)
            {

                _tranProcess.BeginTran();
                try
                {
                    model.IsStart = true;
                    data.IsSuccess = await _tasksQzServices.Update(model);
                    data.Data = jobId.ObjToString();
                    if (data.IsSuccess)
                    {
                        data.Msg = "更新成功";
                        var ResuleModelStop = await _schedulerCenter.StopScheduleJobAsync(model);
                        var ResuleModelStar = await _schedulerCenter.AddScheduleJobAsync(model);
                        if (ResuleModelStar.IsSuccess)
                        {
                            data.Msg = $"{data.Msg}=>停止:{ResuleModelStop.Msg}=>启动:{ResuleModelStar.Msg}";
                            data.Data = jobId.ObjToString();

                        }
                        else
                        {
                            data.Msg = $"{data.Msg}=>停止:{ResuleModelStop.Msg}=>启动:{ResuleModelStar.Msg}";
                            data.Data = jobId.ObjToString();
                        }
                        data.IsSuccess = ResuleModelStar.IsSuccess;
                    }
                    else
                    {
                        data.Msg = "更新失败";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (data.IsSuccess)
                        _tranProcess.CommitTran();
                    else
                        _tranProcess.RollbackTran();
                }
            }
            else
            {
                data.Msg = "任务不存在";
            }
            return data;

        }
        /// <summary>
        /// 获取任务命名空间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public APITResult<List<QuartzReflectionModel>> GetTaskNameSpace()
        {
            var baseType = typeof(IJob);
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var referencedAssemblies = System.IO.Directory.GetFiles(path, "TBlog.Tasks.dll").Select(Assembly.LoadFrom).ToArray();
            var types = referencedAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).Select(item => new QuartzReflectionModel { NameSpace = item.Namespace, ClassName = item.Name, Remark = "" }).ToList();
            return APITResult<List<QuartzReflectionModel>>.Success("获取成功", implementTypes);
        }

    }
}
