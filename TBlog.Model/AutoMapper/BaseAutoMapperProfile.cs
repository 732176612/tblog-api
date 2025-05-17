using AutoMapper;

namespace TBlog.Model
{
    public class BaseAutoMapperProfile : Profile
    {
        public BaseAutoMapperProfile()
        {
            AllowNullDestinationValues = true;
            AllowNullCollections = true;
            CreateMap<string, DateTime>().ConvertUsing(AutoMapperExtension.AutoMapperConvert);
            CreateMap<string, long>().ConvertUsing(AutoMapperExtension.AutoMapperConvert);
            CreateMap<DateTime, string>().ConvertUsing(AutoMapperExtension.AutoMapperConvert);
            CreateMap<long, string>().ConvertUsing(AutoMapperExtension.AutoMapperConvert);
            CreateMap<string, string>().ConvertUsing(AutoMapperExtension.AutoMapperConvert);
        }
    }
}
