using System.Reflection;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TBlog.Extensions.ServiceExtensions;
using Xunit;

namespace TBlog.Test;

/// <summary>
/// 使用 SqlSugar 从 MySQL 源库读取实体表数据并插入 PostgreSQL。
/// 目标库需已用 CodeFirst 建好表；默认先 TRUNCATE 全部业务表（不含 HttpLog 分表）。
/// </summary>
public static class MySqlToPostgreSqlDataMigrator
{
    public static async Task RunAsync(
        string mySqlConnectionString,
        string postgreSqlConnectionString,
        bool truncatePostgreSqlBeforeCopy = true,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= NullLogger.Instance;
        if (string.IsNullOrWhiteSpace(mySqlConnectionString))
            throw new ArgumentException("MySQL 连接串不能为空", nameof(mySqlConnectionString));
        if (string.IsNullOrWhiteSpace(postgreSqlConnectionString))
            throw new ArgumentException("PostgreSQL 连接串不能为空", nameof(postgreSqlConnectionString));

        var allTypes = GetEntityTypes().ToList();
        var insertOrder = GetOrderedEntityTypesForCopy();

        using var mysql = CreateMySqlClient(mySqlConnectionString);
        using var pg = CreatePostgreSqlClient(postgreSqlConnectionString);

        if (truncatePostgreSqlBeforeCopy)
            await TruncateAllEntityTablesAsync(pg, allTypes, logger, cancellationToken);

        foreach (var entityType in insertOrder)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var copied = await CopyTableDynamicAsync(mysql, pg, entityType, logger, cancellationToken);
            logger.LogInformation("迁移表 {Table}: {Count} 行", GetDbTableName(pg, entityType), copied);
        }
    }

    /// <summary>
    /// 插入顺序：满足常见外键依赖（普通数据库用户无法使用 session_replication_role）。
    /// </summary>
    private static readonly Type[] CopyEntityOrder =
    {
        typeof(RoleEntity),
        typeof(MenuEntity),
        typeof(UserEntity),
        typeof(CompanyInfoEntity),
        typeof(ProjectInfoEntity),
        typeof(EduInfoEntity),
        typeof(SkillInfoEntity),
        typeof(MediaInfoEntity),
        typeof(ActicleEntity),
        typeof(ActicleTagEntity),
        typeof(ActicleStatsEntity),
        typeof(ActicleHisLogEntity),
        typeof(CommentEntity),
        typeof(CommentLikeEntity),
        typeof(TaskQzEntity),
        typeof(TaskLogEntity),
        typeof(IpAddressInfo),
    };

    /// <summary>用于 COPY 的实体顺序；未出现在预定义列表中的类型按名称排在末尾。</summary>
    private static List<Type> GetOrderedEntityTypesForCopy()
    {
        var set = GetEntityTypes().ToHashSet();
        var ordered = new List<Type>();
        foreach (var t in CopyEntityOrder)
        {
            if (set.Contains(t))
                ordered.Add(t);
        }

        foreach (var t in set.OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            if (!ordered.Contains(t))
                ordered.Add(t);
        }

        return ordered;
    }

    public static IEnumerable<Type> GetEntityTypes()
    {
        return from t in Assembly.GetAssembly(typeof(IEntity))!.GetTypes()
               where t.IsClass
                     && t.Namespace == "TBlog.Model"
                     && t.IsAssignableTo(typeof(IEntity))
                     && t != typeof(HttpLogEntity)
               select t;
    }

    private static SqlSugarClient CreateMySqlClient(string connectionString)
    {
        return new SqlSugarClient(new ConnectionConfig
        {
            DbType = DbType.MySql,
            ConnectionString = connectionString,
            IsAutoCloseConnection = true,
            MoreSettings = new ConnMoreSettings
            {
                IsWithNoLockQuery = false
            }
        });
    }

    private static SqlSugarClient CreatePostgreSqlClient(string connectionString)
    {
        return new SqlSugarClient(new ConnectionConfig
        {
            DbType = DbType.PostgreSQL,
            ConnectionString = connectionString,
            IsAutoCloseConnection = true,
            MoreSettings = new ConnMoreSettings
            {
                IsWithNoLockQuery = false,
                IsAutoRemoveDataCache = false,
                PgSqlIsAutoToLower = false,
                PgSqlIsAutoToLowerCodeFirst = false
            },
            ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityService = (property, column) =>
                {
                    if (column.IsPrimarykey && property.PropertyType == typeof(int))
                        column.IsIdentity = true;
                    if (!column.IsIgnore)
                    {
                        var attr = property.GetCustomAttribute<SugarColumn>();
                        column.DbColumnName = string.IsNullOrWhiteSpace(attr?.ColumnName)
                            ? property.Name
                            : attr.ColumnName!;
                    }
                }
            }
        });
    }

    private static string GetDbTableName(ISqlSugarClient db, Type entityType)
    {
        var em = db.EntityMaintenance;
        var m = em.GetType().GetMethod("GetEntityInfo", new[] { typeof(Type) });
        object? info;
        if (m != null)
            info = m.Invoke(em, new object[] { entityType });
        else
        {
            var gm = em.GetType().GetMethods()
                .Single(x => x.Name == "GetEntityInfo" && x.IsGenericMethodDefinition && x.GetParameters().Length == 0);
            info = gm.MakeGenericMethod(entityType).Invoke(em, null);
        }

        if (info == null)
            throw new InvalidOperationException($"无法解析实体 {entityType.Name} 的表名");
        var name = info.GetType().GetProperty("DbTableName")?.GetValue(info) as string;
        return name ?? throw new InvalidOperationException($"{entityType.Name} 的 DbTableName 为空");
    }

    private static async Task TruncateAllEntityTablesAsync(
        ISqlSugarClient pg,
        IReadOnlyList<Type> modelTypes,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var names = modelTypes.Select(t => GetDbTableName(pg, t))
            .Distinct()
            .Select(n => "\"" + n.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"")
            .ToList();

        if (names.Count == 0)
            return;

        var sql = $"TRUNCATE TABLE {string.Join(", ", names)} RESTART IDENTITY CASCADE";
        logger.LogInformation("执行: {Sql}", sql);
        try
        {
            await pg.Ado.ExecuteCommandAsync(sql);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "TRUNCATE 失败（若目标库为空或首次迁移可忽略），将继续逐表插入");
        }
    }

    private static async Task<int> CopyTableDynamicAsync(
        ISqlSugarClient source,
        ISqlSugarClient dest,
        Type entityType,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var mi = typeof(MySqlToPostgreSqlDataMigrator).GetMethod(
                     nameof(CopyTableGenericAsync),
                     BindingFlags.NonPublic | BindingFlags.Static)
                 ?? throw new InvalidOperationException(nameof(CopyTableGenericAsync));

        var g = mi.MakeGenericMethod(entityType);
        var task = (Task<int>)g.Invoke(null, new object[] { source, dest, logger, cancellationToken })!;
        return await task.ConfigureAwait(false);
    }

    private static async Task<int> CopyTableGenericAsync<T>(
        ISqlSugarClient source,
        ISqlSugarClient dest,
        ILogger logger,
        CancellationToken cancellationToken) where T : class, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        var list = await source.Queryable<T>().ToListAsync();
        if (list.Count == 0)
            return 0;

        await dest.Insertable(list).ExecuteCommandAsync();
        return list.Count;
    }
}

