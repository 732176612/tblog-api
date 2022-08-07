global using System;
global using System.Linq;
global using System.Collections.Generic;
global using TBlog.IRepository;
global using TBlog.Model;
global using TBlog.Common;
global using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace TBlog.Repository
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class, IEntity, new()
    {
        IMongoTransaction Transaction { get; set; }

        IMongoClient Client { get; set; }

        public IMongoCollection<TEntity> Collection { get; set; }

        public MongoRepository(IMongoTransaction transaction)
        {
            Transaction = transaction;
            Client = transaction.GetDbClient();
            Collection = Client.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public MongoRepository()
        {
            Client = new MongoClient(ApiConfig.DBSetting.MongoConnection);
            Collection = Client.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<TEntity>(typeof(TEntity).Name);
        }

        #region 查询

        public Expression<Func<TEntity, bool>> GetBaseFilter()
        {
            return FilterHelper.CreateExp<TEntity>(c => c.IsDeleted == false);
        }

        public async Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter)
        {
            filter = GetBaseFilter().AddExp(filter);
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return await Collection.Find(Transaction.GetSessionHandle(), filter).ToListAsync();
            else
                return await Collection.Find(filter).ToListAsync();
        }

        public async Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> filter)
        {
            filter = GetBaseFilter().AddExp(filter);
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return await Collection.Find(Transaction.GetSessionHandle(), filter).SingleOrDefaultAsync();
            else
            {
                TEntity respone = null;
                try
                {
                    respone = await Collection.Find(filter).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return respone;
            }
        }

        public async Task<List<TEntity>> GetAll()
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return await Collection.Find(Transaction.GetSessionHandle(), GetBaseFilter()).ToListAsync();
            else
                return await Collection.Find(c => true).ToListAsync();
        }

        public async Task<TEntity> GetById(object id)
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return await Collection.Find(Transaction.GetSessionHandle(), Builders<TEntity>.Filter.Eq("_id", id)).SingleAsync();
            else
                return await Collection.Find(Builders<TEntity>.Filter.Eq("_id", id)).SingleOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetByIds(IEnumerable<object> ids)
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return await Collection.Find(Transaction.GetSessionHandle(), Builders<TEntity>.Filter.In("_id", ids)).ToListAsync();
            else
                return await Collection.Find(Builders<TEntity>.Filter.In("_id", ids)).ToListAsync();
        }

        public async Task<PageModel<TEntity>> GetPage(int pageIndex = 1, int pageSize = 20, Expression<Func<TEntity, bool>> filter = null, Dictionary<Expression<Func<TEntity, object>>, bool> sorts = null)
        {
            var countFacet = AggregateFacet.Create("count",
            PipelineDefinition<TEntity, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TEntity>()
            }));

            List<PipelineStageDefinition<TEntity, TEntity>> pipelineDefinitions = new();

            SortDefinition<TEntity> sortDef = null;
            if (sorts.Any())
            {
                if (sorts.Count == 1)
                {
                    var sort = sorts.First();
                    if (sort.Value)
                    {
                        sortDef = new SortDefinitionBuilder<TEntity>().Ascending(sort.Key);
                    }
                    else
                    {
                        sortDef = new SortDefinitionBuilder<TEntity>().Descending(sort.Key);
                    }
                }
                else
                {
                    sortDef = new SortDefinitionBuilder<TEntity>().Combine(sorts.Select(sort =>
                      {
                          if (sort.Value)
                          {
                              return new SortDefinitionBuilder<TEntity>().Ascending(sort.Key);
                          }
                          else
                          {
                              return new SortDefinitionBuilder<TEntity>().Descending(sort.Key);
                          }
                      }));
                }
                pipelineDefinitions.Add(PipelineStageDefinitionBuilder.Sort(sortDef));
            }

            pipelineDefinitions.Add(PipelineStageDefinitionBuilder.Skip<TEntity>((pageIndex - 1) * pageSize));
            pipelineDefinitions.Add(PipelineStageDefinitionBuilder.Limit<TEntity>(pageSize));

            var dataFacet = AggregateFacet.Create("data", PipelineDefinition<TEntity, TEntity>.Create(pipelineDefinitions));
            List<AggregateFacetResults> list = null;
            filter = GetBaseFilter().AddExp(filter);
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                list = await Collection.Aggregate(Transaction.GetSessionHandle()).Match(filter).Facet(countFacet, dataFacet).ToListAsync();
            else
                list = await Collection.Aggregate().Match(filter).Facet(countFacet, dataFacet).ToListAsync();
            long totalCount = list.FirstOrDefault()?.Facets[0].Output<AggregateCountResult>().FirstOrDefault()?.Count ?? 0;
            var result = list.FirstOrDefault()?.Facets[1].Output<TEntity>();
            int pageCount = (Math.Ceiling(totalCount / (decimal)pageSize)).ToInt();
            return new PageModel<TEntity>() { TotalCount = totalCount, PageCount = pageCount, PageIndex = pageIndex, PageSize = pageSize, Data = result };
        }
        #endregion

        #region 插入

        public Task AddEntity(TEntity entity)
        {
            entity.CDate = DateTime.UtcNow;
            entity.MDate = DateTime.UtcNow;
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return Collection.InsertOneAsync(Transaction.GetSessionHandle(), entity);
            else
                return Collection.InsertOneAsync(entity);
        }

        public Task AddEntities(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.CDate = DateTime.UtcNow;
                entity.MDate = DateTime.UtcNow;
            });
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return Collection.InsertManyAsync(Transaction.GetSessionHandle(), entities);
            else
                return Collection.InsertManyAsync(entities);
        }
        #endregion

        #region 更新
        public async Task<bool> Update(TEntity entity)
        {
            entity.MDate = DateTime.UtcNow;
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return (await Collection.ReplaceOneAsync(Transaction.GetSessionHandle(), Builders<TEntity>.Filter.Eq("_id", entity.EntityId), entity)).ModifiedCount == 1;
            else
            {
                ReplaceOneResult respone = null;
                try
                {
                    respone = await Collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", entity.EntityId), entity);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return respone.ModifiedCount == 1;
            }
        }

        public async Task Update(List<TEntity> entities)
        {
            entities.ForEach(entity =>
            {
                entity.MDate = DateTime.UtcNow;
            });
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                await Task.WhenAll(entities.Select(e => Collection.ReplaceOneAsync(Transaction.GetSessionHandle(), Builders<TEntity>.Filter.Eq("_id", e.EntityId), e)));
            else
                await Task.WhenAll(entities.Select(e => Collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", e.EntityId), e)));
        }
        #endregion

        #region 删除
        public async Task<bool> Delete(TEntity entity)
        {
            return await DeleteById(entity.EntityId);
        }

        public async Task<long> Delete(Expression<Func<TEntity, bool>> filter)
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return (await Collection.DeleteManyAsync<TEntity>(Transaction.GetSessionHandle(), filter)).DeletedCount;
            else
                return (await Collection.DeleteManyAsync<TEntity>(filter)).DeletedCount;
        }

        public async Task<bool> DeleteById(object id)
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return (await Collection.DeleteOneAsync(Transaction.GetSessionHandle(), Builders<TEntity>.Filter.Eq("_id", id))).DeletedCount == 1;
            else
                return (await Collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id))).DeletedCount == 1;
        }

        public async Task<bool> DeleteByIds(object[] ids)
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return (await Collection.DeleteManyAsync(Transaction.GetSessionHandle(), Builders<TEntity>.Filter.In("_id", ids))).DeletedCount == ids.Length;
            else
                return (await Collection.DeleteManyAsync(Builders<TEntity>.Filter.In("_id", ids))).DeletedCount == ids.Length;
        }
        #endregion

        #region 计数
        public async Task<long> Count()
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return (await Collection.CountDocumentsAsync<TEntity>(Transaction.GetSessionHandle(), GetBaseFilter()));
            else
                return (await Collection.CountDocumentsAsync<TEntity>(GetBaseFilter()));
        }

        public async Task<long> Count(Expression<Func<TEntity, bool>> filter)
        {
            if (Transaction != null && Transaction.GetSessionHandle() != null)
                return (await Collection.CountDocumentsAsync<TEntity>(Transaction.GetSessionHandle(), filter));
            else
                return (await Collection.CountDocumentsAsync<TEntity>(filter));
        }
        #endregion
    }
}
