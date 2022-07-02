using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TBlog.Common
{
    public class DocsByReflection
    {
        /// <summary>
        /// 获取字段注释
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string XMLFromMemberInnerText(MemberInfo memberInfo)
        {
            try
            {
                return XMLFromName(memberInfo.DeclaringType, memberInfo.MemberType.ToString()[0], memberInfo.Name)["summary"].InnerText.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取类名注释
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string XMLFromTypeInnerText(Type type)
        {
            try
            {
                return XMLFromName(type, 'T', "")["summary"].InnerText.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        private static XmlElement XMLFromName(Type type, char prefix, string name)
        {
            string fullName;

            if (string.IsNullOrEmpty(name))
            {
                fullName = prefix + ":" + type.FullName;
            }
            else
            {
                fullName = prefix + ":" + type.FullName + "." + name;
            }

            XmlDocument xmlDocument = XMLFromAssembly(type.Assembly);

            XmlElement matchedElement = null;

            foreach (XmlNode xn in xmlDocument["doc"]["members"])
            {
                XmlElement xmlElement = xn as XmlElement;
                if (xmlElement == null)
                    continue;
                if (xmlElement.Attributes["name"].Value.Equals(fullName))
                {
                    if (matchedElement != null)
                    {
                        throw new Exception("Multiple matches to query", null);
                    }
                    matchedElement = xmlElement;
                }
            }

            if (matchedElement == null)
            {
                throw new Exception("Could not find documentation for specified element", null);
            }

            return matchedElement;
        }

        static Dictionary<Assembly, XmlDocument> cache = new Dictionary<Assembly, XmlDocument>();

        public static XmlDocument XMLFromAssembly(Assembly assembly)
        {
            if (!cache.ContainsKey(assembly))
            {
                cache[assembly] = XMLFromAssemblyNonCached(assembly);
            }
            return cache[assembly];
        }

        private static XmlDocument XMLFromAssemblyNonCached(Assembly assembly)
        {
            string assemblyFilename = assembly.Location;

            const string prefix = "file:///";

            if (assemblyFilename.StartsWith(prefix))
            {
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(Path.ChangeExtension(assemblyFilename.Substring(prefix.Length), ".xml"));
                }
                catch (FileNotFoundException exception)
                {
                    throw new Exception("XML documentation not present (make sure it is turned on in project properties when building)", exception);
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(streamReader);
                return xmlDocument;
            }
            else
            {
                throw new Exception("Could not ascertain assembly filename", null);
            }
        }
    }
}
