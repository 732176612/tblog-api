using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using TBlog.Test.MongoChannelPricingPerf;
using Xunit;

namespace TBlog.Test;

/// <summary>
/// MongoDB 渠道定价文档性能测试（手工执行）。
/// 配置 User Secrets / 环境变量:
///   DBSetting:MongoConnection
///   MongoPerf:DatabaseName / TotalDocuments / BatchSize / ParallelWriters / ...
/// </summary>
public class MongoChannelPricingPerf_Test
{
    private static ChannelPricingPerfOptions LoadOptions()
    {
        var basePath = AppContext.BaseDirectory;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<MongoChannelPricingPerf_Test>()
            .AddEnvironmentVariables()
            .Build();

        return ChannelPricingPerfOptions.FromConfiguration(configuration);
    }

    [Fact]
    public async Task InsertLegacy_80M()
    {
        var options = LoadOptions();
        using var loggerFactory = LoggerFactory.Create(b =>
            b.AddConsole().SetMinimumLevel(LogLevel.Information));
        var logger = loggerFactory.CreateLogger("MongoChannelPricingPerf");
        var inserted = await ChannelPricingBulkInserter.InsertAsync(
            options,
            ChannelPricingDocumentKind.Legacy,
            logger);

        Assert.True(inserted > 0);
    }

    [Fact]
    public async Task InsertOptimized_80M()
    {
        var options = LoadOptions();
        using var loggerFactory = LoggerFactory.Create(b =>
            b.AddConsole().SetMinimumLevel(LogLevel.Information));
        var logger = loggerFactory.CreateLogger("MongoChannelPricingPerf");
        var inserted = await ChannelPricingBulkInserter.InsertAsync(
            options,
            ChannelPricingDocumentKind.Optimized,
            logger);

        Assert.True(inserted > 0);
    }

    [Fact]
    public async Task RunQueryBenchmark_10k()
    {
        var options = LoadOptions();
        using var loggerFactory = LoggerFactory.Create(b =>
            b.AddConsole().SetMinimumLevel(LogLevel.Information));
        var logger = loggerFactory.CreateLogger("MongoChannelPricingPerf");
        var report = await ChannelPricingQueryBenchmark.RunAsync(options, logger: logger);

        Assert.Equal(options.QuerySampleCount, report.Legacy.QueryCount);
        Assert.Equal(options.QuerySampleCount, report.Optimized.QueryCount);
        Assert.True(report.Legacy.AvgMs >= 0);
        Assert.True(report.Optimized.AvgMs >= 0);
    }

    [Fact]
    public async Task GenerateHIdSampleFile()
    {
        var options = LoadOptions();
        using var loggerFactory = LoggerFactory.Create(b =>
            b.AddConsole().SetMinimumLevel(LogLevel.Information));
        var logger = loggerFactory.CreateLogger("MongoChannelPricingPerf");
        var outputPath = options.HIdSampleFilePath
            ?? Path.Combine(AppContext.BaseDirectory, "mongo-perf-hids.json");

        var hIds = ChannelPricingDataGenerator.GenerateRandomHIds(options.QuerySampleCount, Random.Shared);
        await ChannelPricingQueryBenchmark.SaveHIdSampleAsync(hIds, outputPath);

        logger.LogInformation("已生成 {Count} 个 HId 样本: {OutputPath}", hIds.Length, outputPath);
        Assert.True(File.Exists(outputPath));
    }
}
