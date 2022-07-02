namespace TBlog.Model
{
    /// <summary>
    /// 定时任务的执行模式
    /// </summary>
    public enum EnumTaskQzMode
    {
        [Description("简易模式")]
        Smaple = 0,

        [Description("Cron模式")]
        Cron = 1
    }
}
