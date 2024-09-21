using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using TBlog.Common;
using TBlog.IService;
using TBlog.Model;
using TBlog.Repository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using TBlog.IRepository;
using TBlog.Extensions;
using TencentCloud.Cam.V20190116.Models;
using System.Threading.Tasks;
using System.Diagnostics;
namespace TBlog.Test
{
    public class ApiTest
    {
        Init_Test Init = new Init_Test();

        [Fact]
        public async Task CodeFirst()
        {
            DBHelper.DB.CodeFirst.InitTables(typeof(UserEntity));
        }

        #region 分库分表测试
        [Fact]
        public async Task SplitTableCodeFirst()
        {
            DBHelper.DB.CodeFirst.SplitTables().InitTables(typeof(SingleTableEntity));
            DBHelper.DB.CodeFirst.InitTables(typeof(SplitTableEntity));
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
            DBHelper.DB.Fastest<SingleTableEntity>().BulkCopy(entities);
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
                DBHelper.DB.Fastest<SplitTableEntity>().SplitTable().BulkCopy(entities2);
                stopwatch2.Stop();
            }
        }

        [Fact]
        public async Task DeleteTest()
        {
            int testCount = 1000;
            var entities = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            DBHelper.DB.Deleteable(entities).SplitTable().ExecuteCommand();
            stopwatch1.Stop();

            var entities2 = DBHelper.DB.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch4 = new Stopwatch();
            stopwatch4.Start();
            DBHelper.DB.Deleteable(entities2).ExecuteCommand();
            stopwatch4.Stop();

            var ids = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch3 = new Stopwatch();
            stopwatch3.Start();
            DBHelper.DB.Deleteable<SplitTableEntity>().In(ids).SplitTable(c => c.Take(5)).ExecuteCommand();
            stopwatch3.Stop();

            var ids2 = DBHelper.DB.Queryable<SingleTableEntity>().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            DBHelper.DB.Deleteable<SingleTableEntity>().In(ids2).ExecuteCommand();
            stopwatch2.Stop();

            testCount = 10000;
            var entities3 = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch5 = new Stopwatch();
            stopwatch5.Start();
            DBHelper.DB.Deleteable(entities3).SplitTable().ExecuteCommand();
            stopwatch5.Stop();

            var entities4 = DBHelper.DB.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch6 = new Stopwatch();
            stopwatch6.Start();
            DBHelper.DB.Deleteable(entities4).ExecuteCommand();
            stopwatch6.Stop();

            var ids3 = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch7 = new Stopwatch();
            stopwatch7.Start();
            DBHelper.DB.Deleteable<SplitTableEntity>().In(ids3).SplitTable(c => c.Take(5)).ExecuteCommand();
            stopwatch7.Stop();

            var ids4 = DBHelper.DB.Queryable<SingleTableEntity>().Take(testCount).Select(c => c.Id).ToList();
            Stopwatch stopwatch8 = new Stopwatch();
            stopwatch8.Start();
            DBHelper.DB.Deleteable<SingleTableEntity>().In(ids4).ExecuteCommand();
            stopwatch8.Stop();

            Random random = new Random();
            DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
            DateTime startDate = date.AddDays(random.Next(0, 100));
            DateTime endDate = startDate.AddDays(random.Next(0, 100));

            var entities5 = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch9 = new Stopwatch();
            stopwatch9.Start();
            DBHelper.DB.Deleteable(entities5).SplitTable().ExecuteCommand();
            stopwatch9.Stop();

            var entities6 = DBHelper.DB.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch10 = new Stopwatch();
            stopwatch10.Start();
            DBHelper.DB.Deleteable(entities6).ExecuteCommand();
            stopwatch10.Stop();
        }

