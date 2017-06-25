using System;
using System.Linq.Expressions;

namespace Practicals.ExpressionTree
{
    public static class ExpressionMaker
    {
        public static Expression<Func<int, int>> MakeIncrementExpr()
            => null;

        public static Expression<Func<int, int>> MakeSimpleCalcExpr()
            => null;

        public static Expression<Func<string, int>> MakeGetStrLengthExpr()
            => null;

        public static Expression<Func<SimpleClass>> MakeSimpleConstructorExpr()
            => null;

        public static Expression<Func<string, int, SimpleClass>> MakeCreateNewExpr()
            => null;

        public static Expression<Action<SimpleClass, string, int>> MakeModifyExpr()
            => null;

        public static Expression<Func<SimpleClass, string>> MakeDisplayExpr()
            => null;
    }
}
