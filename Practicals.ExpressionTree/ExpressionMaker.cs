using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Practicals.ExpressionTree
{
    public static class ExpressionMaker
    {
        public static Expression<Func<int, int>> MakeIncrementExpr()
        {
            var paramI = Expression.Parameter(typeof(int));

            return Expression.Lambda<Func<int, int>>(
                Expression.AddAssign(paramI, Expression.Constant(1)), paramI);
        }

        public static Expression<Func<int, int>> MakeSimpleCalcExpr()
        {
            var paramI = Expression.Parameter(typeof(int));
            var returnTarget = Expression.Label(typeof(int), "returnLabel");
            var returnLabel = Expression.Label(returnTarget, paramI);

            var block = Expression.Block(
                Expression.AddAssign(paramI, Expression.Constant(5)),
                Expression.MultiplyAssign(paramI, Expression.Constant(10)),
                Expression.DivideAssign(paramI, Expression.Constant(2)),
                Expression.Return(returnTarget, paramI, typeof(int)),
                returnLabel);

            return Expression.Lambda<Func<int, int>>(block, paramI);
        }

        public static Expression<Func<string, int>> MakeGetStrLengthExpr()
        {
            var paramStr = Expression.Parameter(typeof(string));

            return Expression.Lambda<Func<string, int>>(Expression.Property(paramStr, "Length"), paramStr);
        }

        public static Expression<Func<SimpleClass>> MakeSimpleConstructorExpr()
            => Expression.Lambda<Func<SimpleClass>>(Expression.New(typeof(SimpleClass)));

        public static Expression<Func<string, int, SimpleClass>> MakeCreateNewExpr()
        {
            var paramStr = Expression.Parameter(typeof(string));
            var paramI = Expression.Parameter(typeof(int));
            var result = Expression.Variable(typeof(SimpleClass));

            var ctor = Expression.New(typeof(SimpleClass));
            var strProp = Expression.Property(result, "Name");
            var iProp = Expression.Property(result, "SomeProperty");

            var returnTarget = Expression.Label(typeof(SimpleClass), "returnLabel");
            var returnLabel = Expression.Label(returnTarget, ctor);

            var block = Expression.Block(new[] { result },
                Expression.Assign(result, ctor),
                Expression.Assign(strProp, paramStr),
                Expression.Assign(iProp, paramI),
                Expression.Return(returnTarget, result),
                returnLabel);

            return Expression.Lambda<Func<string, int, SimpleClass>>(block, paramStr, paramI);
        }

        public static Expression<Action<SimpleClass, string, int>> MakeModifyExpr()
        {
            var paramClass = Expression.Parameter(typeof(SimpleClass));
            var paramStr = Expression.Parameter(typeof(string));
            var paramI = Expression.Parameter(typeof(int));

            var strProp = Expression.Property(paramClass, "Name");
            var iProp = Expression.Property(paramClass, "SomeProperty");

            var block = Expression.Block(
                Expression.Assign(strProp, paramStr),
                Expression.Assign(iProp, paramI));

            return Expression.Lambda<Action<SimpleClass, string, int>>(block, paramClass, paramStr, paramI);
        }

        public static Expression<Func<SimpleClass, string>> MakeDisplayExpr()
        {
            var paramClass = Expression.Parameter(typeof(SimpleClass));
            var result = Expression.Variable(typeof(string));
            var strProp = Expression.Property(paramClass, "Name");
            var iProp = Expression.Property(paramClass, "SomeProperty");

            var formatMi = typeof(string).GetMethods().First(mi =>
                mi.GetParameters().Length == 3 &&
                mi.Name == nameof(string.Format));
            var writeLineMi = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });

            var formatCall = Expression.Call(formatMi,
                Expression.Constant("SimpleClass name : {0}, with property : {1}"),
                Expression.Convert(strProp, typeof(object)),
                Expression.Convert(iProp, typeof(object)));

            var returnTarget = Expression.Label(typeof(string), "returnLabel");
            var returnLabel = Expression.Label(returnTarget, Expression.Constant(string.Empty));

            var block = Expression.Block(new[] { result },
                Expression.Assign(result, formatCall),
                Expression.Call(writeLineMi, result),
                Expression.Return(returnTarget, result),
                returnLabel);

            return Expression.Lambda<Func<SimpleClass, string>>(block, paramClass);
        }
    }
}
