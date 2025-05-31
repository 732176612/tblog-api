using TBlog.Repository;

namespace TBlog.Service
{
    public class ActicleService : SugarService<ActicleEntity>, IActicleService
    {
        private readonly IActicleHisLogService _ActicleHisLogService;
        private readonly IUserRepository _UserRepository;
        private readonly IElasticClient _ElasticClient;
        public ActicleService(IUserRepository userRepository, IActicleHisLogService acticleHisLogService, IElasticClient elasticClient)
        {
            _UserRepository = userRepository;
            _ActicleHisLogService = acticleHisLogService;
            _ElasticClient = elasticClient;
        }

        public async Task<string> SaveActicle(ActicleDto dto, long userId)
        {
            var entity = dto.ToEntity<ActicleEntity, ActicleDto>();
            var titleIsExist = await CheckRepeatTitle(userId, entity.Title);
            if (dto.ReleaseForm != EnumActicleReleaseForm.Draft && entity.Id == 0 && titleIsExist)
            {
                throw new TBlogApiException("文章标题重复");
            }

            var tran = await DbScoped.SugarScope.UseTranAsync(async () =>
            {
                entity.CUserId = userId;

                if (entity.Id != 0)
                {
                    var existEntity = await Repository.DBQuery.InSingleAsync(entity.Id);
                    if (existEntity != null)
                    {
                        if (existEntity.CUserId != entity.CUserId) throw new TBlogApiException("您无权修改当前文章");
                        entity.CDate = existEntity.CDate;
                        entity.MDate = DateTime.Now;
                        await DbScoped.SugarScope.UpdateNav(entity).Include(s => s.Tags).ExecuteCommandAsync();
                        return;
                    }
                }

                entity.CDate = DateTime.Now;
                entity.MDate = DateTime.Now;
                entity.Id = SnowFlakeSingle.instance.NextId();
                entity.Stats.LookNum = 0;
                entity.Stats.LikeNum = 0;
                await DbScoped.SugarScope.InsertNav(entity).Include(c => c.Tags).Include(c => c.Stats).ExecuteCommandAsync();
            });

            //if (tran.IsSuccess)
            //{
            //    await CommonHelper.ExceptionRetry((() => _ElasticClient.IndexDocumentAsync(entity).Result.IsValid), 3);
            //}

            return entity.Id.ToString();
        }

        public async Task<bool> CheckRepeatTitle(long userId, string title)
        {
            return await Repository.DBQuery.AnyAsync(c => c.CUserId == userId && c.Title == title);
        }

        public async Task<ActicleDto> GetActicle(long id, long userid)
        {
            var entity = await Repository.DBQuery.Includes(c => c.Tags).Includes(c => c.Stats).InSingleAsync(id);
            if (entity == null)
            {
                throw new TBlogApiException("不存在该文章");
            }
            if (entity.ReleaseForm == EnumActicleReleaseForm.Private && entity.CUserId != userid)
            {
                throw new TBlogApiException("您无权查看当前文章");
            }
            var dto = entity.ToDto<ActicleDto, ActicleEntity>();
            return dto;
        }

        public async Task<IEnumerable<string>> GetTagsByUseId(string blogName, EnumActicleReleaseForm releaseForm)
        {
            var user = await _UserRepository.GetByBlogName(blogName);
            if (user == null) throw new TBlogApiException("该博客不存在");
            return await Repository.DBQuery.Where(c => c.CUserId == user.Id && c.ReleaseForm == releaseForm)
                        .InnerJoin<ActicleTagEntity>((c, y) => c.Id == y.ActicleId)
                        .GroupBy((c, y) => y.Name)
                        .Select((c, y) => y.Name)
                        .ToListAsync();
        }

