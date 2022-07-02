using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Common.Help
{
    public static class EnumHelper
    {
        //
        // 摘要:
        //     获取枚举的描述值
        //
        // 参数:
        //   enumSubitem:
        //     枚举的元素
        public static string GetEnumDescription(Enum enumSubitem)
        {
            string text = enumSubitem.ToString();
            FieldInfo field = enumSubitem.GetType().GetField(text);
            if (field == null)
            {
                return null;
            }

            object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
            if (customAttributes == null || customAttributes.Length == 0)
            {
                return text;
            }

            return ((DescriptionAttribute)customAttributes[0]).Description;
        }

        //
        // 摘要:
        //     获取枚举的描述值
        //
        // 参数:
        //   enumSubitem:
        //     枚举的元素
        public static string GetDescription(this Enum enumSubitem)
        {
            return GetEnumDescription(enumSubitem);
        }

        //
        // 摘要:
        //     获取枚举描述
        //
        // 参数:
        //   type:
        //     枚举类型
        //
        //   value:
        //     枚举hasecode
        public static string GetEnumDescription(Type type, object value)
        {
            string result = value.ToString();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }

            FieldInfo field = type.GetField(name);
            if (field == null)
            {
                return null;
            }

            object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
            if (customAttributes == null || customAttributes.Length == 0)
            {
                return result;
            }

            return ((DescriptionAttribute)customAttributes[0]).Description;
        }
    }
}
