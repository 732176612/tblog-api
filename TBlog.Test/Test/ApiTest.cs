using Autofac;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TBlog.Extensions;
using TBlog.Repository;
using Xunit;
namespace TBlog.Test
{
    public class ApiTest
    {
        private readonly Init_Test Init = new();

        [Fact]
        public async Task CodeFirst()
        {
            await SqlSugarDBSeed.SeedAsync();
        }

        #region 分库分表测试
        [Fact]
        public async Task SplitTableCodeFirst()
        {
            DbScoped.SugarScope.CodeFirst.SplitTables().InitTables(typeof(SingleTableEntity));
            DbScoped.SugarScope.CodeFirst.InitTables(typeof(SplitTableEntity));
        }

        [Fact]
        public async Task InsertTest()
        {
            List<SingleTableEntity> entities = new List<SingleTableEntity>();
            for (int j = 0; j < 1; j++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    //1-5的随机数
                    Random random = new Random();
                    //随机五年内的一个时间
                    DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
                    DateTime startDate = date.AddSeconds(random.Next(0, 30));
                    DateTime endDate = startDate.AddSeconds(random.Next(0, 30));
                    //随机的IP地址
                    string ip = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}";
                    entities.Add(new SingleTableEntity
                    {
                        CDate = date,
                        StartDate = startDate,
                        EndDate = endDate,
                        IP = ip,
                        MDate = date,
                        RequestData = Guid.NewGuid().ToString(),
                        RequestMethod = Guid.NewGuid().ToString().Substring(0, 20),
                        ResponetData = Guid.NewGuid().ToString(),
                        Url = Guid.NewGuid().ToString(),
                        UserAgent = Guid.NewGuid().ToString(),
                        UserName = Guid.NewGuid().ToString().Substring(0, 20)
                    });
                }
            }
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            DbScoped.SugarScope.Fastest<SingleTableEntity>().BulkCopy(entities);
            stopwatch1.Stop();