        public async Task<PageModel<ActicleDto>> GetActicleList(int pageIndex, int pageSize, string blogName,
            EnumActicleReleaseForm releaseForm, EnumActicleSortTag acticleSortTag, string tags = "", string searchVal = "")
        {
            if (string.IsNullOrEmpty(blogName)) throw new TBlogApiException("系统异常，请稍后重试");
            long cuserId = (await _UserRepository.GetByBlogName(blogName))?.Id ?? 0;

            List<long> filterActicleIds = new List<long>();
            if (!string.IsNullOrEmpty(searchVal))
            {
                var searchResponse = await _ElasticClient.SearchAsync<ActicleEntity>(s => s
                                        .From(0)
                                        .Size(10000)
                                         .Query(q => q
                                            .Bool(b => b
                                                .Must(c => c.
                                                    QueryString(c => c.
                                                        Fields(d => d.
                                                            Fields(q => q.Content, q => q.Title))
                                                        .Query(searchVal))
                                                    )
                                                   .Filter(fi => fi.Term(b => b.CUserId, cuserId))
                                                )
                                            )
                                         );

                filterActicleIds = searchResponse.Documents.Select(c => c.Id).ToList();
            }

            string[] tagsSplit = tags?.Split(',').Where(c => c.IsNotEmptyOrNull()).ToArray() ?? [];

            var queryPage = await Repository.DBQuery.Where(c => c.CUserId == cuserId && c.ReleaseForm == releaseForm)
                            .Includes(c => c.Stats)
                            .Includes(c => c.Tags)
                            .WhereIF(tagsSplit.Any(), c => c.Tags.Any(s => tagsSplit.Contains(s.Name)))
                            .WhereIF(filterActicleIds.Count > 0, c => filterActicleIds.Contains(c.Id))
                            .OrderByIF(acticleSortTag == EnumActicleSortTag.Likes, c => c.Stats.LikeNum, OrderByType.Desc)
                            .OrderByDescending(c => c.CDate)
                            .ToPageModel(pageIndex, pageSize);

            var listData = queryPage.Data.ToDto<ActicleDto, ActicleEntity>();

            foreach (var item in listData)
            {
                item.Content = item.Content.ClearHtmlTag();
                item.CBlogName = blogName;
                if (!string.IsNullOrEmpty(searchVal))
                {
                    item.Title = item.Title.Replace(searchVal, $"<b style='color:red;'>{searchVal}</b>");

                    if (item.Content.Length > 100)
                    {
                        int contentIndex = item.Content.IndexOf(searchVal);
                        if (contentIndex == -1)
                        {
                            foreach (var charItem in searchVal)
                            {
                                contentIndex = item.Content.IndexOf(charItem);
                                if (contentIndex != -1) break;
                            }
                        }

                        if (contentIndex != -1)
                        {
                            int startIndex = contentIndex - 50 > 0 ? contentIndex - 50 : 0;
                            int offest = item.Content.Length - startIndex - 100;
                            if (offest < 0)
                            {
                                startIndex += offest;
                                if (startIndex <= 0)
                                {
                                    startIndex = 0;
                                }
                            }
                            item.Content = item.Content.Substring(startIndex, 100);
                        }
                        else
                        {
                            item.Content = item.Content.Substring(0, 100);
                        }
                    }
                    foreach (var charItem in searchVal)
                    {
                        item.Content = item.Content.Replace(charItem.ToString(), $"<b style='color:red;'>{charItem}</b>");
                    }
                    item.Content += "...";
                }
                else
                {
                    if (item.Content.Length > 100)
                    {
                        item.Content = item.Content.Substring(0, 100);
                    }
                }
            }

            return queryPage.AsPageModel(listData);
        }

        public async Task LikeArticle(long id, long cuserid, string ipAddress)
        {
            var entity = await Repository.DBQuery.InSingleAsync(id);
            if (entity == null) throw new TBlogApiException("不存在该文章");
            if (entity.CUserId == cuserid) throw new TBlogApiException("不能给自己点赞");
            var isAdd = await _ActicleHisLogService.AddLog(new ActicleHisLogEntity
            {
                ActicleId = id,
                HisType = EnumActicleHisType.Like,
                CUserId = cuserid == 0 ? ipAddress.Replace(".", "").ToLong() : cuserid
            });
            if (isAdd == false) throw new TBlogApiException("您已经点赞过了!");
            await DbScoped.SugarScope.Updateable<ActicleStatsEntity>()
            .SetColumns(c => c.LikeNum ==
                SqlFunc.Subqueryable<ActicleHisLogEntity>()
                .Where(s => s.ActicleId == id && s.HisType == EnumActicleHisType.Like)
                .Count())
            .Where(c => c.ActicleId == id)
            .ExecuteCommandAsync();
        }

        public async Task LookArticle(long id, long cuserid, string ipAddress)
        {
            var entity = await Repository.DBQuery.InSingleAsync(id);
            if (entity == null) throw new TBlogApiException("不存在该文章");
            if (entity.CUserId == cuserid) return;
            var isAdd = await _ActicleHisLogService.AddLog(new ActicleHisLogEntity
            {
                ActicleId = id,
                HisType = EnumActicleHisType.Look,
                CUserId = cuserid == 0 ? ipAddress.Replace(".", "").ToLong() : cuserid
            });
            if (isAdd)
            {
                await DbScoped.SugarScope.Updateable<ActicleStatsEntity>()
                .SetColumns(c => c.LookNum ==
                    SqlFunc.Subqueryable<ActicleHisLogEntity>()
                    .Where(s => s.ActicleId == id && s.HisType == EnumActicleHisType.Look)
                    .Count())
                .Where(c => c.ActicleId == id)
                .ExecuteCommandAsync();
            }
        }

        public async Task DeleteArticle(long id, long cuserid)
        {
            var entity = await Repository.DBQuery.InSingleAsync(id);
            if (entity == null) throw new TBlogApiException("不存在该文章");
            if (entity.CUserId != cuserid) throw new TBlogApiException("您无权删除当前文章");
            await Repository.DeleteByIds(id);
            var path = new DocumentPath<ActicleEntity>(entity).Index(ApiConfig.Elasticsearch.DefaultIndex);
            var respone = await _ElasticClient.DeleteAsync(path);
        }
    }
}
