extern alias Exercise4;

using Exercise4.Ads.Exercise4;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise4.BracketBalancerTests
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
        public void Should_CheckIsBalanced(bool isBalanced, string input)
        {
            BracketBalancer.CheckIsBalanced(input).ShouldBe(isBalanced);
        }

        [Theory]
        [InlineData(true, "([({()})])")]
        [InlineData(false, "((())})")]
        [InlineData(false, "((){())}")]
        [InlineData(false, "(()()))")]
        [InlineData(true, "({()((())[()])})")]
        [InlineData(true, "(()()(()))")]
        public void Should_AdvancedCheckIsBalanced(bool isBalanced, string input)
        {
            BracketBalancer.AdvancedCheckIsBalanced(input).ShouldBe(isBalanced);
        }
    }
}
