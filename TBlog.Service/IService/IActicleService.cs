﻿namespace TBlog.Service
{
    public interface IActicleService : ISugarService<ActicleEntity>
    {
        public Task<ActicleDto> GetActicle(long id, long userid);

        public Task<PageModel<ActicleDto>> GetActicleList(int pageIndex, int pageSize, string blogName, EnumActicleReleaseForm acticleReleaseForm, EnumActicleSortTag acticleSortTag, string tags = "", string searchVal = "");

        public Task<string> SaveActicle(ActicleDto dto, long userId);

        public Task<bool> CheckRepeatTitle(long userId, string title);

        public Task<IEnumerable<string>> GetTagsByUseId(string blogName, EnumActicleReleaseForm releaseForm);

        public Task LikeArticle(long id, long cuserid, string ipAddress);

        public Task LookArticle(long id, long cuserid, string ipAddress);

        public Task DeleteArticle(long id,long cuserid);
    }
}