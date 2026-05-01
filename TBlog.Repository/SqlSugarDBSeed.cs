using Newtonsoft.Json;
using System.Reflection;
using System.Text;
namespace TBlog.Repository
{
    public class SqlSugarDBSeed
    {
        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        public static async Task SeedAsync()
        {
            if (ApiConfig.DBSetting.SeedDBEnabled)
                try
                {
                    Console.WriteLine("************ TBlog DataBase Set *****************");
                    Console.WriteLine();
                    Console.WriteLine($"Master DB Type: {ApiConfig.DBSetting.MainDB.DBType}");
                    Console.WriteLine($"Master DB ConnectString: {ApiConfig.DBSetting.MainDB.Connection}");
                    Console.WriteLine($"--------------------------------------");
                    var slaveIndex = 0;
                    ApiConfig.DBSetting.SlaveDBs.ToList().ForEach((m) =>
                    {
                        slaveIndex++;
                        Console.WriteLine($"Slave{slaveIndex} DB ID: {slaveIndex}");
                        Console.WriteLine($"Slave{slaveIndex} DB Type: {m.DBType}");
                        Console.WriteLine($"Slave{slaveIndex} DB ConnectString: {m.Connection}");
                        Console.WriteLine($"--------------------------------------");
                    });

                    Console.WriteLine("Create Database...");
                    if (ApiConfig.DBSetting.MainDB.DBType != DbType.Oracle.ToString())
                    {
                        DbScoped.SugarScope.DbMaintenance.CreateDatabase();
                        $"Database created successfully!".WriteSuccessLine();
                    }
                    else
                    {
                        $"Oracle 数据库不支持该操作，可手动创建Oracle数据库!".WriteSuccessLine();
                    }

                    Console.WriteLine("Create Tables...");
                    var modelTypes = (from t in Assembly.GetAssembly(typeof(IEntity)).GetTypes()
                                      where t.IsClass && t.Namespace == "TBlog.Model" && t.IsAssignableTo(typeof(IEntity)) && t != typeof(HttpLogEntity)
                                      select t).ToList();
                    foreach (var t in modelTypes)
                        Console.WriteLine($"初始化数据库表:[{t.Name}]");
                    // 一次性初始化：PostgreSQL 等库要求外键引用的父表先存在，逐个 InitTables 时反射顺序不定会导致 42P01
                    DbScoped.SugarScope.CodeFirst.InitTables(modelTypes.ToArray());
                    $"Tables created successfully!".WriteSuccessLine();
                    Console.WriteLine();

                    if (ApiConfig.DBSetting.SeedDBDataEnabled)
                    {
                        //如果不存在数据则插入
                        var userCount = await DbScoped.SugarScope.Queryable<MenuEntity>().CountAsync();
                        if (userCount == 0)
                        {
                            await DbScoped.SugarScope.Insertable(new List<MenuEntity>()
                        {
                            new MenuEntity()
                            {
                                Enabled = true,
                                OrderSort  =1,
                                Name= "首页",
                                Url="/view/index",
                            },
                            new MenuEntity()
                            {
                                Enabled = true,
                                OrderSort  =2,
                                Name= "文章",
                                Url="/view/acticleList",
                            }
                        }).ExecuteCommandAsync();
                        }

                        var roleCount = await DbScoped.SugarScope.Queryable<RoleEntity>().CountAsync();
                        if (roleCount == 0)
                        {
                            await DbScoped.SugarScope.Insertable(new List<RoleEntity>()
                            {
                                new RoleEntity()
                                {
                                    Id = 10000,
                                    OrderSort = 1,
                                    Desc="超级管理员",
                                    Name =ConstHelper.SystemRole
                                },
                                new RoleEntity()
                                {
                                    Id = 20000,
                                    OrderSort = 2,
                                    Desc = "普通用户",
                                    Name =ConstHelper.UserRole
                                }
                            }).ExecuteCommandAsync();
                        }

                        $"Done seeding database!".WriteSuccessLine();
                    }
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
        }
    }
}