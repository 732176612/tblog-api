using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using TBlog.Extensions;
using System.Reflection;
using System.ComponentModel;

namespace TBlog.Api
{
    /// <summary>
    /// 公用API
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ILogger<CommonController> _logger;

        public CommonController(ILogger<CommonController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取正则表达式 (Phone,Mail,PassWord,BlogName)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public APITResult<List<TreeNodeModel>> VerifyRegex(string regexName)
        {
            List<TreeNodeModel> result = new List<TreeNodeModel>();
            var regexSplit = regexName.Split(',');
            foreach (var regex in regexSplit)
            {
                switch (regex)
                {
                    case "Phone":
                        result.Add(new TreeNodeModel() { Key = "Phone", Value = ConstHelper.PhoneRegex });
                        break;
                    case "Mail":
                        result.Add(new TreeNodeModel() { Key = "Mail", Value = ConstHelper.MailRegex });
                        break;
                    case "PassWord":
                        result.Add(new TreeNodeModel() { Key = "PassWord", Value = ConstHelper.PassWordRegex });
                        break;
                    case "BlogName":
                        result.Add(new TreeNodeModel() { Key = "BlogName", Value = ConstHelper.BlogNameRegex });
                        break;
                }
            }
            return APITResult<List<TreeNodeModel>>.Success(result.Count == 0 ? "无匹配信息" : "获取成功", result);
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="enumNames">枚举名称,多个用","分隔</param>
        /// <returns></returns>
        [HttpGet]
        public APITResult<IEnumerable<EnumModel>> GetEnums(string enumNames)
        {
            IEnumerable<EnumModel> enumModels = Enumerable.Empty<EnumModel>();
            try
            {
                List<Type> listType = new List<Type>();
                foreach (var item in enumNames.Split(','))
                {
                    listType.Add(Assembly.GetAssembly(typeof(IModel)).GetType($"TBlog.Model.{item}"));
                }
                if (listType.Any())
                {
                    enumModels = (from enumType in listType
                                  from c in enumType.GetFields()
                                  where c.FieldType == enumType
                                  group c by c.FieldType into temp1
                                  let tlist = temp1.ToArray()
                                  select new EnumModel
                                  {
                                      Name = temp1.Key.Name,
                                      EnumKeyValues = tlist.Select(s => new KeyValueModel
                                      {
                                          Key = s.GetRawConstantValue().ToString(),
                                          Value = s.GetCustomAttributes(typeof(DescriptionAttribute)).Count() > 0 ? ((DescriptionAttribute)s.GetCustomAttributes(typeof(DescriptionAttribute)).First()).Description : string.Empty
                                      })
                                  });
                }
            }catch (Exception ex)
            {
                APITResult<IEnumerable<EnumModel>>.Fail();
            }
            return APITResult<IEnumerable<EnumModel>>.Success(enumModels);
        }
    }
}
