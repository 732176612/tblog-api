using AutoMapper;
using System.Reflection;
using System.Text.RegularExpressions;
namespace TBlog.Model
{
    public static class AutoMapperExtension
    {
        public readonly static AutoMapperConvert AutoMapperConvert = new AutoMapperConvert();

        private static Dictionary<string, IMapper> MapperConfigs = new Dictionary<string, IMapper>();

        static AutoMapperExtension()
        {
            //找到AutoMapperConfig的父类
            var autoMapperConfigType = typeof(AutoMapperConfig<,>);
            var test = typeof(ActicleAutoMapperConfig);
            var autoMapperConfigTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType?.Name == autoMapperConfigType.Name);
            foreach (var type in autoMapperConfigTypes)
            {
                Activator.CreateInstance(type);
            }
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static TEntity ToEntity<TEntity, TDto>(this TDto dto, bool isVerify = true) where TEntity : IEntity where TDto : IDto
        {
            if (dto == null) return default;
            IMapper mapper = GetMapper<TDto, TEntity>();
            TEntity entity = mapper.Map<TEntity>(dto);
            if (isVerify) VerifyEntity(entity);
            return entity;
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static TDto ToDto<TDto, TEntity>(this TEntity source) where TEntity : IEntity where TDto : IDto
        {
            if (source == null) return default;
            var mapper = GetMapper<TEntity, TDto>();
            return mapper.Map<TDto>(source);
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static IEnumerable<TEntity> ToEntity<TEntity, TDto>(this IEnumerable<TDto> source, bool isVerify = true) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return Enumerable.Empty<TEntity>();
            var mapper = GetMapper<TDto, TEntity>();
            var entities = mapper.Map<IEnumerable<TDto>, IEnumerable<TEntity>>(source);
            if (isVerify)
            {
                string[] verifyFieIds = typeof(TDto).GetProperties().Select(c => c.Name).ToArray();
                foreach (var entity in entities)
                {
                    VerifyEntity(entity);
                }
                return entities;
            }
            return entities;
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static IEnumerable<TDto> ToDto<TDto, TEntity>(this IEnumerable<TEntity> source) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return Enumerable.Empty<TDto>();
            var mapper = GetMapper<TEntity, TDto>();
            return mapper.Map<IEnumerable<TEntity>, IEnumerable<TDto>>(source);
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static List<TEntity> ToEntity<TEntity, TDto>(this List<TDto> source, bool isVerify = true) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return new List<TEntity>();
            var mapper = GetMapper<TDto, TEntity>();
            var entities = mapper.Map<List<TDto>, List<TEntity>>(source);
            if (isVerify)
            {
                string[] verifyFieIds = typeof(TDto).GetProperties().Select(c => c.Name).ToArray();
                entities.ForEach(entity => VerifyEntity(entity));
                return entities;
            }
            return entities;
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static List<TDto> ToDto<TDto, TEntity>(this List<TEntity> source) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return new List<TDto>();
            var mapper = GetMapper<TEntity, TDto>();
            return mapper.Map<List<TEntity>, List<TDto>>(source);
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static TEntity[] ToEntity<TEntity, TDto>(this TDto[] source, bool isVerify = true) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return [];
            var mapper = GetMapper<TDto, TEntity>();
            var entities = mapper.Map<TDto[], TEntity[]>(source);
            if (isVerify)
            {
                string[] verifyFieIds = typeof(TDto).GetProperties().Select(c => c.Name).ToArray();
                foreach (var entity in entities)
                {
                    VerifyEntity(entity);
                }
                return entities;
            }
            return entities;
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        public static TDto[] ToDto<TDto, TEntity>(this TEntity[] source) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return [];
            var mapper = GetMapper<TEntity, TDto>();
            return mapper.Map<TEntity[], TDto[]>(source);
        }

        private static IMapper GetMapper<T1, T2>()
        {
            if (MapperConfigs.ContainsKey(typeof(T1).FullName + typeof(T2).FullName))
            {
                return  MapperConfigs[typeof(T1).FullName + typeof(T2).FullName];
            }

            if (MapperConfigs.ContainsKey(typeof(T2).FullName + typeof(T1).FullName))
            {
                return MapperConfigs[typeof(T2).FullName + typeof(T1).FullName];
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T1, T2>();
                cfg.CreateMap<T2, T1>();
                cfg.AddProfile(new BaseAutoMapperProfile());
            });
            var mapper = config.CreateMapper();
            MapperConfigs.Add(typeof(T1).FullName + typeof(T2).FullName, mapper);
            MapperConfigs.Add(typeof(T2).FullName + typeof(T1).FullName, mapper);
            return mapper;
        }

        public static IMapper SetMapper<T1, T2>(Action<IMapperConfigurationExpression> mappingExpression)
        {
            var config = new MapperConfiguration(cfg =>
            {
                mappingExpression(cfg);
                cfg.AddProfile(new BaseAutoMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            if (MapperConfigs.ContainsKey(typeof(T1).FullName + typeof(T2).FullName))
            {
                return MapperConfigs[typeof(T1).FullName + typeof(T2).FullName] = mapper;
            }
            if (MapperConfigs.ContainsKey(typeof(T2).FullName + typeof(T1).FullName))
            {
                return MapperConfigs[typeof(T2).FullName + typeof(T1).FullName] = mapper;
            }
            MapperConfigs.Add(typeof(T1).FullName + typeof(T2).FullName, mapper);
            MapperConfigs.Add(typeof(T2).FullName + typeof(T1).FullName, mapper);
            return mapper;
        }

        /// <summary>
        /// 反射校验实体
        /// </summary>
        public static void VerifyEntity<TEntity>(TEntity entity) where TEntity : IEntity
        {
            var properites = entity.GetType().GetProperties();
            foreach (var property in properites)
            {
                var propertyType = property.GetType();
                var sugarColumnAtt = propertyType.GetCustomAttribute<SugarColumn>();
                if (sugarColumnAtt != null)
                {
                    if (sugarColumnAtt.IsNullable == false)
                    {
                        if (string.IsNullOrEmpty(property.GetValue(entity)?.ToString()))
                        {
                            throw new TBlogApiException($"{propertyType.GetDescription()}不能为空;");
                        }
                    }
                }

                switch (Type.GetTypeCode(propertyType))
                {
                    case TypeCode.String:
                        var stringLength = propertyType.GetCustomAttribute<StringLengthAttribute>();
                        if (stringLength != null)
                        {
                            var strValue = property.GetValue(entity);
                            if ((strValue == null && stringLength.MinimumLength != 0) || strValue.ToString().Length == 0)
                            {
                                throw new TBlogApiException($"{propertyType.GetDescription()}长度不能为0");
                            }
                            var strValueToStr = strValue.ToString();
                            if (strValueToStr.Length > stringLength.MaximumLength)
                            {
                                throw new TBlogApiException($"{propertyType.GetDescription()}长度不能大于{stringLength.MaximumLength}");
                            }
                            if (strValueToStr.Length < stringLength.MinimumLength)
                            {
                                throw new TBlogApiException($"{propertyType.GetDescription()}长度不能小于{stringLength.MinimumLength}");
                            }
                        }
                        break;
                }

                var regexValid = propertyType.GetCustomAttribute<RegularExpressionAttribute>();
                if (regexValid != null)
                {
                    var strValue = property.GetValue(entity).ToString();
                    if (!Regex.IsMatch(strValue, regexValid.Pattern))
                    {
                        throw new TBlogApiException(regexValid.ErrorMessage);
                    }
                }
            }
            return;
        }
    }
}
