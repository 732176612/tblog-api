namespace TBlog.Repository
{
    public class ActicleRepository : SugarRepository<ActicleEntity>, IActicleRepository
    {
        public async Task<long> CountByUIdAndTitle(long userid, string title)
        {
            return await DBQuery.CountAsync(c => c.CUserId == userid && c.Title == title);
        }

        public async Task<IEnumerable<string>> GetTagsByUseId(long userid, EnumActicleReleaseForm releaseForm)
        {
            return await DBHelper.DB.Queryable<ActicleEntity>().Where(c => c.CUserId == userid && c.ReleaseForm == releaseForm)
                        .InnerJoin<ActicleTagEntity>((c, y) => c.Id == y.ActicleId)
                        .GroupBy((c, y) => y.Name)
                        .Select((c, y) => y.Name)
                        .ToListAsync();
        }
    }
}