using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using SqlSugar;
using TBlog.Common;
namespace TBlog.Extensions
{
    public class AutoMapperConvert : ITypeConverter<string, DateTime>, ITypeConverter<DateTime, string>,
    ITypeConverter<string, long>, ITypeConverter<long, string>, ITypeConverter<string, string>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            return source.ToDateTime().ToUniversalTime();
        }

        public long Convert(string source, long destination, ResolutionContext context)
        {
            return long.TryParse(source, out long result) ? result : 0;
        }

        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            DateTime dateTime = source.ToLocalTime();
            if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0)
            {
                return dateTime.Toyyyymmdd();
            }
            return dateTime.ToyMdHms();
        }

        public string Convert(long source, string destination, ResolutionContext context)
        {
            return source.ToString();
        }

        public string Convert(string source, string destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }
            return source;
        }
    }
}
