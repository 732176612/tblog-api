using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.IOC;
using TBlog.Common;
using TBlog.Model;
namespace TBlog.Extensions
{
    public static class SqlSugarSetup
    {
        public static void AddSqlSugarSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (ApiConfig.DBSetting.MainDB == null) throw new Exception("请配置主数据库链接：DBSetting.DBS");
            if (ApiConfig.DBSetting.MainDB.Connection == null) throw new Exception("主数据库链接不能为空！");

             services.AddSqlSugar(new IocConfig()
            {
                ConfigId = ApiConfig.DBSetting.MainDB,
                DbType = (IocDbType)Enum.Parse(typeof(IocDbType), ApiConfig.DBSetting.MainDB.DBType),
                ConnectionString = ApiConfig.DBSetting.MainDB.Connection,
                IsAutoCloseConnection = true,
                SlaveConnectionConfigs = ApiConfig.DBSetting.SlaveDBs
                .Select((c, index) => new IocConfig
                {
                    ConnectionString = c.Connection,
                }).ToList()
            });

            //自动补充NoLock
            services.ConfigurationSugar(db =>
            {
                db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
                {
                    IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                };

                db.QueryFilter.AddTableFilter<IDeleteEntity>(it => it.IsDeleted == false);

                db.Aop.OnLogExecuting = (sql, param) =>
                {
                    if (ApiConfig.AOPSetting.SqlLogAOP)
                    {
                        Parallel.For(0, 1, e =>
                        {
                            var sqlval = $"[SQL参数]:{string.Join(";\n", param.Select(c => $"{c.ParameterName}:{c.Value}"))};[SQL语句]:{sql};";
                            Console.WriteLine(UtilMethods.GetSqlString(((DbType)Enum.Parse(typeof(DbType), ApiConfig.DBSetting.MainDB.DBType)), sql, param));
                            LogLock.OutSql2Log("SqlLog", dataParas: sqlval);
                        });
                    }
                };
                db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) =>
                    {
                        if (column.IsPrimarykey && property.PropertyType == typeof(int))
                        {
                            column.IsIdentity = true;
                        }
                    }
                };
            });
        }
    }
}
