// MongoDB 分片集群：在 mongos 上执行本脚本。
// 建议在插入 8000 万数据之前，对空集合完成分片，避免后期 chunk 迁移。
//
// 用法:
//   mongosh "mongodb://user:pass@mongos-host:27017" mongo-shard-index-commands.js
//
// 库名/集合名需与 ChannelPricingPerfOptions 默认值一致:
//   perf_test.channel_pricing_legacy
//   perf_test.channel_pricing_optimized

const dbName = "perf_test";
const legacyCollection = "channel_pricing_legacy";
const optimizedCollection = "channel_pricing_optimized";

const perfDb = db.getSiblingDB(dbName);

print("=== Step 1: enable sharding for database ===");
const enableResult = sh.enableSharding(dbName);
printjson(enableResult);

// shardCollection 要求分片键上已有索引；空集合需先显式 createIndex
print("\n=== Step 2: create hashed index on HId (required before sharding) ===");
perfDb.getCollection(legacyCollection).createIndex({ HId: "hashed" });
perfDb.getCollection(optimizedCollection).createIndex({ HId: "hashed" });
print(`${dbName}.${legacyCollection} indexes:`);
printjson(perfDb.getCollection(legacyCollection).getIndexes());
print(`${dbName}.${optimizedCollection} indexes:`);
printjson(perfDb.getCollection(optimizedCollection).getIndexes());

print("\n=== Step 3: shard legacy collection on HId (hashed) ===");
const legacyShardResult = sh.shardCollection(
  `${dbName}.${legacyCollection}`,
  { HId: "hashed" }
);
printjson(legacyShardResult);

print("\n=== Step 4: shard optimized collection on HId (hashed) ===");
const optimizedShardResult = sh.shardCollection(
  `${dbName}.${optimizedCollection}`,
  { HId: "hashed" }
);
printjson(optimizedShardResult);

print("\n=== Step 5: verify shard status ===");
sh.status();

print("\n=== Step 6: optional collection stats (after data load) ===");
print(`${dbName}.${legacyCollection} stats:`);
printjson(perfDb.getCollection(legacyCollection).stats());
print(`${dbName}.${optimizedCollection} stats:`);
printjson(perfDb.getCollection(optimizedCollection).stats());

print("\nDone. Hashed index on HId must exist before shardCollection (Step 2).");
