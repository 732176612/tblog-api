using Autofac;
namespace TBlog.Extensions
{
    public class ContainerHelper
    {
        private static IContainer _container;

        public static void RegisterContainer(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// 根据类型获取组件
        /// </summary>
        public static TService Resolve<TService>()
        {
            return _container.Resolve<TService>();
        }

        /// <summary>
        /// 根据名称获取组件
        /// </summary>
        /// <param name="name">组件名称</param>
        public static TService ResolveNamed<TService>(string name)
        {
            return _container.ResolveNamed<TService>(name);
        }
    }
}