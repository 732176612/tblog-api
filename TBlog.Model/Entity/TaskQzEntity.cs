namespace TBlog.Model
{
    /// <summary>
    /// 任务计划表
    /// </summary>
    public class TaskQzEntity : IEntity
    {
        #region 基础属性
        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = SnowFlakeSingle.instance.NextId();

        /// <summary>
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime MDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object EntityId => Id;
        #endregion

        #region 实体属性
        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务运行时间表达式
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = true)]
        public string Cron { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务所在类
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string ClassName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 1000, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int RunTimes { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 触发器类型（0、simple 1、cron）
        /// </summary>
        public EnumTaskQzMode TriggerType { get; set; }

        /// <summary>
        /// 执行间隔时间, 秒为单位
        /// </summary>
        public int IntervalSecond { get; set; }

        /// <summary>
        /// 循环执行次数
        /// </summary>
        public int CycleRunTimes { get; set; }

        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsStart { get; set; } = false;

        /// <summary>
        /// 执行传参
        /// </summary>
        public string JobParams { get; set; }

        /// <summary>
        /// 任务内存中的状态
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<QuartzTaskModel> Triggers { get; set; }
        #endregion
    }
}
