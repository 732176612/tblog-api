using AutoMapper;
namespace TBlog.Model
{
    public class AutoMapperConvert : ITypeConverter<string, DateTime>, ITypeConverter<DateTime, string>,
    ITypeConverter<string, long>, ITypeConverter<long, string>, ITypeConverter<string, string>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            return source.ToDateTime();
        }

        public long Convert(string source, long destination, ResolutionContext context)
        {
            return long.TryParse(source, out long result) ? result : 0;
        }

        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            if (source.Minute == 0 && source.Second == 0)
            {
                return source.Toyyyymmdd();
            }
            return source.ToyMdHms();
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
