extern alias Exercise6;

using Exercise6.Ads.Exercise6;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_6.StringExtensionsTests
{
    public class StringExtensions_GetIsPalindrome_Tests
    {
        [Theory]
        [InlineData("123 33 33 321", true)]
        [InlineData("123 31 33 321", false)]
        [InlineData("abfba", true)]
        public void Should_GetIsPalindrome(string str, bool result)
        {
            str.GetIsPalindrome().ShouldBe(result);
        }
    }
}
