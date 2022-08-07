using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TBlog.Common
{
    public static class ReflectHelper
    {
        /// <summary>
        /// 获取泛型类型名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetGenericTypeName(this Type type)
        {
            var typeName = string.Empty;

            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }

        /// <summary>
        /// 是否为异步方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsyncMethod(this MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

        /// <summary>
        /// 获取类型注释
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(this Type type)
        {
            var displayAtt = type.GetCustomAttribute<DisplayAttribute>();
            if (displayAtt != null)
            {
                return displayAtt.Name;
            }
            var descriptionAtt = type.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAtt != null)
            {
                return descriptionAtt.Description;
            }
            var result = DocsByReflection.XMLFromTypeInnerText(type);
            if (string.IsNullOrEmpty(result))
            {
                return type.Name;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 获取字段注释
        /// </summary>
        /// <param name="meber"></param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo meber)
        {
            var displayAtt = meber.GetCustomAttribute<DisplayAttribute>();
            if (displayAtt != null)
            {
                return displayAtt.Name;
            }
            var descriptionAtt = meber.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAtt != null)
            {
                return descriptionAtt.Description;
            }
            var result = DocsByReflection.XMLFromMemberInnerText(meber);
            if (string.IsNullOrEmpty(result))
            {
                return meber.Name;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 获取枚举注释
        /// </summary>
        /// <param name="@enum"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum @enum)
        {
            var enumType = @enum.GetType().GetField(@enum.ToString());
            return enumType.GetDescription();
        }
    }
}