        [Fact]
        public async Task UpdateTest()
        {
            int testCount = 1000;

            var entities = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            DBHelper.DB.Updateable(entities).SplitTable().ExecuteCommand();
            stopwatch1.Stop();

            var entities2 = DBHelper.DB.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            DBHelper.DB.Updateable(entities2).ExecuteCommand();
            stopwatch2.Stop();

            testCount = 10000;
            var entities3 = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Take(testCount).ToList();
            Stopwatch stopwatch3 = new Stopwatch();
            stopwatch3.Start();
            DBHelper.DB.Updateable(entities3).SplitTable().ExecuteCommand();
            stopwatch3.Stop();

            var entities4 = DBHelper.DB.Queryable<SingleTableEntity>().Take(testCount).ToList();
            Stopwatch stopwatch4 = new Stopwatch();
            stopwatch4.Start();
            DBHelper.DB.Updateable(entities4).ExecuteCommand();
            stopwatch4.Stop();

            Random random = new Random();
            DateTime date = DateTime.Now.AddDays(-1 * (new Random().Next(0, 1825)));
            DateTime startDate = date.AddDays(random.Next(0, 100));
            DateTime endDate = startDate.AddDays(random.Next(0, 100));

            var entities5 = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch5 = new Stopwatch();
            stopwatch5.Start();
            DBHelper.DB.Updateable(entities5).SplitTable().ExecuteCommand();
            stopwatch5.Stop();

            var entities6 = DBHelper.DB.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
            Stopwatch stopwatch6 = new Stopwatch();
            stopwatch6.Start();
            DBHelper.DB.Updateable(entities6).ExecuteCommand();
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
                var list = DBHelper.DB.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).ToList();
                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                var sqlitList = DBHelper.DB.Queryable<SplitTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).SplitTable(startDate, endDate).ToList();
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
                var list = DBHelper.DB.Queryable<SingleTableEntity>().Where(c => c.IP.Contains(ip)).ToList();
                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                var sqlitList = DBHelper.DB.Queryable<SplitTableEntity>().Where(c => c.IP.Contains(ip)).SplitTable().ToList();
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

                var ids = DBHelper.DB.Queryable<SingleTableEntity>().Where(c => c.CDate > startDate && c.CDate < endDate).Select(c => c.Id).ToList();
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                DBHelper.DB.Queryable<SingleTableEntity>().In(ids).ToList();
                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                var ids2 = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable().Where(c => c.CDate > startDate && c.CDate < endDate).Select(c => c.Id).ToList();
                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                //DBHelper.DB.Queryable<SplitTableEntity>().In(ids2).SplitTable(c => c.Take(5)).ToList();
                DBHelper.DB.Queryable<SplitTableEntity>().In(ids2).SplitTable(startDate, endDate).ToList();
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
                var list = DBHelper.DB.Queryable<SingleTableEntity>()
                    .InnerJoin<IpAddressInfo>((x, y) => x.IP == y.Ip)
                    .Select((x, y) => new { x.IP, y.Address })
                    .ToList();

                stopwatch1.Stop();
                bufenTotalCount += stopwatch1.ElapsedMilliseconds;

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                var sqlitList = DBHelper.DB.Queryable<SplitTableEntity>().SplitTable(x => x.Take(5)).InnerJoin<IpAddressInfo>((x, y) => x.IP == y.Ip).Select((x, y) => new { x.IP, y.Address }).ToList();
                stopwatch2.Stop();
                fenTotalCount += stopwatch2.ElapsedMilliseconds;
            }

            var bufenAverage = bufenTotalCount / 10;
            var fenAverage = fenTotalCount / 10;
        }
        #endregion

        [Fact]
        public async Task InitRole()
        {
            var roleRepository = ContainerHelper.Resolve<IRoleRepository>();

            var systemRole = new RoleEntity()
            {
                Id = 10000,
                Desc = "超级管理员",
                Enabled = true,
                Name = ConstHelper.SystemRole,
                OrderSort = 0
            };

            await DBHelper.DB.Insertable(systemRole).ExecuteCommandAsync();

            var userRole = new RoleEntity()
            {
                Id = 20000,
                OrderSort = 1,
                PId = (await roleRepository.GetByName(ConstHelper.SystemRole)).Id,
                Name = ConstHelper.UserRole,
                Enabled = true,
                Desc = "普通用户"
            };

            await DBHelper.DB.Insertable(userRole).ExecuteCommandAsync();
        }

        [Fact]
        public async void InitMenu()
        {
            var menuRepository = ContainerHelper.Resolve<IMenuRepository>();

            await menuRepository.DBDelete.Where(c => true).ExecuteCommandAsync();

            var indexEntity = new MenuEntity()
            {
                Id = 10000,
                Url = "/view/index",
                Enabled = true,
                Name = "首页",
                RoleIds = new long[] { 10000, 20000 },
                OrderSort = 1
            };
            await DBHelper.DB.Insertable(indexEntity).ExecuteCommandAsync();

            var articleEntity = new MenuEntity()
            {
                Id = 20000,
                OrderSort = 2,
                Name = "文章",
                Enabled = true,
                RoleIds = new long[] { 10000, 20000 },
                Url = "/view/index/article"
            };
            await DBHelper.DB.Insertable(articleEntity).ExecuteCommandAsync();

            var userInfoEntity = new MenuEntity()
            {
                Id = 30000,
                OrderSort = 3,
                Name = "个人信息",
                Enabled = true,
                RoleIds = new long[] { 10000, 20000 },
                Url = "/view/index/userinfo"
            };

            await DBHelper.DB.Insertable(userInfoEntity).ExecuteCommandAsync();
        }
    }
}