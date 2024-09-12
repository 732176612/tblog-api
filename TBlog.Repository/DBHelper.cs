namespace TBlog.Repository
{
    public class DBHelper
    {
        public static ConnectionConfig GetConfig()
        {
            string connectionString = ApiConfig.DBSetting.DBS.First().Connection;
            var config = new ConnectionConfig
            {
                ConfigId = 1,
                ConnectionString = connectionString,
                DbType = (DbType)Enum.Parse(typeof(DbType), ApiConfig.DBSetting.DBS.First().DBType),
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
                                LogLock.OutSql2Log("SqlLog", dataParas: sqlval);
                            });
                        }
                    }
                },
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoRemoveDataCache = true
                },
                SlaveConnectionConfigs = ApiConfig.DBSetting.DBS.Skip(1).Select((c, index) => new SlaveConnectionConfig
                {
                    ConnectionString = c.Connection,
                    HitRate = index
                }).ToList(),
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) =>
                    {
                        if (column.IsPrimarykey && property.PropertyType == typeof(int))
                        {
                            column.IsIdentity = true;
                        }
                    }
                },
                InitKeyType = InitKeyType.Attribute
            };

            return config;
        }

        public static SqlSugarScope DB = new SqlSugarScope(GetConfig());
    }
}
