using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TBlog.Test.MongoChannelPricingPerf;

public enum ChannelPricingDocumentKind
{
    Legacy,
    Optimized,
}

public static class ChannelPricingBulkInserter
{
    private const int ProgressLogEveryBatches = 100;

    public static async Task<long> InsertAsync(
        ChannelPricingPerfOptions options,
        ChannelPricingDocumentKind kind,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger ??= NullLogger.Instance;
        var collectionName = kind switch
        {
            ChannelPricingDocumentKind.Legacy => options.LegacyCollection,
            ChannelPricingDocumentKind.Optimized => options.OptimizedCollection,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

        var client = options.CreateClient();
        var database = client.GetDatabase(options.DatabaseName);
        var collection = database.GetCollection<BsonDocument>(collectionName);

        var totalDocuments = options.TotalDocuments;
        var batchSize = options.BatchSize;
        var totalBatches = (int)((totalDocuments + batchSize - 1) / batchSize);
        var parallelWriters = Math.Max(1, options.ParallelWriters);

        logger.LogInformation(
            "开始插入 {Kind} 数据: 总量={TotalDocuments}, 批次大小={BatchSize}, 批次数={TotalBatches}, 并行度={ParallelWriters}",
            kind,
            totalDocuments,
            batchSize,
            totalBatches,
            parallelWriters);

        var insertedCount = 0L;
        var completedBatches = 0;
        var stopwatch = Stopwatch.StartNew();
        var insertOptions = new InsertManyOptions { IsOrdered = false };
        var progressLock = new object();

        await Parallel.ForEachAsync(
            Enumerable.Range(0, totalBatches),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = parallelWriters,
                CancellationToken = cancellationToken,
            },
            async (batchIndex, token) =>
            {
                var random = new Random(unchecked(batchIndex * 397 ^ (int)kind));
                var currentBatchSize = batchIndex == totalBatches - 1
                    ? (int)(totalDocuments - (long)batchIndex * batchSize)
                    : batchSize;

                if (currentBatchSize <= 0)
                    return;

                var documents = kind switch
                {
                    ChannelPricingDocumentKind.Legacy => ChannelPricingDataGenerator.CreateLegacyBatch(currentBatchSize, random),
                    ChannelPricingDocumentKind.Optimized => ChannelPricingDataGenerator.CreateOptimizedBatch(currentBatchSize, random),
                    _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
                };

                await collection.InsertManyAsync(documents, insertOptions, token);

                var currentInserted = Interlocked.Add(ref insertedCount, currentBatchSize);
                var batchCount = Interlocked.Increment(ref completedBatches);
                if (batchCount % ProgressLogEveryBatches == 0 || currentInserted >= totalDocuments)
                {
                    lock (progressLock)
                    {
                        var elapsedSeconds = Math.Max(stopwatch.Elapsed.TotalSeconds, 0.001);
                        var docsPerSecond = currentInserted / elapsedSeconds;
                        logger.LogInformation(
                            "{Kind} 插入进度: {Inserted}/{TotalDocuments} ({Progress:P1}), 速率={DocsPerSecond:N0} docs/s, 耗时={Elapsed}",
                            kind,
                            currentInserted,
                            totalDocuments,
                            (double)currentInserted / totalDocuments,
                            docsPerSecond,
                            stopwatch.Elapsed);
                    }
                }
            });

        stopwatch.Stop();
        logger.LogInformation(
            "{Kind} 插入完成: {InsertedCount} 条, 总耗时={Elapsed}, 平均速率={DocsPerSecond:N0} docs/s",
            kind,
            insertedCount,
            stopwatch.Elapsed,
            insertedCount / Math.Max(stopwatch.Elapsed.TotalSeconds, 0.001));

        return insertedCount;
    }
}
