extern alias Exercise4;

using Exercise4.Ads.Exercise4;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise4.StackPostfixExpressionEvaluatorTests
{
    public class StackPostfixExpressionEvaluator_Tests
    {
        [Theory]
        [InlineData(59, "8 2 + 5 * 9 + =")]
        public void Should_Evaluate(double value, string expression)
        {
            StackPostfixExpressionEvaluator.Evaluate(expression).ShouldBe(value);
        }
    }
}