/// <summary>
/// 手工执行：旧 MySQL（远程 tblog）全表数据拷贝到 PostgreSQL。
/// Apollo 使用用户机密中的 MetaServer/AppId/Secret，合并后的 DBSetting.MainDB.Connection 为 PG 目标。
/// </summary>
public class MySqlToPostgreSqlMigration_Test
{
    /// <summary>远程旧库 MySQL 连接（按项目约定写死）。</summary>
    private const string MySqlSourceConnection =
        "";

    [Fact]
    public async Task CopyAll_FromMySql_ToPostgreSql()
    {
        var basePath = ApplicationEnvironment.ApplicationBasePath;

        var pre = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<MySqlToPostgreSqlMigration_Test>()
            .AddEnvironmentVariables()
            .Build();

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<MySqlToPostgreSqlMigration_Test>()
            .AddEnvironmentVariables();
        builder.AddApolloSetUp(pre.GetSection("Apollo"));
        var cfg = builder.Build();

        var pg = cfg["DBSetting:MainDB:Connection"];
        if (string.IsNullOrWhiteSpace(pg))
        {
            throw new InvalidOperationException(
                "未读到 PostgreSQL 连接串。请确认 Apollo 已下发 DBSetting:MainDB:Connection，或在用户机密/环境变量中配置。");
        }

        using var loggerFactory = LoggerFactory.Create(b => b.AddConsole().SetMinimumLevel(LogLevel.Information));
        var logger = loggerFactory.CreateLogger("MySqlToPostgreSql");

        await MySqlToPostgreSqlDataMigrator.RunAsync(MySqlSourceConnection, pg, logger: logger);
    }
}
