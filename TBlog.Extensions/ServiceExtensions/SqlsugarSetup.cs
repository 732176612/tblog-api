using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;
using SqlSugar;
using StackExchange.Profiling;

namespace TBlog.Extensions
{
    public static class SqlsugarSetup
    {
        /// <summary>
        /// SqlSugar配置
        /// </summary>
        /// <param name="services"></param>
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            services.AddScoped<ISqlSugarClient>(o =>
            {
                var mainDb = ApiConfig.DBSetting.DBS.First();
                var slaveDbs = ApiConfig.DBSetting.DBS.Skip(1).Select((c, index) => new SlaveConnectionConfig
                {
                    ConnectionString = c.Connection,
                    HitRate = index
                }).ToList();

                var connectionConfig = new ConnectionConfig
                {
                    ConfigId = 1,
                    ConnectionString = mainDb.Connection,
                    DbType = (DbType)Enum.Parse(typeof(DbType), mainDb.DBType),
                    IsAutoCloseConnection = true,
                    AopEvents = new AopEvents
                    {
                        OnLogExecuting = (sql, param) =>
                        {
                            if (ApiConfig.AOPSetting.SqlLogAOP)
                            {
                                Parallel.For(0, 1, e =>
                                {
                                    var sqlval = $"[SQL参数]:{string.Join(";\n", param.Select(c => $"{c.ParameterName}:{c.Value}"))};[SQL语句]:{sql};";
                                    MiniProfiler.Current.CustomTiming("SqlLog:", sqlval);
                                    LogLock.OutSql2Log("SqlLog", dataParas: sqlval);
                                });
                            }
                        }
                    },
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true
                    },
                    SlaveConnectionConfigs = slaveDbs,
                    ConfigureExternalServices = new ConfigureExternalServices()
                    {
                        EntityService = (property, column) =>
                        {
                            if (column.IsPrimarykey && property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
                            {
                                column.IsIdentity = true;
                            }
                        }
                    },
                    InitKeyType = InitKeyType.Attribute
                };
                return new SqlSugarClient(connectionConfig);
            });
        }
    }
}
