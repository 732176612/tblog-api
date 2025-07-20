using AutoMapper;
namespace TBlog.Model
{
    public abstract class AutoMapperConfig<T1, T2>
    {
        protected abstract Action<IMapperConfigurationExpression> Configure();
        public AutoMapperConfig()
        {
            AutoMapperExtension.SetMapper<T1, T2>(Configure());
        }
    }
}
