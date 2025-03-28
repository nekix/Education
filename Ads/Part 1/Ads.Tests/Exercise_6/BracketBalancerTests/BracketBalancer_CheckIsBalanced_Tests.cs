extern alias Exercise6;
using Exercise6::Ads.Exercise6;
using Shouldly;
using Xunit;

namespace Ads.Tests.Exercise_6.BracketBalancerTests
{
    public class BracketBalancer_CheckIsBalanced_Tests
    {
        [Theory]
        [InlineData(true, "((()))")]
        [InlineData(false, "((())")]
        [InlineData(true, "(()())")]
        [InlineData(false, "(()()))")]
        [InlineData(true, "(()((())()))")]
        [InlineData(true, "(()()(()))")]
        [InlineData(true, "(1 * 5) * 3")]
        [InlineData(true, "(1 * 5) * 3 + {5 * 6}")]
        [InlineData(true, "[(15 - 3) * (x + y) * {(12 - 15) / 12}] * [2 - 3]")]
        [InlineData(false, "(1 * 5) * 3]")]
        [InlineData(false, "(1 * 5) * 3 + {[5 * 6}")]
        [InlineData(false, "[(15 - 3) * (x + y) * ((12 - 15) / 12}] * [2 - 3]")]
        public void Should_CheckIsBalanced(bool isBalanced, string input)
        {
            BracketBalancer.CheckIsBalanced(input).ShouldBe(isBalanced);
        }

    }
}
