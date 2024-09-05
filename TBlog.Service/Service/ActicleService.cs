namespace TBlog.Service
{
    public class ActicleService : BaseService<ActicleEntity>, IActicleService
    {
        private readonly IActicleRepository _ActicleRepository;
        private readonly IActicleHisLogService _ActicleHisLogService;
        private readonly IActicleHisLogRepository _ActicleHisLogRepository;
        private readonly IRoleRepository _RoleRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IElasticClient _ElasticClient;
        public ActicleService(IActicleRepository acticleRepository, IRoleRepository roleRepository, IUserRepository userRepository,
            IActicleHisLogService acticleHisLogService, IActicleHisLogRepository acticleHisLogRepository,
            IElasticClient elasticClient)
        {
            _ActicleRepository = acticleRepository;
            _RoleRepository = roleRepository;
            _UserRepository = userRepository;
            _ActicleHisLogService = acticleHisLogService;
            _ActicleHisLogRepository = acticleHisLogRepository;
            _ElasticClient = elasticClient;
        }

        public async Task<string> SaveActicle(ActicleDto dto, long userId, string blogName)
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
            entity.CUserId = userId;
            entity.CBlogName = blogName;
            if (entity.Id != 0)
            {
                var existEntity = await _ActicleRepository.GetById(entity.Id);
                if (existEntity == null)
                {
                    var id = (await _ActicleRepository.AddEntity(entity)).ToString();
                    var respone = await _ElasticClient.IndexDocumentAsync(entity);
                    return id;
                }
                else
                {
                    if (existEntity.CUserId == entity.CUserId)
                    {
                        entity.CDate = existEntity.CDate;
                        await _ActicleRepository.Update(entity);
                        _ElasticClient.IndexDocument(entity);
                        return entity.Id.ToString();
                    }
                    else
                        throw new TBlogApiException("您无权修改当前文章");
                }
            }
            else
            {
                var id = (await _ActicleRepository.AddEntity(entity)).ToString();
                var respone = await _ElasticClient.IndexDocumentAsync(entity);
                return id;
            }

        }

        public async Task<bool> CheckRepeatTitle(long userId, string title)
        {
            return await _ActicleRepository.CountByUIdAndTitle(userId, title) > 0;
        }

        public async Task<ActicleDto> GetActicle(long id, long userid)
        {
            var entity = await _ActicleRepository.GetById(id);
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
            if (user == null)
            {
                throw new TBlogApiException("该博客不存在");
            }
            return await _ActicleRepository.GetTagsByUseId(user.Id, releaseForm);
        }

        public async Task<PageModel<ActicleDto>> GetActicleList(int pageIndex, int pageSize, string blogName,
            EnumActicleReleaseForm releaseForm, EnumActicleSortTag acticleSortTag, string tags = "",
            string searchVal = "")
        {
            var sortDir = new Dictionary<Expression<Func<ActicleEntity, object>>, bool>();
            switch (acticleSortTag)
            {
                case EnumActicleSortTag.Likes:
                    sortDir.Add(c => c.LikeNum, false);
                    break;
            }
            sortDir.Add(c => c.CDate, false);
            var filter = FilterHelper.AddExp<ActicleEntity>(c => c.CBlogName == blogName && c.ReleaseForm == releaseForm);
            if (!string.IsNullOrEmpty(tags))
            {
                string[] tagsSplit = tags.Split(',');
                if (tagsSplit.Any())
                {
                    filter = filter.AddExp(c => tagsSplit.Any(s => c.Tags.Contains(s)));
                }
            }

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
                                                .Filter(fi => fi.Term(b => b.CBlogName, blogName.ToLower()))
                                                )
                                            )
                                     );

                var acticleIds = searchResponse.Documents.Select(c => c.Id).ToArray();
                filter = filter.AddExp(c => acticleIds.Contains(c.Id));
            }

            var pages = await _ActicleRepository.GetPage(pageIndex, pageSize, filter, sortDir);
            var listData = pages.Data.ToDto<ActicleDto, ActicleEntity>();
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
            return pages.AsPageModel(listData);
        }

        public async Task LikeArticle(long id, long cuserid, string ipAddress)
        {
            var entity = await _ActicleRepository.GetById(id);
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
                entity.LikeNum = await _ActicleHisLogRepository.CountByActicleIdAndHisType(id, EnumActicleHisType.Like);
                await _ActicleRepository.Update(entity);
            }
            else
            {
                throw new TBlogApiException("您已经点赞过了!");
            }
        }

        public async Task LookArticle(long id, long cuserid, string ipAddress)
        {
            var entity = await _ActicleRepository.GetById(id);
            if (entity == null)
            {
                throw new TBlogApiException("不存在该文章");
            }

            if (entity.CUserId != cuserid && await _ActicleHisLogService.AddLog(new ActicleHisLogEntity
            {
                ActicleId = id,
                HisType = EnumActicleHisType.Look,
                IpAddress = ipAddress
            }))
            {

                entity.LookNum = await _ActicleHisLogRepository.CountByActicleIdAndHisType(id, EnumActicleHisType.Look);
                await _ActicleRepository.Update(entity);
            }
        }

        public async Task DeleteArticle(long id, long cuserid)
        {
            var entity = await _ActicleRepository.GetById(id);
            if (entity == null)
            {
                throw new TBlogApiException("不存在该文章");
            }

            if (entity.CUserId != cuserid)
            {
                throw new TBlogApiException("您无权删除当前文章");
            }

            entity.IsDeleted = true;
            await _ActicleRepository.Update(entity);
            var path = new DocumentPath<ActicleEntity>(entity).Index(ApiConfig.Elasticsearch.DefaultIndex);
            var respone = await _ElasticClient.DeleteAsync(path);
        }
    }
}
