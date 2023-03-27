using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.Model;

namespace TBlog.Repository
{
    public class SqlSugarDBSeed
    {
        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        /// <returns></returns>
        public static async void SeedAsync(ISqlSugarClient _db)
        {
            if (ApiConfig.DBSetting.SeedDBEnabled)
                try
                {
                    Console.WriteLine("************ TBlog DataBase Set *****************");
                    Console.WriteLine($"Is CQRS: {ApiConfig.DBSetting.CQRSEnabled}");
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
                        _db.DbMaintenance.CreateDatabase();
                        $"Database created successfully!".WriteSuccessLine();
                    }
                    else
                    {
                        $"Oracle 数据库不支持该操作，可手动创建Oracle数据库!".WriteSuccessLine();
                    }

                    Console.WriteLine("Create Tables...");
                    var modelTypes = (from t in Assembly.GetAssembly(typeof(IEntity)).GetTypes()
                                      where t.IsClass && t.Namespace == "TBlog.Model" && t.IsAssignableTo(typeof(IEntity))
                                      select t).ToList();
                    modelTypes.ForEach(t =>
                    {
                        if (!_db.DbMaintenance.IsAnyTable(t.Name))
                        {
                            Console.WriteLine(t.Name);
                            var test = typeof(ActicleEntity).GetProperties().Select(c => c.GetCustomAttribute<SugarColumn>()).ToArray();
                            _db.CodeFirst.InitTables(t);
                        }
                    });
                    $"Tables created successfully!".WriteSuccessLine();
                    Console.WriteLine();

                    if (ApiConfig.DBSetting.SeedDBDataEnabled)
                    {
                        JsonSerializerSettings setting = new JsonSerializerSettings();
                        JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
                        {
                            //日期类型默认格式化处理
                            setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                            //空值处理
                            setting.NullValueHandling = NullValueHandling.Ignore;
                            return setting;
                        });

                        #region UserEntity
                        var typeName = "UserEntity";
                        if (!await _db.Queryable<UserEntity>().AnyAsync())
                        {
                            string consoleVal = $"Table:{typeName} already exists...";
                            var fileStr = FileHelper.ReadFile(string.Format(ApiConfig.DBSetting.SeedDataFolderPath, typeName), Encoding.UTF8);
                            if (string.IsNullOrEmpty(fileStr))
                            {
                                new SimpleClient<UserEntity>(_db).InsertRange(JsonHelper.ParseFormByJson<List<UserEntity>>(fileStr));
                                consoleVal = $"Table:{typeName} created success!";
                            }
                            Console.WriteLine(consoleVal);
                        }
                        #endregion
                        $"Done seeding database!".WriteSuccessLine();
                    }
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"1、若是Mysql,查看常见问题:https://github.com/anjoy8/TBlog/issues/148#issue-776281770 \n" +
                        $"2、若是Oracle,查看常见问题:https://github.com/anjoy8/TBlog/issues/148#issuecomment-752340231 \n" +
                        "3、其他错误：" + ex.Message);
                }
        }
    }
}