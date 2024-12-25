using TBlog.Repository;

namespace TBlog.Service
{
    public class ActicleService : SugarService<IActicleRepository, ActicleEntity>, IActicleService
    {
        private readonly IActicleHisLogService _ActicleHisLogService;
        private readonly IActicleHisLogRepository _ActicleHisLogRepository;
        private readonly IRoleRepository _RoleRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IElasticClient _ElasticClient;
        public ActicleService(IActicleRepository acticleRepository, IRoleRepository roleRepository, IUserRepository userRepository,
            IActicleHisLogService acticleHisLogService, IActicleHisLogRepository acticleHisLogRepository,
            IElasticClient elasticClient)
        {
            _RoleRepository = roleRepository;
            _UserRepository = userRepository;
            _ActicleHisLogService = acticleHisLogService;
            _ActicleHisLogRepository = acticleHisLogRepository;
            _ElasticClient = elasticClient;
        }

        public async Task<string> SaveActicle(ActicleDto dto, long userId)
        {
            ActicleEntity entity;
            try
            {
                entity = dto.ToEntity<ActicleEntity, ActicleDto>(dto.ReleaseForm != EnumActicleReleaseForm.Draft);//草稿不校验字段
            }
            catch (Exception ex)
            {
                throw new TBlogApiException(ex.Message);
            }

            if (dto.ReleaseForm != EnumActicleReleaseForm.Draft && entity.Id == 0 && await CheckRepeatTitle(userId, entity.Title))
            {
                throw new TBlogApiException("文章标题重复");
            }

            var tran = await DBHelper.DB.UseTranAsync(async () =>
            {
                entity.CUserId = userId;

                if (entity.Id != 0)
                {
                    var existEntity = await Repository.DBQuery.InSingleAsync(entity.Id);
                    if (existEntity != null)
                    {
                        if (existEntity.CUserId != entity.CUserId) throw new TBlogApiException("您无权修改当前文章");
                        entity.CDate = existEntity.CDate;
                        await Repository.Update(entity);
                        return;
                    }
                }

                entity.Id = await DBHelper.DB.Insertable(entity).ExecuteReturnBigIdentityAsync();
                await DBHelper.DB.Insertable(new ActicleStatsEntity { ActicleId = entity.Id }).ExecuteCommandAsync();
            });

            if (tran.IsSuccess)
            {
                await CommonHelper.ExceptionRetry((() => _ElasticClient.IndexDocumentAsync(entity).Result.IsValid), 3);
                //插入失败的写入日志处理
            }

            return entity.Id.ToString();
        }

        public async Task<bool> CheckRepeatTitle(long userId, string title)
        {
            return await Repository.CountByUIdAndTitle(userId, title) > 0;
        }

        public async Task<ActicleDto> GetActicle(long id, long userid)
        {
            var entity = await Repository.DBQuery.InSingleAsync(id);
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
            return await Repository.GetTagsByUseId(user.Id, releaseForm);
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

            string[] tagsSplit = tags?.Split(',') ?? [];

            var queryPage = await Repository.DBQuery.Where(c => c.CUserId == cuserId && c.ReleaseForm == releaseForm)
                            .WhereIF(filterActicleIds.Count > 0, c => filterActicleIds.Contains(c.Id))
                            .Includes(c => c.Tags)
                            .Where(c => tagsSplit.Any(s => c.Tags.Select(q => q.Name).Contains(s)))
                            .OrderByIF(acticleSortTag == EnumActicleSortTag.Likes, c => c.Stats.LikeNum, OrderByType.Desc)
                            .OrderBy(c => c.CDate)
                            .ToPageModel(pageIndex, pageSize);

            var listData = queryPage.Data.ToDto<ActicleDto, ActicleEntity>();

            foreach (var item in listData)
            {
                item.Content = item.Content.ClearHtmlTag();

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

        [Transaction]
        public async Task LikeArticle(long id, long cuserid, string ipAddress)
        {
            var entity = await Repository.DBQuery.InSingleAsync(id);
            if (entity == null)
            {
                throw new TBlogApiException("不存在该文章");
            }

            if (entity.CUserId == cuserid)
            {
                throw new TBlogApiException("不能给自己点赞");
            }

            if (await _ActicleHisLogService.AddLog(new ActicleHisLogEntity
            {
                ActicleId = id,
                HisType = EnumActicleHisType.Like,
                IpAddress = ipAddress
            }))
            {
                var likeNum = await _ActicleHisLogRepository.CountByActicleIdAndHisType(id, EnumActicleHisType.Like);
                await DBHelper.DB.Updateable<ActicleStatsEntity>().SetColumns(c => c.LikeNum == likeNum + 1).ExecuteCommandAsync();
            }
            else
            {
                throw new TBlogApiException("您已经点赞过了!");
            }
        }

        [Transaction]
        public async Task LookArticle(long id, long cuserid, string ipAddress)
        {
            var entity = await Repository.DBQuery.InSingleAsync(id);
            if (entity == null) throw new TBlogApiException("不存在该文章");
            if (entity.CUserId == cuserid) return;
            await _ActicleHisLogService.AddLog(new ActicleHisLogEntity
            {
                ActicleId = id,
                HisType = EnumActicleHisType.Look,
                IpAddress = ipAddress
            });
            var LookNum = await _ActicleHisLogRepository.CountByActicleIdAndHisType(id, EnumActicleHisType.Look);
            await DBHelper.DB.Updateable<ActicleStatsEntity>().SetColumns(c => c.LookNum == LookNum + 1).ExecuteCommandAsync();
        }

        public async Task DeleteArticle(long id, long cuserid)
        {
            var entity = await Repository.DBQuery.InSingleAsync(id);
            if (entity == null)
            {
                throw new TBlogApiException("不存在该文章");
            }

            if (entity.CUserId != cuserid)
            {
                throw new TBlogApiException("您无权删除当前文章");
            }

            entity.IsDeleted = true;
            await Repository.Update(entity);
            var path = new DocumentPath<ActicleEntity>(entity).Index(ApiConfig.Elasticsearch.DefaultIndex);
            var respone = await _ElasticClient.DeleteAsync(path);
        }
    }
}
