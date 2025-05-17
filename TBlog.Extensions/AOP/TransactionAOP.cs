using Castle.DynamicProxy;
using System.Reflection;
using TBlog.Common;
using TBlog.IRepository;
namespace TBlog.Extensions
{
    /// <summary>
    /// 事务拦截器 继承IInterceptor接口
    /// </summary>
    public class TransactionAOP : IInterceptor
    {
        private readonly IMongoTransaction _mongoTransaction;
        private readonly ISqlSugarTransaction _sqlSugarTransaction;
        public TransactionAOP(IMongoTransaction mongoTransaction, ISqlSugarTransaction sqlSugarTransaction)
        {
            _mongoTransaction = mongoTransaction;
            _sqlSugarTransaction = sqlSugarTransaction;
        }

        /// <summary>
        /// 实例化IInterceptor唯一方法 
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            //获取Service的构造参数
            var construtorParmeters = invocation.TargetType.GetConstructors().SelectMany(c => c.GetParameters());
            //判断构造参数中任意一个是否继承了IMongoRepository
            var Parameters = invocation.TargetType.GetConstructors().SelectMany(c => c.GetParameters()).Select(c=>c.ToString());
            var isMongoDB = Parameters.Any(c => c.Contains("IMongoRepository"));
            ITransaction transaction = isMongoDB ? _mongoTransaction : _sqlSugarTransaction;
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(TransactionAttribute)) is TransactionAttribute)
            {
                try
                {
                    transaction.BeginTran();
                    invocation.Proceed();
                    // 异步获取异常，先执行
                    if (IsAsyncMethod(invocation.Method))
                    {
                        var result = invocation.ReturnValue;
                        if (result is Task)
                        {
                            Task.WaitAll(result as Task);
                        }
                    }
                    transaction.CommitTran();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Rollback Transaction");
                    transaction.RollbackTran();
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }
    }
}
