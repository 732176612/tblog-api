using AutoMapper;

namespace TBlog.Model
{
    public class CommentAutoMapperConfig : AutoMapperConfig<CommentEntity, CommentDto>
    {
        protected override Action<IMapperConfigurationExpression> Configure()
        {
            return (cfg) =>
            {
                cfg.CreateMap<CommentDto, CommentEntity>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore());

                cfg.CreateMap<CommentEntity, CommentDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children))
                .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.Parent))
                .ForMember(dest => dest.ShowCollapse, opt => opt.Ignore())
                .ForMember(dest => dest.IsCollapsed, opt => opt.Ignore())
                .ForMember(dest => dest.IsLiked, opt => opt.Ignore());

                cfg.CreateMap<UserEntity, UserInfoDto>();


                cfg.AddProfile(new BaseAutoMapperProfile());
            };
        }
    }
} 