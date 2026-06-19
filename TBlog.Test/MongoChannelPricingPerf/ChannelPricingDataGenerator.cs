using MongoDB.Bson;

namespace TBlog.Test.MongoChannelPricingPerf;

internal readonly record struct ChannelPricingSeed(
    bool AgentMode,
    int AgentCId,
    int AgentId,
    int HId,
    int SupplierCId,
    int SupplierId,
    string? ChannelAttribute,
    string? PricingAttribute,
    int CreatedBy,
    DateTime CreatedTime,
    string LastTaskId);

internal static class ChannelPricingDataGenerator
{
    private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static ChannelPricingSeed CreateSeed(Random random)
    {
        var agentMode = random.Next(2) == 0;
        var supplierCId = random.Next(ChannelPricingPerfOptions.IdMin, ChannelPricingPerfOptions.IdMax + 1);
        var supplierId = random.Next(ChannelPricingPerfOptions.IdMin, ChannelPricingPerfOptions.IdMax + 1);
        var hId = random.Next(ChannelPricingPerfOptions.HIdMin, ChannelPricingPerfOptions.HIdMax + 1);

        if (agentMode)
        {
            return new ChannelPricingSeed(
                AgentMode: true,
                AgentCId: random.Next(ChannelPricingPerfOptions.IdMin, ChannelPricingPerfOptions.IdMax + 1),
                AgentId: random.Next(ChannelPricingPerfOptions.IdMin, ChannelPricingPerfOptions.IdMax + 1),
                HId: hId,
                SupplierCId: supplierCId,
                SupplierId: supplierId,
                ChannelAttribute: null,
                PricingAttribute: null,
                CreatedBy: random.Next(ChannelPricingPerfOptions.IdMin, ChannelPricingPerfOptions.IdMax + 1),
                CreatedTime: RandomCreatedTime(random),
                LastTaskId: RandomHexString(random, 24));
        }

        return new ChannelPricingSeed(
            AgentMode: false,
            AgentCId: 0,
            AgentId: 0,
            HId: hId,
            SupplierCId: supplierCId,
            SupplierId: supplierId,
            ChannelAttribute: RandomUppercaseLetters(random, random.Next(5, 7)),
            PricingAttribute: RandomUppercaseLetters(random, random.Next(5, 7)),
            CreatedBy: random.Next(ChannelPricingPerfOptions.IdMin, ChannelPricingPerfOptions.IdMax + 1),
            CreatedTime: RandomCreatedTime(random),
            LastTaskId: RandomHexString(random, 24));
    }

    public static BsonDocument CreateLegacyDocument(ChannelPricingSeed seed)
    {
        var document = new BsonDocument
        {
            { "HId", seed.HId },
            { "SupplierCId", seed.SupplierCId },
            { "SupplierId", seed.SupplierId },
            { "CreatedBy", seed.CreatedBy },
            { "CreatedTime", seed.CreatedTime },
            { "LastTaskId", seed.LastTaskId },
            { "SellStatus", true },
        };

        if (seed.AgentMode)
        {
            document.Add("AgentCId", seed.AgentCId);
            document.Add("AgentId", seed.AgentId);
            document.Add("ChannelAttribute", BsonNull.Value);
            document.Add("PricingAttribute", BsonNull.Value);
        }
        else
        {
            document.Add("AgentCId", BsonNull.Value);
            document.Add("AgentId", BsonNull.Value);
            document.Add("ChannelAttribute", seed.ChannelAttribute!);
            document.Add("PricingAttribute", seed.PricingAttribute!);
        }

        return document;
    }

    public static BsonDocument CreateOptimizedDocument(ChannelPricingSeed seed)
    {
        var document = new BsonDocument
        {
            { "HId", seed.HId },
            { "Supplier", $"{seed.SupplierCId}_{seed.SupplierId}" },
            { "SellStatus", true },
        };

        if (seed.AgentMode)
        {
            document.Add("Agent", $"{seed.AgentCId}_{seed.AgentId}");
            document.Add("Attr", BsonNull.Value);
        }
        else
        {
            document.Add("Agent", BsonNull.Value);
            document.Add("Attr", $"{seed.ChannelAttribute}_{seed.PricingAttribute}");
        }

        return document;
    }

    public static List<BsonDocument> CreateLegacyBatch(int count, Random random)
    {
        var batch = new List<BsonDocument>(count);
        for (var i = 0; i < count; i++)
            batch.Add(CreateLegacyDocument(CreateSeed(random)));
        return batch;
    }

    public static List<BsonDocument> CreateOptimizedBatch(int count, Random random)
    {
        var batch = new List<BsonDocument>(count);
        for (var i = 0; i < count; i++)
            batch.Add(CreateOptimizedDocument(CreateSeed(random)));
        return batch;
    }

    public static int[] GenerateRandomHIds(int count, Random random)
    {
        var hIds = new int[count];
        for (var i = 0; i < count; i++)
            hIds[i] = random.Next(ChannelPricingPerfOptions.HIdMin, ChannelPricingPerfOptions.HIdMax + 1);
        return hIds;
    }

    private static DateTime RandomCreatedTime(Random random)
    {
        var daysAgo = random.Next(0, 366);
        return DateTime.UtcNow.AddDays(-daysAgo).AddSeconds(-random.Next(0, 86400));
    }

    private static string RandomUppercaseLetters(Random random, int length)
    {
        Span<char> chars = stackalloc char[length];
        for (var i = 0; i < length; i++)
            chars[i] = UppercaseLetters[random.Next(UppercaseLetters.Length)];
        return new string(chars);
    }

    private static string RandomHexString(Random random, int length)
    {
        Span<char> chars = stackalloc char[length];
        for (var i = 0; i < length; i++)
            chars[i] = "0123456789abcdef"[random.Next(16)];
        return new string(chars);
    }
}
