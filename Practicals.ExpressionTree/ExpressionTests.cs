using System;
using System.Linq.Expressions;
using Xunit;
// ReSharper disable ConvertToConstant.Local

namespace Practicals.ExpressionTree
{
    /// <summary>
    /// This class is a test provider for expression trees exercises
    /// </summary>
    /// <seealso cref="LambdaExpression"/> https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/
    /// <seealso cref="Func{TResult}"/> https://docs.microsoft.com/en-us/dotnet/csharp/lambda-expressions
    /// <seealso cref="Action{TResult}"/>
    public class ExpressionTests
    {
        #region Original functions
        /// <summary>
        /// Base functions to translate to ET, you better do it from first to last
        /// </summary>
        private static Func<int, int> IncrementOriginal => i => i + 1;
        private static Func<int, int> SimpleCalcOriginal => i => ((i + 5) * 10) / 2;
        private static Func<string, int> GetStrLengthOriginal => s => s.Length;
        private static Func<SimpleClass> SimpleConstructorOriginal => () => new SimpleClass();

        private static Func<string, int, SimpleClass> CreateNewOriginal => (name, prop) =>
        {
            // use your own functions on your ET, not mine :)
            var simpleClass = SimpleConstructorOriginal();

            simpleClass.Name = name;
            simpleClass.SomeProperty = prop;

            return simpleClass;
        };

        private static Action<SimpleClass, string, int> ModifyOriginal => (sClass, name, prop) =>
        {
            sClass.Name = name;
            sClass.SomeProperty = prop;
        };

        // Bonus exercice, you don't have to it because it is more advanced (require reflection logic) 
        // but you can try
        private static Func<SimpleClass, string> DisplayOriginal => sClass =>
        {
            // ET doesn't support string interpolation, therefore you have to use string.Format to acquire the result
            // just like this
            var result = string.Format("SimpleClass name : {0}, with property : {1}", sClass.Name, sClass.SomeProperty);
            Console.WriteLine(result);

            return result;
        };
        #endregion

        #region Compiled ET to create

        public static Func<int, int> Increment { get; }
        public static Func<int, int> SimpleCalc { get; }
        public static Func<string, int> GetStrLength { get; }
        public static Func<SimpleClass> SimpleConstructor { get; }
        public static Func<string, int, SimpleClass> CreateNew { get; }
        public static Action<SimpleClass, string, int> Modify { get; }
        public static Func<SimpleClass, string> Display { get; }

        #endregion

        static ExpressionTests()
        {
            Increment = ExpressionMaker.MakeIncrementExpr().Compile();
            SimpleCalc = ExpressionMaker.MakeSimpleCalcExpr().Compile();
            GetStrLength = ExpressionMaker.MakeGetStrLengthExpr().Compile();
            SimpleConstructor = ExpressionMaker.MakeSimpleConstructorExpr().Compile();
            CreateNew = ExpressionMaker.MakeCreateNewExpr().Compile();
            Modify = ExpressionMaker.MakeModifyExpr().Compile();
            Display = ExpressionMaker.MakeDisplayExpr().Compile();
        }

        #region Tests
        [Fact]
        public void ShouldIncrement()
        {
            var originalI = 0;
            var i = 0;

            var originalResult = IncrementOriginal(originalI);
            var result = Increment(i);

            Assert.Equal(1, result);
            Assert.Equal(1, originalResult);
            Assert.Equal(originalResult, result);
        }

        [Fact]
        public void ShouldCalculate()
        {
            var originalI = 2;
            var i = 2;

            var expected = ((i + 5) * 10) / 2;

            var originalResult = SimpleCalcOriginal(originalI);
            var result = SimpleCalc(i);

            Assert.Equal(expected, result);
            Assert.Equal(expected, originalResult);
            Assert.Equal(originalResult, result);
        }

        [Fact]
        public void ShouldGetStrLen()
        {
            var str = "four";

            var originalResult = GetStrLengthOriginal(str);
            var result = GetStrLength(str);

            Assert.Equal(4, result);
            Assert.Equal(4, originalResult);
            Assert.Equal(originalResult, result);
        }

        [Fact]
        public void ShouldConstruct()
        {
            var sClass = new SimpleClass();
            var expected = sClass.GetHashCode();

            var originalResult = SimpleConstructorOriginal();
            var result = SimpleConstructor();

            Assert.Equal(expected, result.GetHashCode());
            Assert.Equal(expected, originalResult.GetHashCode());
            Assert.Equal(originalResult.GetHashCode(), result.GetHashCode());
        }

        [Fact]
        public void ShouldCreateNew()
        {
            var sClass = new SimpleClass()
            {
                Name = "Nameless",
                SomeProperty = 48
            };

            var expected = sClass.GetHashCode();

            var originalResult = CreateNewOriginal("Nameless", 48);
            var result = CreateNew("Nameless", 48);

            Assert.Equal(expected, result.GetHashCode());
            Assert.Equal(expected, originalResult.GetHashCode());
            Assert.Equal(originalResult.GetHashCode(), result.GetHashCode());
        }

        [Fact]
        public void ShouldModify()
        {
            var sClass = new SimpleClass()
            {
                Name = "Nameless",
                SomeProperty = 48
            };

            var expected = sClass.GetHashCode();

            var newClassOriginal = SimpleConstructorOriginal();
            var newClass = SimpleConstructorOriginal();

            ModifyOriginal(newClassOriginal, "Nameless", 48);
            Modify(newClass, "Nameless", 48);

            Assert.Equal(expected, newClass.GetHashCode());
            Assert.Equal(expected, newClassOriginal.GetHashCode());
            Assert.Equal(newClassOriginal.GetHashCode(), newClass.GetHashCode());
        }

        [Fact]
        public void ShouldDisplay()
        {
            var sClass = new SimpleClass()
            {
                Name = "Nameless",
                SomeProperty = 48
            };

            var expected = "SimpleClass name : Nameless, with property : 48";

            var originalResult = DisplayOriginal(sClass);
            var result = Display(sClass);

            Assert.Equal(expected, result);
            Assert.Equal(expected, originalResult);
            Assert.Equal(originalResult, result);
        }
        #endregion
    }

    public class SimpleClass
    {
        public string Name { get; set; }
        public int SomeProperty { get; set; }

        public override bool Equals(object obj)
        {
            var casted = (SimpleClass)obj;

            return string.Equals(Name, casted.Name) && SomeProperty == casted.SomeProperty;
        }

        protected bool Equals(SimpleClass other)
        {
            return string.Equals(Name, other.Name) && SomeProperty == other.SomeProperty;
        }

        // just for test, never use length prop to override GetHashCode method
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.Length : 0) * 397) ^ SomeProperty;
            }
        }
    }
}

