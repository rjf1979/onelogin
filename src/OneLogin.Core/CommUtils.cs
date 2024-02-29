using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OneLogin.Core
{
    public static class CommUtils
    {
        public static IList<IList<T>> ToPageList<T>(IList<T> tList,int pageSize=100)
        {
            IList<IList<T>> list = new List<IList<T>>();
            int pageIndex = 1;
            int pageCount = tList.Count / pageSize;
            if (tList.Count % pageSize > 0) pageCount++;
            while (pageIndex<=pageCount)
            {
                var sList = tList.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList();
                list.Add(sList);
                pageIndex++;
            }

            return list;
        }
    }

    public static class ExpressionClone<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> _cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            var memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Clone(TIn tIn)
        {
            return _cache(tIn);
        }
    }
}
