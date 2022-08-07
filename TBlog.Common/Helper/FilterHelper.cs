using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace TBlog.Common
{
    public static class FilterHelper
    {
        /// <summary>
        /// 创建表达式
        /// </summary>
        public static Expression<Func<T, bool>> CreateExp<T>(Expression<Func<T, bool>> filterExp)
        {
            return filterExp;
        }

        /// <summary>
        /// 合并表达式
        /// </summary>
        public static Expression<Func<TEntityType, bool>> AddExp<TEntityType>(this Expression<Func<TEntityType, bool>> left,
            Expression<Func<TEntityType, bool>> right = null)
        {
            if (left == null) { left = e => true; }
            if (right == null) { right = e => true; }
            return CombineExpression(left, right);
        }

        /// <summary>
        /// 合并表达式
        /// </summary>
        public static Expression<Func<TEntityType, bool>> AddExp<TEntityType>(this Expression<Func<TEntityType, bool>> left, bool isAdd, Expression<Func<TEntityType, bool>> right)
        {
            if (left == null) { left = e => true; }
            if (right == null) { right = e => true; }
            if (isAdd)
                return CombineExpression(left, right);
            else
                return left;
        }

        /// <summary>
        /// 合并表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CombineExpression<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null) { return right; }
            if (right == null) { return left; }
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(new ExpressionParmeterVisit(left.Parameters[0], right.Parameters[0]).Visit(left.Body), right.Body), right.Parameters);
        }
    }

    /// <summary>
    /// 将两个表达式树的参数变成一样（反编译后极其复杂,暂时搞不懂，以后再来搞）
    /// </summary>
    public class ExpressionParmeterVisit : ExpressionVisitor
    {
        private readonly Expression from;
        private readonly Expression to;

        public ExpressionParmeterVisit(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node)
        {
            if (from == node)
            {
                return to;
            }
            else
            {
                return base.Visit(node);
            }
        }
    }
}
