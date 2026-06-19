using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TBlog.Test.MongoChannelPricingPerf;

public sealed class ChannelPricingQueryResult
{
    public string? Agent { get; init; }
    public string? Supplier { get; init; }
    public string? Attr { get; init; }
    public bool SellStatus { get; init; }
}

public sealed class ChannelPricingBenchmarkStats
{
    public required string CollectionName { get; init; }
    public int QueryCount { get; init; }
    public double AvgMs { get; init; }
    public double P50Ms { get; init; }
    public double P95Ms { get; init; }
    public double MaxMs { get; init; }
    public double TotalMs { get; init; }
}

public sealed class ChannelPricingBenchmarkReport
{
    public required int[] HIds { get; init; }
    public required ChannelPricingBenchmarkStats Legacy { get; init; }
    public required ChannelPricingBenchmarkStats Optimized { get; init; }
    public double SpeedupRatio => Legacy.AvgMs / Math.Max(Optimized.AvgMs, 0.001);
}

public static class ChannelPricingQueryBenchmark
{
    private static readonly ProjectionDefinition<BsonDocument> LegacyProjection = Builders<BsonDocument>.Projection
        .Include("AgentCId")
        .Include("AgentId")
        .Include("SupplierCId")
        .Include("SupplierId")
        .Include("ChannelAttribute")
        .Include("PricingAttribute")
        .Include("SellStatus");

    private static readonly ProjectionDefinition<BsonDocument> OptimizedProjection = Builders<BsonDocument>.Projection
        .Include("Agent")
        .Include("Supplier")
        .Include("Attr")
        .Include("SellStatus");

