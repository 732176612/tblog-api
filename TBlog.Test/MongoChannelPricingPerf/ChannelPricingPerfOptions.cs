using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TBlog.Test.MongoChannelPricingPerf;

public sealed class ChannelPricingPerfOptions
{
    public const long DefaultTotalDocuments = 40_000_000;
    public const int DefaultBatchSize = 5_000;
    public const int DefaultParallelWriters = 4;
    public const int DefaultQuerySampleCount = 10_000;
    public const int DefaultWarmupQueryCount = 100;

    public const string DefaultDatabaseName = "perf_test";
    public const string LegacyCollectionName = "channel_pricing_legacy";
    public const string OptimizedCollectionName = "channel_pricing_optimized";

    public const int HIdMin = 1;
    public const int HIdMax = 500_000;
    public const int IdMin = 1;
    public const int IdMax = 10_000;

    public string MongoConnection { get; init; } = "";
    public string DatabaseName { get; init; } = DefaultDatabaseName;
    public string LegacyCollection { get; init; } = LegacyCollectionName;
    public string OptimizedCollection { get; init; } = OptimizedCollectionName;
    public long TotalDocuments { get; init; } = DefaultTotalDocuments;
    public int BatchSize { get; init; } = DefaultBatchSize;
    public int ParallelWriters { get; init; } = DefaultParallelWriters;
    public int QuerySampleCount { get; init; } = DefaultQuerySampleCount;
    public int WarmupQueryCount { get; init; } = DefaultWarmupQueryCount;
    public WriteConcern WriteConcern { get; init; } = WriteConcern.W1;
    public string? HIdSampleFilePath { get; init; }

    public static ChannelPricingPerfOptions FromConfiguration(IConfiguration configuration)
    {
        var mongoConnection = configuration["DBSetting:MongoConnection"];
        if (string.IsNullOrWhiteSpace(mongoConnection))
        {
            throw new InvalidOperationException(
                "未读到 MongoDB 连接串。请在 User Secrets 或环境变量中配置 DBSetting:MongoConnection（需指向 mongos）。");
        }

        return new ChannelPricingPerfOptions
        {
            MongoConnection = mongoConnection,
            DatabaseName = configuration["MongoPerf:DatabaseName"] ?? DefaultDatabaseName,
            LegacyCollection = configuration["MongoPerf:LegacyCollection"] ?? LegacyCollectionName,
            OptimizedCollection = configuration["MongoPerf:OptimizedCollection"] ?? OptimizedCollectionName,
            TotalDocuments = ParseLong(configuration["MongoPerf:TotalDocuments"], DefaultTotalDocuments),
            BatchSize = ParseInt(configuration["MongoPerf:BatchSize"], DefaultBatchSize),
            ParallelWriters = ParseInt(configuration["MongoPerf:ParallelWriters"], DefaultParallelWriters),
            QuerySampleCount = ParseInt(configuration["MongoPerf:QuerySampleCount"], DefaultQuerySampleCount),
            WarmupQueryCount = ParseInt(configuration["MongoPerf:WarmupQueryCount"], DefaultWarmupQueryCount),
            WriteConcern = ParseWriteConcern(configuration["MongoPerf:WriteConcern"]),
            HIdSampleFilePath = configuration["MongoPerf:HIdSampleFilePath"],
        };
    }

    public MongoClient CreateClient()
    {
        var settings = MongoClientSettings.FromConnectionString(MongoConnection);
        settings.WriteConcern = WriteConcern;
        return new MongoClient(settings);
    }

    private static int ParseInt(string? value, int defaultValue) =>
        int.TryParse(value, out var parsed) ? parsed : defaultValue;

    private static long ParseLong(string? value, long defaultValue) =>
        long.TryParse(value, out var parsed) ? parsed : defaultValue;

    private static bool ParseBool(string? value, bool defaultValue) =>
        bool.TryParse(value, out var parsed) ? parsed : defaultValue;

    private static WriteConcern ParseWriteConcern(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "majority" => WriteConcern.WMajority,
            "w1" or "1" => WriteConcern.W1,
            _ => WriteConcern.W1,
        };
}
