using AutoMapper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBlog.Common;
using TBlog.Model;

namespace TBlog.Extensions
{
    public static class AutoMapperExtension
    {
        private static AutoMapperConvert AutoMapperConvert = new AutoMapperConvert();

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <typeparam name="TDto">数据传输对象</typeparam>
        /// <returns>转化之后的实体</returns>
        public static TEntity ToEntity<TEntity, TDto>(this TDto source, bool isVerify = true) where TEntity : IEntity where TDto : IDto
        {
            if (source == null) return default(TEntity);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TDto, TEntity>();
                cfg.CreateMap<string, DateTime>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, long>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<DateTime, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<long, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, string>().ConvertUsing(AutoMapperConvert);
            });
            var mapper = config.CreateMapper();
            TEntity entity = mapper.Map<TEntity>(source);
            if (isVerify)
            {
                return VerifyEntity(entity, typeof(TDto).GetProperties().Select(c => c.Name).ToArray());
            }
            else
            {
                return entity;
            }
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <typeparam name="TDto">数据传输对象</typeparam>
        /// <returns>转化之后的实体</returns>
        public static TDto ToDto<TDto, TEntity>(this TEntity source) where TEntity : IEntity where TDto : IDto
        {
            if (source == null) return default(TDto);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TEntity, TDto>();
                cfg.CreateMap<DateTime, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<long, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, DateTime>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, long>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, string>().ConvertUsing(AutoMapperConvert);
            });
            var mapper = config.CreateMapper();
            return mapper.Map<TDto>(source);
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <typeparam name="TDto">数据传输对象</typeparam>
        /// <returns>转化之后的实体</returns>
        public static IEnumerable<TEntity> ToEntity<TEntity, TDto>(this IEnumerable<TDto> source, bool isVerify = true) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return Enumerable.Empty<TEntity>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TDto, TEntity>();
                cfg.CreateMap<string, DateTime>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, long>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<DateTime, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<long, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, string>().ConvertUsing(AutoMapperConvert);
            });
            IEnumerable<TEntity> entities = config.CreateMapper().Map<IEnumerable<TDto>, IEnumerable<TEntity>>(source);
            if (isVerify)
            {
                string[] verifyFieIds = typeof(TDto).GetProperties().Select(c => c.Name).ToArray();
                return entities.Select(c => VerifyEntity(c, verifyFieIds));
            }
            else
            {
                return entities;
            }
        }

        /// <summary>
        ///  类型映射,默认字段名字一一对应
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <typeparam name="TDto">数据传输对象</typeparam>
        /// <returns>转化之后的实体</returns>
        public static IEnumerable<TDto> ToDto<TDto, TEntity>(this IEnumerable<TEntity> source) where TEntity : IEntity where TDto : IDto
        {
            if (source == null || source.Any() == false) return Enumerable.Empty<TDto>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TEntity, TDto>();
                cfg.CreateMap<DateTime, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<long, string>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, DateTime>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, long>().ConvertUsing(AutoMapperConvert);
                cfg.CreateMap<string, string>().ConvertUsing(AutoMapperConvert);
            });
            return config.CreateMapper().Map<IEnumerable<TEntity>, IEnumerable<TDto>>(source);
        }

        /// <summary>
        /// 反射校验实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="verifyFields">需校验的字段</param>
        /// <returns></returns>
        public static TEntity VerifyEntity<TEntity>(TEntity entity, string[] verifyFields) where TEntity : IEntity
        {
            if (verifyFields == null || !verifyFields.Any()) return entity;
            var properites = entity.GetType().GetProperties().Where(p => verifyFields.Contains(p.Name)).ToArray();
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
                            throw new Exception($"{propertyType.GetDescription()}不能为空;");
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
                                throw new Exception($"{propertyType.GetDescription()}长度不能为0");
                            }
                            var strValueToStr = strValue.ToString();
                            if (strValueToStr.Length > stringLength.MaximumLength)
                            {
                                throw new Exception($"{propertyType.GetDescription()}长度不能大于{stringLength.MaximumLength}");
                            }
                            if (strValueToStr.Length < stringLength.MinimumLength)
                            {
                                throw new Exception($"{propertyType.GetDescription()}长度不能小于{stringLength.MinimumLength}");
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
                        throw new Exception(regexValid.ErrorMessage);
                    }
                }
            }
            return entity;
        }
    }
}
