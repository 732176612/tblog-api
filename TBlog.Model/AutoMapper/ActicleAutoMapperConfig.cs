using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Model
{
    public class ActicleAutoMapperConfig : AutoMapperConfig<ActicleEntity, ActicleDto>
    {
        protected override Action<IMapperConfigurationExpression> Configure()
        {
            return (cfg) =>
            {
                cfg.CreateMap<ActicleDto, ActicleEntity>()
                .ForMember(dest => dest.Tags, opt => opt
                    .MapFrom(src => src.Tags.Select(s => new ActicleTagEntity
                    {
                        ActicleId = ConvertHelper.ToLong(src.Id, 0),
                        Name = s
                    }).ToList())
                )
                .ForMember(dest => dest.Stats, opt => opt
                    .MapFrom(src => new ActicleStatsEntity
                    {
                        ActicleId = ConvertHelper.ToLong(src.Id, 0),
                        LookNum = src.LookNum,
                        LikeNum = src.LikeNum
                    }
                ));

                cfg.CreateMap<ActicleEntity, ActicleDto>()
                .ForMember(dest => dest.Tags, opt => opt
                    .MapFrom(src => src.Tags.Select(s => s.Name).ToList())
                )
                .ForMember(dest => dest.LookNum, opt => opt
                    .MapFrom(src => src.Stats.LookNum)
                )
                .ForMember(dest => dest.LikeNum, opt => opt
                    .MapFrom(src => src.Stats.LikeNum)
                );

                cfg.AddProfile(new BaseAutoMapperProfile());
            };
        }
    }
}