            for (int j = 0; j < 1; j++)
            {
                var entities2 = new List<SplitTableEntity>();
                for (int i = 0; i < 1000000; i++)
                {
                    //1-5的随机数
                    Random random = new Random();
                    //随机五年内的一个时间
                    DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
                    DateTime startDate = date.AddSeconds(random.Next(0, 30));
                    DateTime endDate = startDate.AddSeconds(random.Next(0, 30));
                    //随机的IP地址
                    string ip = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}";
                    entities2.Add(new SplitTableEntity
                    {
                        CDate = date,
                        StartDate = startDate,
                        EndDate = endDate,
                        IP = ip,
                        MDate = date,
                        RequestData = Guid.NewGuid().ToString(),
                        RequestMethod = Guid.NewGuid().ToString().Substring(0, 20),
                        ResponetData = Guid.NewGuid().ToString(),
                        Url = Guid.NewGuid().ToString(),
                        UserAgent = Guid.NewGuid().ToString(),
                        UserName = Guid.NewGuid().ToString().Substring(0, 20)
                    });
                }
                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                DbScoped.SugarScope.Fastest<SplitTableEntity>().SplitTable().BulkCopy(entities2);
                stopwatch2.Stop();
            }
        }

        [Fact]
        public async Task DeleteTest()
        {
            int testCount = 1000;
            var entities = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            DbScoped.SugarScope.Deleteable(entities).SplitTable().ExecuteCommand();
            stopwatch1.Stop();

            var entities2 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch4 = new Stopwatch();
            stopwatch4.Start();
            DbScoped.SugarScope.Deleteable(entities2).ExecuteCommand();
            stopwatch4.Stop();

            var ids = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch3 = new Stopwatch();
            stopwatch3.Start();
            DbScoped.SugarScope.Deleteable<SplitTableEntity>().In(ids).SplitTable(c => c.Take(5)).ExecuteCommand();
            stopwatch3.Stop();

            var ids2 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            DbScoped.SugarScope.Deleteable<SingleTableEntity>().In(ids2).ExecuteCommand();
            stopwatch2.Stop();

            testCount = 10000;
            var entities3 = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch5 = new Stopwatch();
            stopwatch5.Start();
            DbScoped.SugarScope.Deleteable(entities3).SplitTable().ExecuteCommand();
            stopwatch5.Stop();

            var entities4 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch6 = new Stopwatch();
            stopwatch6.Start();
            DbScoped.SugarScope.Deleteable(entities4).ExecuteCommand();
            stopwatch6.Stop();

            var ids3 = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch7 = new Stopwatch();
            stopwatch7.Start();
            DbScoped.SugarScope.Deleteable<SplitTableEntity>().In(ids3).SplitTable(c => c.Take(5)).ExecuteCommand();
            stopwatch7.Stop();

            var ids4 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch8 = new Stopwatch();
            stopwatch8.Start();
            DbScoped.SugarScope.Deleteable<SingleTableEntity>().In(ids4).ExecuteCommand();
            stopwatch8.Stop();

            Random random = new Random();
            DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
            DateTime startDate = date.AddDays(random.Next(0, 100));
            DateTime endDate = startDate.AddDays(random.Next(0, 100));

            var entities5 = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch9 = new Stopwatch();
            stopwatch9.Start();
            DbScoped.SugarScope.Deleteable(entities5).SplitTable().ExecuteCommand();
            stopwatch9.Stop();

            var entities6 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch10 = new Stopwatch();
            stopwatch10.Start();
            DbScoped.SugarScope.Deleteable(entities6).ExecuteCommand();
            stopwatch10.Stop();
        }

        [Fact]
        public async Task UpdateTest()
        {
            int testCount = 1000;

            var entities = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            DbScoped.SugarScope.Updateable(entities).SplitTable().ExecuteCommand();
            stopwatch1.Stop();

            var entities2 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            DbScoped.SugarScope.Updateable(entities2).ExecuteCommand();
            stopwatch2.Stop();

            testCount = 10000;
            var entities3 = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch3 = new Stopwatch();
            stopwatch3.Start();
            DbScoped.SugarScope.Updateable(entities3).SplitTable().ExecuteCommand();
            stopwatch3.Stop();

            var entities4 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch4 = new Stopwatch();
            stopwatch4.Start();
            DbScoped.SugarScope.Updateable(entities4).ExecuteCommand();
            stopwatch4.Stop();

            Random random = new Random();
            DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
            DateTime startDate = date.AddDays(random.Next(0, 100));
            DateTime endDate = startDate.AddDays(random.Next(0, 100));

            var entities5 = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch5 = new Stopwatch();
            stopwatch5.Start();
            DbScoped.SugarScope.Updateable(entities5).SplitTable().ExecuteCommand();
            stopwatch5.Stop();

            var entities6 = DbScoped.SugarScope.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch6 = new Stopwatch();
            stopwatch6.Start();
            DbScoped.SugarScope.Updateable(entities6).ExecuteCommand();
            stopwatch6.Stop();
        }

        [Fact]
        public async Task QueryTest()
        {
            long bufenTotalCount = 0;
            long fenTotalCount = 0;

            for (int i = 0; i < 10; i++)
            {
                Random random = new Random();
                DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
                DateTime startDate = date.AddDays(random.Next(0, 100));
                DateTime endDate = startDate.AddDays(random.Next(0, 100));

                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                var list = DbScoped.SugarScope.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                var sqlitList = DbScoped.SugarScope.Queryable<SplitTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).SplitTable(startDate, endDate).ToList();
                stopwatch2.Stop();
                fenTotalCount += stopwatch2.ElapsedMilliseconds;
            }

            var bufenAverage = bufenTotalCount / 10;
            var fenAverage = fenTotalCount / 10;

        }

        [Fact]
        public async Task QueryTest2()
        {
            long bufenTotalCount = 0;
            long fenTotalCount = 0;

            for (int i = 0; i < 10; i++)
            {
                Random random = new Random();
                var ip = random.Next(1, 255).ToString();

                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                var list = DbScoped.SugarScope.Queryable<SingleTableEntity>().Where(c => c.IP.Contains(ip)).ToList();
                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                var sqlitList = DbScoped.SugarScope.Queryable<SplitTableEntity>().Where(c => c.IP.Contains(ip)).SplitTable().ToList();
                stopwatch2.Stop();
                fenTotalCount += stopwatch2.ElapsedMilliseconds;
            }

            var bufenAverage = bufenTotalCount / 10;
            var fenAverage = fenTotalCount / 10;
        }

        [Fact]
        public async Task QueryTest3()
        {
            long bufenTotalCount = 0;
            long fenTotalCount = 0;

            for (int i = 0; i < 10; i++)
            {
                Random random = new Random();
                DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
                DateTime startDate = date.AddDays(random.Next(0, 30));
                DateTime endDate = startDate.AddDays(random.Next(0, 30));

                var ids = DbScoped.SugarScope.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).Select(c => c.Id).ToList();
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                DbScoped.SugarScope.Queryable<SingleTableEntity>().In(ids).ToList();
                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                var ids2 = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable().Where(c => c.CDate > startDate && c.CDate < endDate).Select(c => c.Id).ToList();
                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                //DbScoped.SugarScope.Queryable<SplitTableEntity>().In(ids2).SplitTable(c => c.Take(5)).ToList();
                DbScoped.SugarScope.Queryable<SplitTableEntity>().In(ids2).SplitTable(startDate, endDate).ToList();
                stopwatch2.Stop();
                fenTotalCount += stopwatch2.ElapsedMilliseconds;
            }

            var bufenAverage = bufenTotalCount / 10;
            var fenAverage = fenTotalCount / 10;
        }

        [Fact]
        public async Task QueryTest4()
        {
            long bufenTotalCount = 0;
            long fenTotalCount = 0;

            for (int i = 0; i < 10; i++)
            {
                Random random = new Random();
                var ip = random.Next(1, 255).ToString();

                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                var list = DbScoped.SugarScope.Queryable<SingleTableEntity>()
                    .InnerJoin<IpAddressInfo>((x, y) => x.IP == y.Ip)
                    .Select((x, y) => new { x.IP, y.Address })
                    .ToList();

                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                var sqlitList = DbScoped.SugarScope.Queryable<SplitTableEntity>().SplitTable(x => x.Take(5)).InnerJoin<IpAddressInfo>((x, y) => x.IP == y.Ip).Select((x, y) => new { x.IP, y.Address }).ToList();
                stopwatch2.Stop();
                fenTotalCount += stopwatch2.ElapsedMilliseconds;
            }

            var bufenAverage = bufenTotalCount / 10;
            var fenAverage = fenTotalCount / 10;
        }
        #endregion

        #region MongoDB迁移MYSQL
        [Fact]
        public async Task Migration()
        {
            var mongoClient = ContainerHelper.Resolve<IMongoClient>();
            await SqlSugarDBSeed.SeedAsync();
            await DbScoped.SugarScope.BeginTranAsync();
            if (ApiConfig.DBSetting.SeedDBDataEnabled == false)
            {
                {
                    var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<RoleEntity>(typeof(RoleEntity).Name).Find(c => true).ToListAsync();
                    await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
                }
                {
                    var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<MenuEntity>(typeof(MenuEntity).Name).Find(c => true).ToListAsync();
                    await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
                }
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<OldActicleEntity>(typeof(ActicleEntity).Name).Find(c => true).ToListAsync();
                foreach (var item in entities.Where(c => c.Tags?.Any() == true))
                {
                    await DbScoped.SugarScope.Insertable(item.Tags.Select(c => new ActicleTagEntity
                    {
                        ActicleId = item.Id,
                        Name = c
                    }).ToList()).ExecuteCommandAsync();

                }
                await DbScoped.SugarScope.Insertable(entities.Select(c => new ActicleStatsEntity
                {
                    CDate = c.CDate,
                    Id = c.Id,
                    MDate = c.MDate,
                    ActicleId = c.Id,
                    LikeNum = c.LikeNum,
                    LookNum = c.LookNum
                }).ToList()).ExecuteCommandAsync();

                await DbScoped.SugarScope.Insertable(entities.Select(c => new ActicleEntity
                {
                    CDate = c.CDate,
                    Content = c.Content,
                    ActicleType = c.ActicleType,
                    CUserId = c.CUserId,
                    Id = c.Id,
                    IsDeleted = c.IsDeleted,
                    MDate = c.MDate,
                    PosterUrl = c.PosterUrl,
                    ReleaseForm = c.ReleaseForm,
                    Title = c.Title
                }).ToList()).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<ActicleHisLogEntity>(typeof(ActicleHisLogEntity).Name).Find(c => true).ToListAsync();
                await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<ActicleStatsEntity>(typeof(ActicleStatsEntity).Name).Find(c => true).ToListAsync();
                await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<ActicleTagEntity>(typeof(ActicleTagEntity).Name).Find(c => true).ToListAsync();
                await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<OldCompanyInfoEntity>(typeof(CompanyInfoEntity).Name).Find(c => true).ToListAsync();
                await DbScoped.SugarScope.Insertable(entities.Select(c => new CompanyInfoEntity
                {
                    StartDate = c.StartDate.Toyyyymmdd(),
                    CDate = c.CDate,
                    City = c.City,
                    Company = c.Company,
                    CUserId = c.CUserId,
                    Department = c.Department,
                    EndDate = c.EndDate.Toyyyymmdd(),
                    Id = c.Id,
                    Introduction = c.Introduction,
                    IsDeleted = c.IsDeleted,
                    MDate = c.MDate,
                    Position = c.Position
                }).ToList()).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<OldEduInfoEntity>(typeof(EduInfoEntity).Name).Find(c => true).ToListAsync();

                await DbScoped.SugarScope.Insertable(entities.Select(c => new EduInfoEntity
                {
                    CDate = c.CDate,
                    School = c.School,
                    StartDate = c.StartDate.Toyyyymmdd(),
                    CUserId = c.CUserId,
                    EndDate = c.EndDate.Toyyyymmdd(),
                    Id = c.Id,
                    Introduction = c.Introduction,
                    IsDeleted = c.IsDeleted,
                    Major = c.Major,
                    MDate = c.MDate
                }).ToList()).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<MediaInfoEntity>(typeof(MediaInfoEntity).Name).Find(c => true).ToListAsync();
                await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<OldProjectInfoEntity>(typeof(ProjectInfoEntity).Name).Find(c => true).ToListAsync();
                await DbScoped.SugarScope.Insertable(entities.Select(c => new ProjectInfoEntity
                {
                    StartDate = c.StartDate.Toyyyymmdd(),
                    CDate = c.CDate,
                    City = c.City,
                    CUserId = c.CUserId,
                    EndDate = c.EndDate.Toyyyymmdd(),
                    Id = c.Id,
                    Introduction = c.Introduction,
                    IsDeleted = c.IsDeleted,
                    MDate = c.MDate,
                    Project = c.Project,
                    Role = c.Role,
                }).ToList()).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<UserEntity>(typeof(UserEntity).Name).Find(c => true).ToListAsync();
                foreach (var item in entities)
                {
                    if (item.Sign.IsEmptyOrNull())
                    {
                        item.Sign = string.Empty;
                    }
                    if (item.ResumeUrl.IsEmptyOrNull())
                    {
                        item.ResumeUrl = string.Empty;
                    }
                    if (item.ResumeName.IsEmptyOrNull())
                    {
                        item.ResumeName = string.Empty;
                    }
                    if (item.StyleColor.IsEmptyOrNull())
                    {
                        item.StyleColor = string.Empty;
                    }
                    if (item.BackgroundUrl.IsEmptyOrNull())
                    {
                        item.BackgroundUrl = string.Empty;
                    }
                }
                await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
            }
            {
                var entities = await mongoClient.GetDatabase(ApiConfig.DBSetting.MongoDbName).GetCollection<SkillInfoEntity>(typeof(SkillInfoEntity).Name).Find(c => true).ToListAsync();
                await DbScoped.SugarScope.Insertable(entities).ExecuteCommandAsync();
            }
            await DbScoped.SugarScope.CommitTranAsync();
        }
        #endregion
    }
}