    public static int[] LoadOrGenerateHIds(ChannelPricingPerfOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.HIdSampleFilePath) && File.Exists(options.HIdSampleFilePath))
        {
            var json = File.ReadAllText(options.HIdSampleFilePath);
            var hIds = JsonSerializer.Deserialize<int[]>(json);
            if (hIds is { Length: > 0 })
                return hIds;
        }

        return ChannelPricingDataGenerator.GenerateRandomHIds(options.QuerySampleCount, Random.Shared);
    }

    public static async Task SaveHIdSampleAsync(int[] hIds, string filePath, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(hIds);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    public static async Task<ChannelPricingBenchmarkReport> RunAsync(
        ChannelPricingPerfOptions options,
        int[]? hIds = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= NullLogger.Instance;
        hIds ??= LoadOrGenerateHIds(options);

        var client = options.CreateClient();
        var database = client.GetDatabase(options.DatabaseName);
        var legacyCollection = database.GetCollection<BsonDocument>(options.LegacyCollection);
        var optimizedCollection = database.GetCollection<BsonDocument>(options.OptimizedCollection);

        logger.LogInformation("查询基准 warmup: {WarmupCount} 次", options.WarmupQueryCount);
        await WarmupAsync(legacyCollection, optimizedCollection, hIds, options.WarmupQueryCount, cancellationToken);

        logger.LogInformation("开始 Legacy 集合查询基准: {QueryCount} 次", hIds.Length);
        var legacyStats = await BenchmarkCollectionAsync(
            legacyCollection,
            options.LegacyCollection,
            hIds,
            QueryLegacyAsync,
            cancellationToken);

        logger.LogInformation("开始 Optimized 集合查询基准: {QueryCount} 次", hIds.Length);
        var optimizedStats = await BenchmarkCollectionAsync(
            optimizedCollection,
            options.OptimizedCollection,
            hIds,
            QueryOptimizedAsync,
            cancellationToken);

        var report = new ChannelPricingBenchmarkReport
        {
            HIds = hIds,
            Legacy = legacyStats,
            Optimized = optimizedStats,
        };

        LogReport(logger, report);
        return report;
    }

    private static async Task WarmupAsync(
        IMongoCollection<BsonDocument> legacyCollection,
        IMongoCollection<BsonDocument> optimizedCollection,
        int[] hIds,
        int warmupCount,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < warmupCount; i++)
        {
            var hId = hIds[i % hIds.Length];
            await QueryLegacyAsync(legacyCollection, hId, cancellationToken);
            await QueryOptimizedAsync(optimizedCollection, hId, cancellationToken);
        }
    }

    private static async Task<ChannelPricingBenchmarkStats> BenchmarkCollectionAsync(
        IMongoCollection<BsonDocument> collection,
        string collectionName,
        int[] hIds,
        Func<IMongoCollection<BsonDocument>, int, CancellationToken, Task<List<ChannelPricingQueryResult>>> queryFunc,
        CancellationToken cancellationToken)
    {
        var elapsedMs = new double[hIds.Length];
        var totalStopwatch = Stopwatch.StartNew();

        for (var i = 0; i < hIds.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var stopwatch = Stopwatch.StartNew();
            _ = await queryFunc(collection, hIds[i], cancellationToken);
            stopwatch.Stop();
            elapsedMs[i] = stopwatch.Elapsed.TotalMilliseconds;
        }

        totalStopwatch.Stop();
        Array.Sort(elapsedMs);

        return new ChannelPricingBenchmarkStats
        {
            CollectionName = collectionName,
            QueryCount = hIds.Length,
            AvgMs = elapsedMs.Average(),
            P50Ms = Percentile(elapsedMs, 0.50),
            P95Ms = Percentile(elapsedMs, 0.95),
            MaxMs = elapsedMs[^1],
            TotalMs = totalStopwatch.Elapsed.TotalMilliseconds,
        };
    }

    private static async Task<List<ChannelPricingQueryResult>> QueryLegacyAsync(
        IMongoCollection<BsonDocument> collection,
        int hId,
        CancellationToken cancellationToken)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("HId", hId);
        var documents = await collection
            .Find(filter)
            .Project(LegacyProjection)
            .ToListAsync(cancellationToken);

        return documents.Select(MapLegacyDocument).ToList();
    }

    private static async Task<List<ChannelPricingQueryResult>> QueryOptimizedAsync(
        IMongoCollection<BsonDocument> collection,
        int hId,
        CancellationToken cancellationToken)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("HId", hId);
        var documents = await collection
            .Find(filter)
            .Project(OptimizedProjection)
            .ToListAsync(cancellationToken);

        return documents.Select(MapOptimizedDocument).ToList();
    }

    private static ChannelPricingQueryResult MapLegacyDocument(BsonDocument document)
    {
        var agentCId = ReadNullableInt(document, "AgentCId");
        var agentId = ReadNullableInt(document, "AgentId");
        var supplierCId = document.GetValue("SupplierCId", BsonNull.Value);
        var supplierId = document.GetValue("SupplierId", BsonNull.Value);
        var channelAttribute = ReadNullableString(document, "ChannelAttribute");
        var pricingAttribute = ReadNullableString(document, "PricingAttribute");

        string? agent = agentCId.HasValue && agentId.HasValue
            ? $"{agentCId.Value}_{agentId.Value}"
            : null;

        string? supplier = supplierCId.IsBsonNull || supplierId.IsBsonNull
            ? null
            : $"{supplierCId.ToInt32()}_{supplierId.ToInt32()}";

        string? attr = channelAttribute != null && pricingAttribute != null
            ? $"{channelAttribute}_{pricingAttribute}"
            : null;

        return new ChannelPricingQueryResult
        {
            Agent = agent,
            Supplier = supplier,
            Attr = attr,
            SellStatus = document.GetValue("SellStatus", false).ToBoolean(),
        };
    }

    private static ChannelPricingQueryResult MapOptimizedDocument(BsonDocument document) =>
        new()
        {
            Agent = ReadNullableString(document, "Agent"),
            Supplier = ReadNullableString(document, "Supplier"),
            Attr = ReadNullableString(document, "Attr"),
            SellStatus = document.GetValue("SellStatus", false).ToBoolean(),
        };

    private static int? ReadNullableInt(BsonDocument document, string fieldName)
    {
        if (!document.TryGetValue(fieldName, out var value) || value.IsBsonNull)
            return null;
        return value.ToInt32();
    }

    private static string? ReadNullableString(BsonDocument document, string fieldName)
    {
        if (!document.TryGetValue(fieldName, out var value) || value.IsBsonNull)
            return null;
        return value.AsString;
    }

    private static double Percentile(double[] sortedValues, double percentile)
    {
        if (sortedValues.Length == 0)
            return 0;

        var index = (int)Math.Ceiling(percentile * sortedValues.Length) - 1;
        index = Math.Clamp(index, 0, sortedValues.Length - 1);
        return sortedValues[index];
    }

    private static void LogReport(ILogger logger, ChannelPricingBenchmarkReport report)
    {
        logger.LogInformation("========== MongoDB 渠道定价查询基准 ==========");
        LogStats(logger, report.Legacy);
        LogStats(logger, report.Optimized);
        logger.LogInformation(
            "对比: Optimized 相对 Legacy 平均耗时倍率 = {SpeedupRatio:F2}x (小于 1 表示 Optimized 更快)",
            report.SpeedupRatio);
        logger.LogInformation("============================================");
    }

    private static void LogStats(ILogger logger, ChannelPricingBenchmarkStats stats)
    {
        logger.LogInformation(
            "[{Collection}] Queries={QueryCount}, Avg={AvgMs:F3}ms, P50={P50Ms:F3}ms, P95={P95Ms:F3}ms, Max={MaxMs:F3}ms, Total={TotalMs:F1}ms",
            stats.CollectionName,
            stats.QueryCount,
            stats.AvgMs,
            stats.P50Ms,
            stats.P95Ms,
            stats.MaxMs,
            stats.TotalMs);
    }
}
